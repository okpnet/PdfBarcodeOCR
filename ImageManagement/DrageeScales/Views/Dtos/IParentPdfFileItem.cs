using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Views.Dtos
{
    public interface IParentPdfFileItem
    {
        /// <summary>
        /// PDFのページをコレクション
        /// </summary>
        IEnumerable<PdfPageAdpter> PdfFileItems { get; }
    }
}
