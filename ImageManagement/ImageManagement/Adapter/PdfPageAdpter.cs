using PdfConverer.PdfProcessing;

namespace ImageManagement.Adapter
{
    public class PdfPageAdpter : IPageItem,IDisposable
    {
        protected readonly IParentPdfFileItem _parent;
        /// <summary>
        /// PDFのアイテム
        /// </summary>
        public IPdfPage? PdfPages { get; protected set; }

        public bool IsBusy { get; protected set; }

        public PdfPageAdpter(IParentPdfFileItem parent, IPdfPage pdfPage)
        {
            _parent = parent;
            PdfPages = pdfPage;
        }

        public void Dispose()
        {
            if(IPd)
        }
    }
}
