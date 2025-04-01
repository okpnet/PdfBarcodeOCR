using DrageeScales.Views.Dtos;

namespace DrageeScales.Presentation.Dtos
{
    public sealed class SaveToPdfromImagesArg : SaveToPdfromAllImagesArg
    {
        public PdfPageAdpter PdfPageAdpter { get; }

        public SaveToPdfromImagesArg(PdfPageAdpter pdfPageAdpter, string outDir) : base(outDir)
        {
            PdfPageAdpter = pdfPageAdpter;
        }
    }
}
