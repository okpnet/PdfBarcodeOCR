using System.Drawing;
using System.Drawing.Imaging;

namespace ImageShredded
{
    public class BmpShredded
    {
        public IEnumerable<Task<Bitmap?>>  GetHorizotalShreddedAsync<TResult>(Bitmap image, int height)
        {
            if (height > image.Height)
            {
                yield break;
            }
            var loopN = image.Height / height + (image.Height == height ? 0 : 1);
            
            var tasks = new List<Task<TResult>>(loopN);
            for (var index = 0; loopN >= index; index++)
            {
                yield return Task.Run(() => GetTrimHorizontal(image, index * height, height));
            }
        }

        public IEnumerable<Image> GetHorizotalShredded(Bitmap image, int height)
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

        private Bitmap? GetTrimHorizontal(Bitmap image,int startPixY,int height)
        {
            if(startPixY >= image.Height)
            {
                return null;
            }
            var trimHeight=startPixY + height > image.Height ? image.Height - startPixY:height;
            return GetTrimBitmap(image, 0, startPixY, image.Width, trimHeight);
        }

        private Bitmap? GetTrimBitmap(Bitmap image,int pixX,int pixY,int width,int heigh)
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
