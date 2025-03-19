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
        public static async Task<string> GetBarcodeValue(string imagePath)
        {
            if (!imagePath.IsFileExists())
            {
                return string.Empty;
            }
            try
            {

                using var baseImage = new Bitmap(imagePath);
                var firstResult=await Task.Run(()=>BarcodeImageReader.BarcodeReader.ReadFromBitmap(baseImage));
                if(firstResult is not null)
                {
                    return firstResult.Value;
                }
                var shreddedImages = ImageShredded.BmpShredded.GetHorizotalShredded(baseImage, 5);
                var result = new List<IBarcodeItem>();
                foreach (var secImge in shreddedImages)
                {

                }
            }
            catch
            {
                return string.Empty;
            }
        }
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
                IEnumerable<Image> images = [];
                var strechWidth = (int)(bitmap.Width * ImageManagementDefine.STRECH_WIDTH);
                try
                {
                    images = ImageShredded.BmpShredded.GetHorizotalShredded(bitmap, ImageManagementDefine.SHREDDED_HEIGHT);
                    var shreddedTryResult = await images.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(ImageManagementDefine.SHREDDED_HEIGHT);
                    if (shreddedTryResult is not null)
                    {
                        return shreddedTryResult;
                    }
                    var strechImagesResult = images.Select(t =>
                    {
                        using var strechImage = new Bitmap(strechWidth, ImageManagementDefine.SHREDDED_HEIGHT);
                        using var grph = Graphics.FromImage(strechImage);
                        grph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        grph.DrawImage(strechImage, new Rectangle(0, 0, strechWidth, ImageManagementDefine.SHREDDED_HEIGHT));
                        return strechImage;
                    }).GetBarcodeValueShreddedHorizontal(ImageManagementDefine.SHREDDED_HEIGHT);
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
        internal static async Task<BarcodeResult?> GetBarcodeValueShreddedHorizontal(this IEnumerable<Bitmap> images, int shreddedHeight)
        {
            return await Task.Run(() =>
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
                    GetHorizontalShreddedRect(resultList,key, shreddedHeight)
                    );
            });
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
