using System.Drawing;

namespace PdfToImage
{
    public interface IPdfPage:IDisposable
    {
        int PageNumber { get; }

        SizeF Size { get; }

        Image? Thumbnail { get; set; }

        void Redraw();

        Image? GetImage();
    }
}
