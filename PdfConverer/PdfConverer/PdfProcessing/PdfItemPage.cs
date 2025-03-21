using PdfConverer.ImageProcessing;
using PdfiumViewer;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using Image = System.Drawing.Image;

namespace PdfConverer.PdfProcessing
{
    /// <summary>
    /// PDFのページイメージ
    /// PDFのビットマップイメージを生成する
    /// </summary>
    public class PdfItemPage : IPdfPage, IPdfPageSave, IDisposable
    {
        /// <summary>
        /// 従属するPDF
        /// </summary>
        public IPdf Parent { get; }
        /// <summary>
        /// PDFのページ番号
        /// </summary>
        [Range(typeof(int), "-1", "1000")]
        public int PageNumber { get; }
        /// <summary>
        /// ページのサイズ
        /// </summary>
        public SizeF Size { get; }
        /// <summary>
        /// イメージ
        /// </summary>
        public IImageDecorator Decorator { get; protected set; }

        public PdfItemPage(PdfItem paretn, PdfDocument document, int page, string parentPdfItemPath)
        {
            if (!File.Exists(parentPdfItemPath))
            {
                throw new NullReferenceException($"Parent file is nothing.=> {parentPdfItemPath}");
            }
            Parent = paretn;
            PageNumber = document.PageCount > page && page >= 0 ? page : -1;
            if (PageNumber != -1)
            {
                Size = document.PageSizes[PageNumber];
            }
            var dirPath = Path.GetDirectoryName(parentPdfItemPath);
            var filename = Path.GetFileNameWithoutExtension(parentPdfItemPath);
            var imagePath = Path.Combine(dirPath!, filename, $"_{PageNumber}");
            using var image = document.Render(PageNumber, (int)Parent.Dpi, (int)Parent.Dpi, PdfRenderFlags.CorrectFromDpi);
            image.Save(imagePath, ImageFormat.Bmp);
            Decorator = ImageDecorator.Create(imagePath, PdfConverterDfine.THUMBNAIL_MAX_SIDLEN);
        }
        /// <summary>
        /// 再描画。DPIやサムネイルのレートを変えたときに、一次保存したPDFページイメージを作り直す
        /// </summary>
        public void Redraw()
        {
            if (Parent is not PdfItem pdfitem || pdfitem.Document is null)
            {
                return;
            }
            var imagepath = Decorator.ImageFilePath;
            using var image = pdfitem.Document.Render(PageNumber, (int)Parent.Dpi, (int)Parent.Dpi, PdfRenderFlags.CorrectFromDpi);
            image.Save(imagepath, ImageFormat.Bmp);
            if(Decorator is IDisposable disposable)
            {
                disposable.Dispose();
            }
            Decorator = ImageDecorator.Create(imagepath, PdfConverterDfine.THUMBNAIL_MAX_SIDLEN);
        }

        public bool SaveImage(string filepath, ImageFormat format)
        {
            using var image = Decorator.GetImage();
            image.Save(filepath, format);
            return true;
        }

        public void Dispose()
        {
            if(Decorator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
