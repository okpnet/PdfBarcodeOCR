using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Adapter
{
    public class ImageAdapter:IDisposable
    {
        private string _imageFilepath;
        private Image _thumbnailImage;
        private Exception? _lastException;

        public bool HasException=> _lastException is not null;

        public Image Thumbnail
        {
            get=> _thumbnailImage;
            set=>_thumbnailImage = value;
        }

        private ImageAdapter(string imageFilepath, Image thumbnailImage, Exception? lastException)
        {
            _imageFilepath = imageFilepath;
            _thumbnailImage = thumbnailImage;
            _lastException = lastException;
        }

        public static ImageAdapter FromPath(string imagePath,int thumbnailRatio)
        {
            try
            {
                Image thumbNail = default!;
                if (!System.IO.File.Exists(imagePath))
                {
                    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(ImageManagement)}.noimage.bmp");
                    if (stream is null)
                    {
                        throw new NullReferenceException("resource is null");
                    }
                    thumbNail = new Bitmap(stream);

                }
                else
                {
                    thumbNail = new Bitmap(imagePath);
                }
                return new ImageAdapter(imagePath, thumbNail, null);
            }
            catch (Exception ex)
            {
                return new ImageAdapter(imagePath,default!, ex);
            }
        }

        public void SetThumbnil(int ratio)
        {

        }

        public void SetThumbnil(Rectangle rectangle,int ratio)
        {

        }

        private Image CreateDefault()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(ImageManagement)}.noimage.bmp");
            if(stream is null)
            {
                throw new NullReferenceException("resource is null");
            }
            return new Bitmap(stream);
        }

    }
}
