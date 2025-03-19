using BarcodeImageReader;
using ImageManagement.Adapter;
using PdfToImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageManagement.Extenstion
{
    public static class ImageExtension
    {
        /// <summary>
        /// イメージからバーコード読み込み
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static async Task<BarcodeResult?> GetBarcodeResult(this Bitmap bitmap)
        {
            var result = await Task.Run(() =>
            {
                var result = BarcodeReader.ReadFromBitmap(bitmap);
                return result is null ? null : new BarcodeResult(result.Value);
            });

            if(result is not null)
            {
                return result;
            }
            else
            {
                Image[] images = ImageShredded.BmpShredded.GetHorizotalShredded(bitmap, ImageManagementDefine.SHREDDED_HEIGHT).ToArray();
                var strechWidth = (int)(bitmap.Width * ImageManagementDefine.STRECH_WIDTH);
                try
                {
                    //2回目。千切りにして読みやすくする
                    var result2 =await Task.Run(() =>
                    {
                        var shreddedTryResult = images.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(ImageManagementDefine.SHREDDED_HEIGHT);
                        if (shreddedTryResult is null)
                        {
                            return null;
                        }
                        return shreddedTryResult;
                    });

                    if(result2 is not null)
                    {
                        return result2;
                    }

                    var result3 = await Task.Run(() =>
                    {
                        //3回目。千切りを引き延ばして空白スペースを確保する
                        var strechImages = images.Select(t =>
                        {
                            var strechImage = new Bitmap(strechWidth, ImageManagementDefine.SHREDDED_HEIGHT);
                            using var grph = Graphics.FromImage(strechImage);
                            grph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            grph.DrawImage(strechImage, new Rectangle(0, 0, strechWidth, ImageManagementDefine.SHREDDED_HEIGHT));
                            return strechImage;
                        }).ToArray();
                        var strechResult = strechImages.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(ImageManagementDefine.SHREDDED_HEIGHT);
                        foreach(var strechImage in strechImages)
                        {
                            strechImage.Dispose();
                        }
                        return strechResult;
                    });

                    return result3;//Nullなら読み込めていない
                    
                }
                finally
                {
                    foreach (var image in images)
                    {
                        image.Dispose();
                    }
                }
            }
        }


        /// <summary>
        /// 千切りしたイメージからバーコード読み込み
        /// </summary>
        /// <param name="images"></param>
        /// <param name="shreddedpixel"></param>
        /// <returns></returns>
        internal static BarcodeResult? GetBarcodeValueShreddedHorizontal(this IEnumerable<Bitmap> images, int shreddedHeight)
        {
            var resultList = new List<IBarcodeItem?>(images.Count());
            var index = 0;
            foreach (var shreddeddImage in images.OfType<Bitmap>())
            {
                index += 1;
                var result = BarcodeImageReader.BarcodeReader.ReadFromBitmap(shreddeddImage);
                resultList.Add(result);
            }
            if (resultList.Count == 0)
            {
                return null;
            }
            var key = resultList.GroupBy(t => t.Value).Aggregate((a, b) => a.Count() > b.Count() ? a : b).Key;
            return new BarcodeResult(
                key,
                GetHorizontalShreddedRect(resultList, key, shreddedHeight)
                );
        }

        /// <summary>
        /// 千切りイメージから位置を取得
        /// </summary>
        /// <param name="items"></param>
        /// <param name="valueKey"></param>
        /// <param name="shreddedHeight"></param>
        /// <returns></returns>
        internal static Rectangle GetHorizontalShreddedRect(IEnumerable<IBarcodeItem?> items,string valueKey,int shreddedHeight)
        {
            var records = items.Select((t, index) => new { index = index, item = t });
            var posX = items.Where(t => t is not null && t.Value == valueKey).Min(t => t!.Rect.X);
            var posY = records.Where(t => t.item is not null).Min(t => t.index) * shreddedHeight;
            var width = items.Where(t => t is not null && t.Value == valueKey).Select(t => t.Rect.Width + t.Rect.X).Max() - posX;
            var height = records.Where(t => t.item is not null).Max(t => t.index) * shreddedHeight - posY;
            return new Rectangle(posX, posY, width, height);
        }
        /// <summary>
        /// ファイル名
        /// </summary>
        /// <param name="pageItem"></param>
        /// <param name="outputDirictryName"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal static async Task<string> ChangeFileName(this IPageItem pageItem,string outputDirictryName,string filename)
        {
            if (pageItem.IsBusy)
            {
                var loopCounter = 0;
                while (pageItem.IsBusy)
                {
                    await Task.Delay(500);
                    if(loopCounter == 10)
                    {
                        return string.Empty;
                    }
                    loopCounter++;
                }
            }
            var newFilepath=System.IO.Path.Combine(outputDirictryName,filename);
            return newFilepath;
        }
    }
}
