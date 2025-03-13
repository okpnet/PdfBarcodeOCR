using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToImage
{
    public class PdfItemPage
    {
        public PdfItem Parent { get; }
        [Range(typeof(int),"-1","1000")]
        public int Page { get; }

        public SizeF Size { get; }

        public PdfItemPage(PdfItem paretn,PdfDocument document,int page)
        {
            Parent = paretn;
            Page = document.PageCount>page && page>=0?page : -1;
            if (Page != -1)
            {
                Size = document.PageSizes[Page];
            }
        }
        
        public async Task<Image?> GetImageAsync()
        {
            if(Parent.Document is null && 0 > Page)
            {
                return null;
            }
            return await Task.Run(() =>
            {
                return Parent.Document!.Render(Page, Size.Width, Size.Height, PdfRenderFlags.CorrectFromDpi);
            });
        }

        public async Task SaveImageAsync(string filepath,ImageFormat format)
        {
            if (Parent.Document is null && 0 > Page)
            {
                return;
            }
            await Task.Run(() =>
            {
                var image = Parent.Document!.Render(Page, Size.Width, Size.Height, PdfRenderFlags.CorrectFromDpi);
                image.Save(filepath, format);
            });
        }
    }
}
