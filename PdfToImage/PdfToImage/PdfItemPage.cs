using PdfiumViewer;
using PdfToImage.Extension;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using Image = System.Drawing.Image;

namespace PdfToImage
{
    /// <summary>
    /// PDFのページイメージ
    /// PDFのビットマップイメージを生成する
    /// </summary>
    public class PdfItemPage:IPdfPage,IPdfPageSave,IDisposable
    {
        /// <summary>
        /// ページのビットマップイメージファイル
        /// </summary>
        private readonly string _imagePath;
        /// <summary>
        /// 従属するPDF
        /// </summary>
        public IPdf Parent { get; }
        /// <summary>
        /// PDFのページ番号
        /// </summary>
        [Range(typeof(int),"-1","1000")]
        public int PageNumber { get; }
        /// <summary>
        /// ページのサイズ
        /// </summary>
        public SizeF Size { get; }
        /// <summary>
        /// サムネイル
        /// </summary>
        public Image? Thumbnail { get; set; }

        public PdfItemPage(PdfItem paretn,PdfDocument document,int page,string parentPdfItemPath)
        {
            if (!System.IO.File.Exists(parentPdfItemPath))
            {
                throw new NullReferenceException($"Parent file is nothing.=> {parentPdfItemPath}");
            }
            Parent = paretn;
            PageNumber = document.PageCount>page && page>=0?page : -1;
            if (PageNumber != -1)
            {
                Size = document.PageSizes[PageNumber];
            }
            var dirPath = System.IO.Path.GetDirectoryName(parentPdfItemPath);
            var filename = System.IO.Path.GetFileNameWithoutExtension(parentPdfItemPath);
            var imagePath=System.IO.Path.Combine(dirPath!, filename,$"_{PageNumber}");
            _imagePath = imagePath;
            using var image = document.Render(PageNumber, (int)Parent.Dpi, (int)Parent.Dpi, PdfRenderFlags.CorrectFromDpi);
            image.Save(_imagePath, ImageFormat.Bmp);
            Thumbnail =image.CreateThumbnail(Parent.ThumbnailRatio);
        }
        /// <summary>
        /// 再描画。DPIやサムネイルのレートを変えたときに、一次保存したPDFページイメージを作り直す
        /// </summary>
        public void Redraw()
        {
            if(Parent is not PdfItem pdfitem || pdfitem.Document is null)
            {
                return;
            }
            using var image = pdfitem.Document.Render(PageNumber, (int)Parent.Dpi, (int)Parent.Dpi, PdfRenderFlags.CorrectFromDpi);
            image.Save(_imagePath, ImageFormat.Bmp);
            Thumbnail?.Dispose();
            Thumbnail = image.CreateThumbnail(Parent.ThumbnailRatio);
        }

        public Image? GetImage()
        {
            if (!System.IO.File.Exists(_imagePath))
            {
                return null;
            }
            var image = new Bitmap(_imagePath);
            return image;
        }

        public bool SaveImage(string filepath, ImageFormat format)
        {
            if (!System.IO.Path.IsPathFullyQualified(filepath) || !System.IO.File.Exists(_imagePath))
            {
                return false;
            }
            
            using var image = new Bitmap(_imagePath); ;
            image.Save(filepath, format);
            return true;
        }

        internal void SaveImage(PdfDocument pdfDocument,string filepath, ImageFormat format)
        {
            if ( 0 > PageNumber)
            {
                return;
            }
            if (!System.IO.Path.IsPathFullyQualified(filepath))
            {
                return;
            }
            var image= pdfDocument.Render(PageNumber, (int)Parent.Dpi, (int)Parent.Dpi, PdfRenderFlags.CorrectFromDpi);
            image?.Save(filepath, format);
        }

        public void Dispose()
        {
            if (System.IO.File.Exists(_imagePath))
            {
                System.IO.File.Delete(_imagePath);
            }
            if(Thumbnail is not null)
            {
                Thumbnail.Dispose();
            }
        }
    }
}
