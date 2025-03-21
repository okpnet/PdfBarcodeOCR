using System.Drawing;

namespace PdfConverer.PdfProcessing
{
    public interface IPdf
    {
        DpiType Dpi { get; set; }

        int ThumbnailRatio { get; }

        int NumberOfPage { get; }

        IEnumerable<IPdfPage> Pages { get; }

        IPdfPage? this[int pageIndex] { get; }

        Task<Image?> GetImageAsync(int page = 0);

        void Remove(IPdfPage page);
    }
}
