using PdfConverer.Helper;
using System.Drawing;
using System.Reflection;

namespace PdfConverer.ImageProcessing
{
    public class ImageDecorator: IImageDecorator,IDisposable
    {
        internal static readonly ReaderWriterLockSlim imageLock = new ReaderWriterLockSlim();
        private string _imagePath;
        private Image _thumbnail;

        public Image Thumbnail => _thumbnail;

        public Exception? Exceptions { get;private set; }

        public string ImageFilePath => _imagePath;

        private ImageDecorator(string imagePath, Image thumbnail, Exception? exceptions)
        {
            _imagePath = imagePath;
            _thumbnail = thumbnail;
            Exceptions = exceptions;
        }

        public void SetTumbnail(Rectangle trimmingRectangle,int maxsidelength)
        {
            if(_thumbnail is not null)
            {
                _thumbnail.Dispose();
            }
            try
            {
                imageLock.EnterReadLock();
                using var image = GetImage(_imagePath);
                Image thumbnail = new Bitmap(trimmingRectangle.Width,trimmingRectangle.Height);
                using var g = Graphics.FromImage(thumbnail);
                g.DrawImage(image, trimmingRectangle);
                _thumbnail = thumbnail.CreateThumbnail(maxsidelength);
            }
            finally
            {
                imageLock?.ExitReadLock();
            }
        }

        public Image GetImage()=>GetImage(_imagePath);

        public static ImageDecorator Create(string bitmapPath,int maxsidelength)
        {
            try
            {
                Image bitmap = GetImage(bitmapPath!);
                var thumbnail = bitmap.CreateThumbnail(maxsidelength);
                return new ImageDecorator(bitmapPath, thumbnail, null);
            }
            catch (Exception ex)
            {
                return new ImageDecorator(string.Empty,default!,ex);
            }
        }

        private static Bitmap GetImage(string path)
        {
            try
            {
                imageLock.EnterReadLock();
                if (System.IO.File.Exists(path))
                {
                    return new Bitmap(path!);
                }
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(PdfConverer)}.Asset.noimage.bmp");
                return new Bitmap(stream!);
            }
            finally
            {
                imageLock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            if(_thumbnail is not null)
            {
                _thumbnail.Dispose(); 
            }
            try{
                imageLock.EnterWriteLock();
                System.IO.File.Delete(_imagePath);
            }finally 
            {
                imageLock.ExitWriteLock(); 
            }
        }
    }
}
