using BarcodeImageReader;
using ImageManagement.Adapter;
using System.Drawing;

namespace ImageManagement.Helper
{
    public static class ImageBarcodeHelper
    {
        /// <summary>
        /// イメージからバーコード読み込み
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static async Task<BarcodeParameter> GetBarcodeResult(this Bitmap bitmap)
        {

            BarcodeParameter result = default!;
            result = await Task.Run(() =>
            {
                var result = BarcodeReader.ReadFromBitmap(bitmap);
                if (result.TryGetResultValue(out var barcode))
                {
                    return BarcodeParameter.FromSuccess(barcode.Value, barcode.Rect);
                }
                return BarcodeParameter.FromUnableRed();
            });

            if (result.IsSucces)
            {
                return result;
            }

            Image[] images = [];
            var strechWidth = (int)(bitmap.Width * ImageManagementDefine.STRECH_WIDTH);
            try
            {
                images = ImageShredded.BmpShredded.GetHorizotalShredded(bitmap, ImageManagementDefine.SHREDDED_HEIGHT).ToArray();
                //2回目。千切りにして読みやすくする
                result = await Task.Run(() => images.OfType<Bitmap>().GetBarcodeValueShreddedHorizontal(ImageManagementDefine.SHREDDED_HEIGHT));

                if (result.IsSucces)
                {
                    return result;
                }

                result = await Task.Run(() =>
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
                    foreach (var strechImage in strechImages)
                    {
                        strechImage.Dispose();
                    }

                    if (strechResult.IsSucces)
                    {
                        var prevRect = new Rectangle(
                                (int)(strechResult.Rectangles.X / ImageManagementDefine.STRECH_WIDTH),
                                strechResult.Rectangles.Y,
                                (int)(strechResult.Rectangles.Width / ImageManagementDefine.STRECH_WIDTH),
                                strechResult.Rectangles.Height
                            );
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
            if (resultList.Count == 0)
            {
                return null;
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
            var records = items.Where(t => t.IsSuccessRead).Select((t, index) =>
            {
                t.TryGetResultValue(out var resultValue);
                return new { index, item = resultValue };
            });
            var posX = records.Where(t => t is not null && t.item.Value == valueKey).Min(t => t.item.Rect.X);
            var posY = records.Where(t => t.item is not null).Min(t => t.index) * shreddedHeight;
            var width = records.Where(t => t is not null && t.item.Value == valueKey).Select(t => t.item.Rect.Width + t.item.Rect.X).Max() - posX;
            var height = records.Where(t => t is not null && t.item is not null).Max(t => t.index) * shreddedHeight - posY;
            return new Rectangle(posX, posY, width, height);
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
