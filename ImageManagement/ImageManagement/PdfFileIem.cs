using ImageManagement.Extension;
using PdfToImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageManagement
{
    public class PdfFileIem : IPdfFileItem,IDisposable
    {
        protected readonly IParentPdfFileItem _parent;

        public bool IsInitialized { get; protected set; } 

        public IPdf PdfItem { get; protected set; }

        public Image Thumbnail { get; protected set; }

        public PdfFileIem(IParentPdfFileItem parent, string pdfFilepath )
        {
            IsInitialized = false;
            _parent= parent;
            PdfItem= new PdfItem(pdfFilepath);
            Thumbnail = DefaultThumbnailImage();
        }

        public async Task InitAsync()
        {
            await ((PdfItem)PdfItem).InitPageItemAsync();
            if(Thumbnail is not null)
            {
                Thumbnail.Dispose();
            }
            Thumbnail=await CreateThumbnailAsync();
            IsInitialized = true;
        }

        public async Task<Image> CreateThumbnailAsync(int page=0)
        {
            if (!PdfItem.Pages.Any() || 0 > page || page >= PdfItem.Pages.Count())
            {
                return DefaultThumbnailImage();
            }
            var image = await PdfItem.Pages.Select((t, i) => new { index = i, item = t }).Where(t => t.index == page).First().item.GetImageAsync();
            if(image is null)
            {
                return DefaultThumbnailImage();
            }
            var size = image.ChangeScaleSide(_parent.ThumbnailSide);
            var thumbnailImage = image.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero);
            image.Dispose();
            return thumbnailImage;
        }

        public async Task<Image> CreateThumbnailAsync(Rectangle trimingRectangle, int page = 0)
        {
            if (!PdfItem.Pages.Any() || 0 > page || page >= PdfItem.Pages.Count())
            {
                return DefaultThumbnailImage();
            }
            var image = await PdfItem.Pages.Select((t, i) => new { index = i, item = t }).Where(t => t.index == page).First().item.GetImageAsync();
            if (image is null)
            {
                return DefaultThumbnailImage();
            }
            var thumbnailImage = ((Bitmap)image).Clone(trimingRectangle, PixelFormat.DontCare);
            image.Dispose();
            return thumbnailImage;
        }

        private Image DefaultThumbnailImage()
        {
            using  Image image= new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(ImageManagement)}.noimage.bmp")!);
            var size = image.ChangeScaleSide(_parent.ThumbnailSide);
            var thumbnailImage = image.GetThumbnailImage(size.Width, size.Height, null, IntPtr.Zero);
            image.Dispose();
            return thumbnailImage;
        }

        public void Dispose()
        {
            if(Thumbnail is not null)
            {
                Thumbnail.Dispose();
            }
        }
    }
}
