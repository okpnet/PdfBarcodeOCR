using PdfiumViewer;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

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
            if(Parent.Document is null && 0 > PageNumber)
            {
                return null;
            }
            return await Task.Run(() =>
            {
                return Parent.Document!.Render(PageNumber, Size.Width, Size.Height, PdfRenderFlags.CorrectFromDpi);
            });
        }

        public async Task SaveImageAsync(string filepath,ImageFormat format)
        {
            if (Parent.Document is null && 0 > PageNumber)
            {
                return;
            }
            await Task.Run(() =>
            {
                var image = Parent.Document!.Render(PageNumber, Size.Width, Size.Height, PdfRenderFlags.CorrectFromDpi);
                image.Save(filepath, format);
            });
        }
    }
}
