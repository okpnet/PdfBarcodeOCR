using ImageManagement.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Service.Argments
{
    public sealed class SaveToPdfromImagesArg: SaveToPdfromAllImagesArg
    {
        public PdfPageAdpter PdfPageAdpter { get; }

        public SaveToPdfromImagesArg(PdfPageAdpter pdfPageAdpter, string outDir) :base(outDir)
        {
            PdfPageAdpter = pdfPageAdpter;
        }
    }
}
