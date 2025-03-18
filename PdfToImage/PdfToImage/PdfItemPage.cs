using PdfiumViewer;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using Image = System.Drawing.Image;

namespace PdfToImage
{
    public class PdfItemPage:IPdfPage,IPdfPageSave
    {
        public PdfItem Parent { get; }

        [Range(typeof(int),"-1","1000")]
        public int PageNumber { get; }

        public SizeF Size { get; }

        public PdfItemPage(PdfItem paretn,PdfDocument document,int page)
        {
            Parent = paretn;
            PageNumber = document.PageCount>page && page>=0?page : -1;
            if (PageNumber != -1)
            {
                Size = document.PageSizes[PageNumber];
            }
        }
        
        public async Task<Image?> GetImageAsync()
        {
            if(Parent.Document is null || 0 > PageNumber)
            {
                return null;
            }
            using PdfDocument? pdfDocument = Parent.Document;
            return await Task.Run(() =>
            {
                var resultImage= Parent.Document.Render(PageNumber, (int)Parent.Dpi, (int)Parent.Dpi, PdfRenderFlags.CorrectFromDpi);
                Parent.Document.Dispose();
                return resultImage;
            });
        }

        public async Task SaveImageAsync(string filepath,ImageFormat format)
        {
            if (Parent.Document is null || 0 > PageNumber)
            {
                return;
            }

            try
            {
                using Image? image = await GetImageAsync(Parent.Document);
                image?.Save(filepath, format);
            }
            finally
            {
                Parent.Document.Dispose();
            }
        }

        internal async Task<Image?> GetImageAsync(PdfDocument pdfDocument)
        {
            if ( 0 > PageNumber)
            {
                return null;
            }
            return await Task.Run(() =>
            {
                return pdfDocument.Render(PageNumber, (int)Parent.Dpi, (int)Parent.Dpi, PdfRenderFlags.CorrectFromDpi);
            });
        }

        internal async Task SaveImageAsync(PdfDocument pdfDocument,string filepath, ImageFormat format)
        {
            if ( 0 > PageNumber)
            {
                return;
            }
            var image=await GetImageAsync(pdfDocument);
            image?.Save(filepath, format);
        }
    }
}
