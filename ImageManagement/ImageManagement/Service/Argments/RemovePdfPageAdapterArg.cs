using ImageManagement.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Service.Argments
{
    public class RemovePdfPageAdapterArg: ImageMangementArgBase
    {
        public PdfPageAdpter PdfPageAdpter { get; }

        public RemovePdfPageAdapterArg(PdfPageAdpter pdfPageAdpter)
        {
            PdfPageAdpter = pdfPageAdpter;
        }
    }
}
