using BarcodeImageReader;
using DrageeScales.Shared.Dtos;
using DrageeScales.Views.Dtos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using ZXing.QrCode.Internal;

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
        public static async Task<BarcodeParameter> GetBarcodeResult(this Bitmap bitmap,AppSetting appSetting,IProgress<int> progress)
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
                    var barcodeValue = appSetting.RegularExpressionFilter is (null or "") ?
                        barcode.Value : System.Text.RegularExpressions.Regex.Match(barcode.Value, appSetting.RegularExpressionFilter).Value;

                    if(barcodeValue is (null or ""))
                    {
                        return BarcodeParameter.FromUnableRead();
                    }

                    //バーコード切り抜きサイズ
                    var overrapW = (int)((barcode.Rect.Width * 1.2 - barcode.Rect.Width) / 2);
                    var barLength = (barcode.Rect.Width > barcode.Rect.Height ? barcode.Rect.Width : barcode.Rect.Height) + overrapW;
                    var rect = CheckedRect(
                        maxWidth,
                        maxHeight,
                        barcode.Rect.X - overrapW,
                        barcode.Rect.Y - barLength / 2,
                        barLength,//Xのオーバーラップの分
                        barLength//Yのオーバーラップの分
                        );
                    return BarcodeParameter.FromSuccess(barcodeValue, rect);
                }
                return BarcodeParameter.FromUnableRead();
            });

            if (result.IsSucces)
            {
                return result;
            }

            progress.Report(50);
            Image[] images = [];
            var strechWidth = (int)(bitmap.Width *  appSetting.StretchLength);

            try
            {
                images = ImageShredded.BmpShredded.GetHorizotalShredded(bitmap, appSetting.ShreddedRate).ToArray();
                //2回目。千切りにして読みやすくする
                result = await Task.Run(() => images.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(appSetting));

                if (result.IsSucces)
                {
                    //バーコード切り抜きサイズ
                    var overrapW = (int)((result.Rectangles.Width * 1.2 - result.Rectangles.Width) / 2);
                    var barLength = (result.Rectangles.Width > result.Rectangles.Height ? result.Rectangles.Width : result.Rectangles.Height) + overrapW * 3;

                    var rect = CheckedRect(
                            maxWidth,
                            maxHeight,
                            result.Rectangles.X - overrapW,
                            result.Rectangles.Y - barLength / 2,
                            barLength,
                            barLength
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
                        grph.DrawImage(t, new Rectangle(0, 0, strechWidth, appSetting.ShreddedRate));//ここのTをstrechImageにするとエラーにできる
                        return strechImage;
                    }).ToArray();
                    var strechResult = strechImages.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(appSetting);
                    foreach (var strechImage in strechImages)
                    {
                        strechImage.Dispose();
                    }
                    if (strechResult.IsSucces)
                    {
                        //バーコード切り抜きサイズ
                        var overrapW =(int)((strechResult.Rectangles.Width * 1.2 - strechResult.Rectangles.Width)/2);
                        var barLength = (int)(strechResult.Rectangles.Width > strechResult.Rectangles.Height ? (strechResult.Rectangles.Width / appSetting.StretchLength) : (strechResult.Rectangles.Height / appSetting.StretchLength)) + overrapW * 2;

                        var prevRect = CheckedRect(
                            maxWidth,
                            maxHeight,
                            (int)(strechResult.Rectangles.X / appSetting.StretchLength) - overrapW,
                            strechResult.Rectangles.Y - barLength / 2,
                            barLength,//Xのオーバーラップの分
                            barLength
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
        internal static BarcodeParameter GetBarcodeValueShreddedHorizontal(this IEnumerable<Bitmap> images,AppSetting appSetting)
        {
            var resultList = new List<IBarcodeItem>(images.Count());
            var index = 0;
            foreach (var shreddeddImage in images.OfType<Bitmap>())
            {
                index += 1;
                var result = BarcodeReader.ReadFromBitmap(shreddeddImage);
                resultList.Add(result);
            }
            var records = resultList.Select((t,i)=>new { item=t,index=i }).Where(t => t.item.IsSuccessRead).Select(t =>
            {
                t.item.TryGetResultValue(out var resultValue);
                return new { item = resultValue, index = t.index };
            }).Where(t=>t.item is not null);

            var successList=appSetting.RegularExpressionFilter is (null or "") ?
                records : records.Where(t=> System.Text.RegularExpressions.Regex.IsMatch(t.item.Value, appSetting.RegularExpressionFilter));

            if (!successList.Any())
            {
                return BarcodeParameter.FromUnableRead();
            }
            
            var key = successList.Select(t => t.item.Value).
                GroupBy(t => t).
                Aggregate((a, b) => a.Count() > b.Count() ? a : b).
                Key;

            // 千切りイメージから位置を取得
            var rectItems = successList.Where(t => t is not null && t.item.Value == key);
            var minx = rectItems.Min(t => t.item.Rect.X);
            var maxx = rectItems.Select(t => t.item.Rect.Width + t.item.Rect.X).Max();
            var miny = rectItems.Min(t => t.index) * appSetting.ShreddedRate - appSetting.ShreddedRate * 2;
            var maxy = rectItems.Max(t => t.index) * appSetting.ShreddedRate;

            var resultRect= new Rectangle(minx, (maxy - miny) / 2 + miny, maxx - minx, maxx - minx);

            var resultParameter= BarcodeParameter.FromSuccess(key,resultRect,true);
            return resultParameter;
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
