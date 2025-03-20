using System.Drawing;

namespace PdfToImage.Extension
{
    public static class ImageHelper
    {
        internal static Size ChangeScaleRatio(this Image image, int maxSide)
        {
            var resultSize = image.Width > image.Height ?
                new Size(maxSide, (int)(image.Height / (float)maxSide)) : new Size((int)(image.Width / (float)maxSide), maxSide);
            return resultSize;
        }
        /// <summary>
        /// サムネイル生成
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ratio">スケールの縮尺率</param>
        /// <returns></returns>
        public static Image CreateThumbnail(this Image image, int ratio)
        {
            var size=image.ChangeScaleRatio(ratio);
            return image.GetThumbnailImage(size.Width,size.Height,null,IntPtr.Zero);
        }
    }
}
