using System.Drawing;

namespace PdfToImage
{
    public interface IPdfPage
    {
        int PageNumber { get; }

        SizeF Size { get; }

        Task<Image?> GetImageAsync();
    }
}
