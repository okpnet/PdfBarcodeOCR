using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToImage
{
    public interface IPdf
    {
        DpiType Dpi { get; set; }

        int ThumbnailRatio { get; }

        int NumberOfPage { get; }

        IEnumerable<IPdfPage> Pages { get; }

        IPdfPage? this[int pageIndex] { get; }

        Task<Image?> GetImageAsync(int page = 0);

        IEnumerable<IPageImage> AllPageImages();
    }
}
