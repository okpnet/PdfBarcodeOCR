using BarcodeImageReader;
using DrageeScales.Views.Dtos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DrageeScales.Helper
{
    public static class ImageBarcodeHelper
    {
        private static int CheckPsition(int baselength,int position,int length)
        {
           return  baselength >= (length + position ) ? position :
               baselength - (length + position) > 0 ? position - (int)Math.Ceiling((decimal)((baselength - position - length) / 2)) : 0;
                    
        }

        private static int CheckLengh(int baselength, int position, int length)
        {
            return baselength >= (length + position) ? length :
                baselength - (length + position) > 0 ? length - (int)Math.Floor((decimal)((baselength - position - length) / 2)) : baselength;  
        }

        private static Rectangle CheckedRect(int maxWidth,int maxHeight,int x,int y,int w, int h)
        {
            var nX = CheckPsition(maxWidth, x, w);
            var nY = CheckPsition(maxHeight, y, h);
            var nW = CheckLengh(maxWidth, x, w);
            var nH = CheckLengh(maxHeight, y, h);

            return new Rectangle(nX, nY, nW, nH);
        }
        /// <summary>
        /// イメージからバーコード読み込み
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static async Task<BarcodeParameter> GetBarcodeResult(this Bitmap bitmap,IProgress<int> progress)
        {
            progress.Report(25);
            BarcodeParameter result = default!;

            var maxWidth = bitmap.Width;
            var maxHeight= bitmap.Height;

            result = await Task.Run(() =>
            {
                var result = BarcodeReader.ReadFromBitmap(bitmap);
                if (result.TryGetResultValue(out var barcode))
                {
                    //バーコード切り抜きサイズ
                    var overrapW = (int)((barcode.Rect.Width * 1.2 - barcode.Rect.Width) / 2);

                    var rect = CheckedRect(
                        maxWidth,
                        maxHeight,
                        barcode.Rect.X - overrapW,
                        barcode.Rect.Y - AppDefine.BARCODE_IMAGE_OVERLAP,
                        barcode.Rect.Width + (overrapW * 2),//Xのオーバーラップの分
                        barcode.Rect.Height + (AppDefine.BARCODE_IMAGE_OVERLAP * 2)//Yのオーバーラップの分
                        );
                    return BarcodeParameter.FromSuccess(barcode.Value,rect);
                }
                return BarcodeParameter.FromUnableRed();
            });



            if (result.IsSucces)
            {
                return result;
            }

            progress.Report(50);
            Image[] images = [];
            var strechWidth = (int)(bitmap.Width * AppDefine.STRECH_WIDTH);

            try
            {
                images = ImageShredded.BmpShredded.GetHorizotalShredded(bitmap, AppDefine.SHREDDED_HEIGHT).ToArray();
                //2回目。千切りにして読みやすくする
                result = await Task.Run(() => images.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(AppDefine.SHREDDED_HEIGHT));

                if (result.IsSucces)
                {
                    //バーコード切り抜きサイズ
                    var overrapW = (int)((result.Rectangles.Width * 1.2 - result.Rectangles.Width) / 2);

                    var rect = CheckedRect(
                        maxWidth,
                        maxHeight,
                        result.Rectangles.X - overrapW,
                        result.Rectangles.Y - overrapW,
                        result.Rectangles.Width + (overrapW * 3),
                        result.Rectangles.Height + (overrapW * 3)
                        );

                    return BarcodeParameter.FromSuccess(result.Value, rect, true);
                }
                progress.Report(75);
                result = await Task.Run(() =>
                {
                    //3回目。千切りを引き延ばして空白スペースを確保する
                    var strechImages = images.Select((t,i) =>
                    {
                        var strechImage = new Bitmap(strechWidth, AppDefine.SHREDDED_HEIGHT);
                        using var grph = Graphics.FromImage(strechImage);
                        grph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        grph.DrawImage(t, new Rectangle(0, 0, strechWidth, AppDefine.SHREDDED_HEIGHT));//ここのTをstrechImageにするとエラーにできる
                        return strechImage;
                    }).ToArray();
                    var strechResult = strechImages.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(AppDefine.SHREDDED_HEIGHT);
                    foreach (var strechImage in strechImages)
                    {
                        strechImage.Dispose();
                    }
                    if (strechResult.IsSucces)
                    {
                        //バーコード切り抜きサイズ
                        var overrapW =(int)((strechResult.Rectangles.Width * 1.2 - strechResult.Rectangles.Width)/2);

                        var prevRect = CheckedRect(
                            maxWidth,
                            maxHeight,
                            (int)(strechResult.Rectangles.X / AppDefine.STRECH_WIDTH) - overrapW,
                            strechResult.Rectangles.Y - overrapW,
                            (int)(strechResult.Rectangles.Width / AppDefine.STRECH_WIDTH) + (overrapW * 2),//Xのオーバーラップの分
                            (int)(strechResult.Rectangles.Height / AppDefine.STRECH_WIDTH) + (overrapW * 2)
                            );//Yのオーバーラップの分

                        return BarcodeParameter.FromSuccess(strechResult.Value, prevRect, true);
                    }
                    return strechResult;
                });

                return result;

            }
            finally
            {
                foreach (var image in images)
                {
                    image.Dispose();
                }
                progress.Report(100);
                await Task.Delay(500);
            }
        }


        /// <summary>
        /// 垂直方向に水平に千切りしたイメージからバーコード読み込み
        /// </summary>
        /// <param name="images"></param>
        /// <param name="shreddedpixel"></param>
        /// <returns></returns>
        internal static BarcodeParameter GetBarcodeValueShreddedHorizontal(this IEnumerable<Bitmap> images, int shreddedHeight)
        {
            var resultList = new List<IBarcodeItem>(images.Count());
            var index = 0;
            foreach (var shreddeddImage in images.OfType<Bitmap>())
            {
                index += 1;
                var result = BarcodeReader.ReadFromBitmap(shreddeddImage);
                resultList.Add(result);
            }
            if (!resultList.Where(t => t.IsSuccessRead).Any())
            {
                return BarcodeParameter.FromUnableRed();
            }
            
            var key = resultList.Where(t => t.IsSuccessRead).
                Select(t => t.TryGetResultValue(out var buffer) ? buffer.Value : "").
                GroupBy(t => t).
                Aggregate((a, b) => a.Count() > b.Count() ? a : b).
                Key;
            return BarcodeParameter.FromSuccess(
                key,
                GetHorizontalShreddedRect(resultList, key, shreddedHeight),
                true
                );
        }

        /// <summary>
        /// 千切りイメージから位置を取得
        /// </summary>
        /// <param name="items"></param>
        /// <param name="valueKey"></param>
        /// <param name="shreddedHeight"></param>
        /// <returns></returns>
        internal static Rectangle GetHorizontalShreddedRect(IEnumerable<IBarcodeItem> items, string valueKey, int shreddedHeight)
        {
            var records = items.Select((t,i)=>new { val=t, index=i }).Where(t => t.val.IsSuccessRead).Select(t =>
            {
                t.val.TryGetResultValue(out var resultValue);
                return new { t.index, item = resultValue };
            });

            var minx= records.Where(t => t is not null && t.item.Value == valueKey).Min(t => t.item.Rect.X);
            var maxx = records.Where(t => t is not null && t.item.Value == valueKey).Select(t => t.item.Rect.Width + t.item.Rect.X).Max();
            var miny = records.Where(t => t is not null && t.item.Value == valueKey).Min(t => t.index) * shreddedHeight - shreddedHeight * 2;
            var maxy = records.Where(t => t is not null && t.item.Value == valueKey).Max(t =>t.index) * shreddedHeight;


            return new Rectangle(minx, (maxy - miny) / 2 + miny, maxx - minx, maxx - minx);

            //var posX = records.Where(t => t is not null && t.item.Value == valueKey).Min(t => t.item.Rect.X);
            //var posY = records.Where(t => t.item is not null && t.item.Value == valueKey).Min(t => t.index) * shreddedHeight - shreddedHeight;
            //var width = records.Where(t => t is not null && t.item.Value == valueKey).Select(t => t.item.Rect.Width + t.item.Rect.X).Max() - posX;
            //var height = records.Where(t => t is not null && t.item.Value == valueKey).Max(t => t.index) * shreddedHeight +(shreddedHeight * 2) - posY;
            //return new Rectangle(posX, posY, width, height);
        }
        /// <summary>
        /// ファイル名
        /// </summary>
        /// <param name="pageItem"></param>
        /// <param name="outputDirictryName"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal static async Task<string> ChangeFileName(this PdfPageAdpter pageItem, string outputDirictryName, string filename)
        {
            if (pageItem.IsBusy)
            {
                var loopCounter = 0;
                while (pageItem.IsBusy)
                {
                    await Task.Delay(500);
                    if (loopCounter == 10)
                    {
                        return string.Empty;
                    }
                    loopCounter++;
                }
            }
            var newFilepath = Path.Combine(outputDirictryName, filename);
            return newFilepath;
        }
    }
}
