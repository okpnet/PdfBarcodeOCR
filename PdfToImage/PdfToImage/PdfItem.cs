﻿using PdfiumViewer;
using System.Drawing;

namespace PdfToImage
{
    public class PdfItem:IPdf,IPdfFile,IDisposable
    {
        private string? _filePath;
        private string? _baseFilePath;

        private IList<IPdfPage> _pdfItems=[];

        public IEnumerable<IPdfPage> Pages => _pdfItems;

        public string? FilePath => _baseFilePath;

        public DpiType Dpi { get; set; } = DpiType.DPI300;

        public IPdfPage? this[int pageIndex]
        {
            get
            {
                if(Document is null || 0 > pageIndex || pageIndex > Document.PageCount)
                {
                    return null;
                }
                return _pdfItems[pageIndex];
            }
        }

        internal PdfDocument? Document { get; }

        public PdfItem(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    return;
                }
                _filePath = System.IO.Path.GetTempFileName();
                System.IO.File.Copy(filePath, _filePath, true);
                Document = PdfDocument.Load(_filePath);
            }
            finally
            {
                if (Document is null)
                {
                    if (System.IO.File.Exists(_filePath))
                    {
                        System.IO.File.Delete(_filePath);
                    }
                    _filePath = null;
                    _baseFilePath = null;
                }
                else
                {
                    _baseFilePath = filePath;
                }
            }
        }

        public async Task<int> InitPageItemAsync()
        {
            if (Document is null)
            {
                return -1;
            }
            return await Task.Run(() =>
            {
                for (var pageIndex = 0; Document.PageCount > pageIndex; pageIndex++)
                {
                    var sizeF = Document.PageSizes[pageIndex];
                    _pdfItems.Add(new PdfItemPage(this,Document,pageIndex));
                }
                return Document.PageCount;
            });
        }

        public IEnumerable<Image> FromFile(string filename)
        {
            
            var pdfDoc=PdfDocument.Load(filename);
            if(pdfDoc is null)
            {
                return [];
            }
            var images = new Image[pdfDoc.PageCount];
            for (var pageIndex=0;pdfDoc.PageCount> pageIndex; pageIndex++)
            {
                var sizeF=pdfDoc.PageSizes[pageIndex];
                images[pageIndex] = pdfDoc.Render(pageIndex, sizeF.Width, sizeF.Height, PdfRenderFlags.CorrectFromDpi);
            }
            return images;
        }
        public void Dispose()
        {
            Document?.Dispose();
            if(System.IO.File.Exists(_filePath))
            {
                System.IO.File.Delete(_filePath);
            }
        }
    }
}
