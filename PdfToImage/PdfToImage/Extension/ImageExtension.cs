using System.Drawing;

namespace PdfToImage.Extension
{
    public static class ImageExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxSide">一辺の長さ</param>
        /// <returns></returns>
        public static Size ChangeScaleRatio(this Image image, int maxSide)
        {
            var resultSize = image.Width > image.Height ?
                new Size(maxSide, (int)(image.Height / (float)maxSide)) : new Size((int)(image.Width / (float)maxSide), maxSide);
            return resultSize;
        }

        public static Image CreateThumbnail(this Image image, int ratio)
        {
            var size=image.ChangeScaleRatio(ratio);
            return image.GetThumbnailImage(size.Width,size.Height,null,IntPtr.Zero);
        }
    }
}
