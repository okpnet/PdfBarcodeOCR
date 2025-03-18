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

        int NumberOfPage { get; }

        IEnumerable<IPdfPage> Pages { get; }

        IPdfPage? this[int pageIndex] { get; }

        IEnumerable<Image> AllPageImages();
    }
}
