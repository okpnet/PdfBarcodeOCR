using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Adapter
{
    public interface IParentPdfFileItem
    {
        int ThumbnailSide { get; }
        /// <summary>
        /// PDFのページをコレクション
        /// </summary>
        IEnumerable<PdfPageAdpter> PdfFileItems { get; }
    }
}
