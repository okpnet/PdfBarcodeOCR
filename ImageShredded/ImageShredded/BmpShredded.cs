using System.Drawing;
using System.Drawing.Imaging;

namespace ImageShredded
{
    /// <summary>
    /// BitMapイメージの千切り
    /// </summary>
    public static class BmpShredded
    {
        /// <summary>
        /// 縦方向へ千切り
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width">縦に切る幅</param>
        /// <returns></returns>
        public static IEnumerable<Image> GetVirticalShredded(Bitmap image, int width)
        {
            if (width > image.Width)
            {
                yield break;
            }
            var loopN = image.Width / width + (image.Width == width ? 0 : 1);
            for (var index = 0; loopN >= index; index++)
            {
                var trim = GetTrimVirtical(image, index * width, width);
                if (trim is not null)
                {
                    yield return trim;
                }
            }
        }
        /// <summary>
        /// /横方向へ千切り
        /// </summary>
        /// <param name="image"></param>
        /// <param name="height">横に切る高さ</param>
        /// <returns></returns>
        public static IEnumerable<Image> GetHorizotalShredded(Bitmap image, int height)
        {
            if (height > image.Height)
            {
                yield break;
            }
            var loopN = image.Height / height + (image.Height == height ? 0 : 1);
            for (var index = 0; loopN >= index; index++)
            {
                var trim = GetTrimHorizontal(image, index * height, height);
                if (trim is not null)
                {
                    yield return trim;
                }
            }
        }
        /// <summary>
        /// 縦方向へ千切り
        /// </summary>
        /// <param name="image"></param>
        /// <param name="startPixY"></param>
        /// <param name="height">縦に切る幅</param>
        /// <returns></returns>
        internal static Bitmap? GetTrimHorizontal(Bitmap image, int startPixY, int height)
        {
            if (startPixY >= image.Height)
            {
                return null;
            }
            var trimHeight = startPixY + height > image.Height ? image.Height - startPixY : height;
            return GetTrimBitmap(image, 0, startPixY, image.Width, trimHeight);
        }
        /// <summary>
        /// 横方向へ千切り
        /// </summary>
        /// <param name="image"></param>
        /// <param name="startPixX"></param>
        /// <param name="width">横に切る高さ</param>
        /// <returns></returns>
        internal static Bitmap? GetTrimVirtical(Bitmap image,int startPixX,int width)
        {
            if (startPixX >= image.Width)
            {
                return null;
            }
            var trimWidth = startPixX + width > image.Width ? image.Width - startPixX : width;
            return GetTrimBitmap(image, startPixX, 0, trimWidth, image.Height);
        }
        /// <summary>
        /// BitMapを任意範囲に切り取り
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pixX"></param>
        /// <param name="pixY"></param>
        /// <param name="width"></param>
        /// <param name="heigh"></param>
        /// <returns></returns>
        internal static Bitmap? GetTrimBitmap(Bitmap image,int pixX,int pixY,int width,int heigh)
        {
            if( 0 > pixX || pixX > image.Width || pixX +width > image.Width ||
                0 > pixY || pixY >image.Height || pixY + heigh >image.Height)
            {
                return null;
            }
            var rect = new Rectangle(pixX, pixY, width, heigh);
            
            return image.Clone(rect,PixelFormat.DontCare);
        }
    }
}
