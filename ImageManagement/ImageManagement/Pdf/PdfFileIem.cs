using PdfToImage;

namespace ImageManagement.Pdf
{
    public class PdfFileIem : IPageItem
    {
        protected readonly IParentPdfFileItem _parent;

        public bool IsInitialized { get; protected set; }
        /// <summary>
        /// PDFのアイテム
        /// </summary>
        public IPdfPage? PdfPages { get; protected set; }

        public bool IsBusy { get; protected set; }

        public PdfFileIem(IParentPdfFileItem parent, IPdfPage pdfPage)
        {
            IsInitialized = false;
            _parent = parent;
            PdfPages = pdfPage;
        }
    }
}
