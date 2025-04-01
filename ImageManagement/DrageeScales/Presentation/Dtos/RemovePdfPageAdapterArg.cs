using DrageeScales.Views.Dtos;

namespace DrageeScales.Presentation.Dtos
{
    public class RemovePdfPageAdapterArg : ImageMangementArgBase
    {
        public PdfPageAdpter PdfPageAdpter { get; }

        public RemovePdfPageAdapterArg(PdfPageAdpter pdfPageAdpter)
        {
            PdfPageAdpter = pdfPageAdpter;
        }
    }
}
