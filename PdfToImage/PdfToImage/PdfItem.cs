﻿using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PdfToImage
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfItem:IPdf,IPdfFile,IDisposable
    {
        const int THUMB_RATIO = 168;
        /// <summary>
        /// 初期化フラグ
        /// </summary>
        bool isInit = false;
        /// <summary>
        /// コピーしたPDFファイルのパス
        /// </summary>
        private string? _filePath;
        /// <summary>
        /// コピー元のPDFファイルのパス
        /// </summary>
        private string? _baseFilePath;
        /// <summary>
        /// ページアイテム
        /// </summary>
        private IList<IPdfPage> _pdfItems=[];
        /// <summary>
        /// ページアイテム
        /// </summary>
        public IEnumerable<IPdfPage> Pages => _pdfItems;
        /// <summary>
        /// 元のファイルパス
        /// </summary>
        public string? FilePath => _baseFilePath;
        /// <summary>
        /// PDFドキュメントのページ数
        /// </summary>
        public int NumberOfPage { get; protected set; } = -1;
        /// <summary>
        /// 出力解像度
        /// </summary>
        public DpiType Dpi { get; set; } = DpiType.DPI300;
        /// <summary>
        /// 
        /// </summary>
        public int ThumbnailRatio { get; set; } = THUMB_RATIO;
        /// <summary>
        /// ページにアクセスするためのインデクサ
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IPdfPage? this[int pageIndex]
        {
            get
            {
                if( 0 > pageIndex || pageIndex > NumberOfPage)
                {
                    return null;
                }
                return _pdfItems[pageIndex];
            }
        }

        internal PdfDocument? Document 
        {
            get
            {
                return PdfDocument.Load(_filePath);
            }
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filePath"></param>
        public PdfItem(string filePath)
        {
            _filePath = System.IO.Path.GetTempFileName();
            System.IO.File.Copy(filePath, _filePath, true);
        }

        public PdfItem(string filePath,string tmpPath)
        {
            _filePath = tmpPath;
            System.IO.File.Copy(filePath, _filePath, true);
        }

        public async Task InitilizePageAsync(IProgress<(int,int)>? progress=null)
        {
            if (isInit)
            {
                return;
            }
            using var pdfDoc = PdfDocument.Load(_filePath);
            if (pdfDoc is not null)
            {
                NumberOfPage = pdfDoc.PageCount;
                
                await Task.Run(() =>
                {
                    for (var pageIndex = 0; NumberOfPage > pageIndex; pageIndex++)
                    {
                        progress?.Report(new(pageIndex, NumberOfPage));
                        _pdfItems.Add(new PdfItemPage(this, pdfDoc, pageIndex, _filePath!));
                    }
                });
            }
            else
            {
                System.IO.File.Delete(_filePath!);
                _filePath = null;
                _baseFilePath = null;
            }
        }

        /// <summary>
        /// 指定ページのイメージを取得
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>

        public async Task<Image?> GetImageAsync(int page=0)
        {
            if(page>=NumberOfPage ||  0 > page)
            {
                return null;
            }
            using var pdfDoc = PdfDocument.Load(_filePath);
            if (this[page] is PdfItemPage pdfPage)
            {
                return await Task.Run(()=>pdfPage.GetImage());
            }
            return null;
        }
        /// <summary>
        /// 指定ページをイメージで保存
        /// </summary>
        /// <param name="page"></param>
        /// <param name="filepath"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public async Task SaveImageAsync(int page, string filepath, ImageFormat format)
        {
            using var pdfDoc = PdfDocument.Load(_filePath);
            if (this[page] is PdfItemPage pdfPage)
            {
                await Task.Run(() => pdfPage.SaveImage(pdfDoc, filepath, format));
            }
        }

        /// <summary>
        /// すべてのページのイメージを取得
        /// 全ページをイメージとして保存するとき、ファイル名のパターンが限定されるためImageインスタンスを返す
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPageImage> AllPageImages()
        {
            var pdfDoc=PdfDocument.Load(_filePath);
            if(pdfDoc is null)
            {
                yield break;
            }
            foreach(var page in Pages.OfType<PdfItemPage>())
            {
                var image=page.GetImage();
                if (image is not null) yield return new PageImage(page,image);
            }
        }

        public void Dispose()
        {
            if(System.IO.File.Exists(_filePath))
            {
                System.IO.File.Delete(_filePath);
            }
            foreach(var page in Pages)
            {
                page.Dispose();
            }
        }
    }
}
