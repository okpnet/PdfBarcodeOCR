using DrageeScales.Core;
using DrageeScales.Helper;
using DrageeScales.Shared.Helper;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using PdfConverer.Helper;
using PdfConverer.PdfProcessing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace DrageeScales.Views.Dtos
{
    public class PdfPageAdpter : NotifyPropertyChangedBase, IDisposable, INotifyPropertyChanged
    {
        protected readonly IParentPdfFileItem _parent;

        IPdfPage? _pdfPage;
        /// <summary>
        /// PDFのアイテム
        /// </summary>
        public IPdfPage? PdfPages
        {
            get => _pdfPage;
            protected set
            {
                if (Equals(_pdfPage, value))
                {
                    return;
                }
                _pdfPage = value;
                OnPropertyChanged(nameof(PdfPages));
                System.Diagnostics.Debug.WriteLine($"[UI] PdfPages = {_pdfPage?.PageNumber}");
            }
        }

        string _fileNameToSave = string.Empty;
        /// <summary>
        /// ファイル名
        /// 保存するときに使用
        /// </summary>
        public string FileNameToSave
        {
            get => _fileNameToSave;
            set
            {
                if (_fileNameToSave == value)
                {
                    return;
                }
                _fileNameToSave = value; //FileNameHelper.CreateNumberAppendToNewname(_parent.PdfFileItems.Select(t => t.FileNameToSave), value);
                _isBarcodeFail = false;
                OnPropertyChanged(nameof(FileNameToSave));
                OnPropertyChanged(nameof(IsBarcodeReadFail));
                System.Diagnostics.Debug.WriteLine($"[UI] FileNameToSave = {FileNameToSave}");
            }
        }

        bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            protected set
            {
                if (_isBusy == value)
                {
                    return;
                }
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                System.Diagnostics.Debug.WriteLine($"[UI] IsBusy = {IsBusy}");
            }
        }

        Image _thumbnail = new Bitmap(0, 0);
        public Image Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (_thumbnail == value)
                {
                    return;
                }
                _thumbnail = value;
                ThumbnailImageSource=_thumbnail.ConvertImage(ImageFormat.Png);
                OnPropertyChanged(nameof(Thumbnail));
                OnPropertyChanged(nameof(ThumbnailImageSource));
                System.Diagnostics.Debug.WriteLine($"[UI] Thumbnail = {Thumbnail}");
            }
        }

        public string BaseFile
        {
            get
            {
                if (PdfPages is null || PdfPages.Parent is not IPdfFile pdfFile)
                {
                    return string.Empty;
                }
                return System.IO.Path.GetFileNameWithoutExtension(pdfFile.FilePath);
            }
        }

        bool _isBarcodeFail = false;
        public bool IsBarcodeReadFail
        {
            get => _isBarcodeFail;
            set
            {
                if (_isBarcodeFail == value)
                {
                    return;
                }
                _isBarcodeFail = value;
                OnPropertyChanged(nameof(IsBarcodeReadFail));
                System.Diagnostics.Debug.WriteLine($"[UI] IsBarcodeReadFail = {IsBarcodeReadFail}");
            }
        }

        public ImageSource ThumbnailImageSource { get; protected set; }

        int _progressValue = 0;
        public int ProgressValue
        {
            get => _progressValue;
            protected set
            {
                if (_progressValue == value)
                {
                    return;
                }
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
                System.Diagnostics.Debug.WriteLine($"[UI] ProgressValue = {ProgressValue}");
            }
        }

        protected PdfPageAdpter(IParentPdfFileItem parent, IPdfPage pdfPage, Image thumbnail)
        {
            _parent = parent;
            _pdfPage = pdfPage;
            var saveFileName = _pdfPage.Parent is not IPdfFile file ? $"{DateTime.Now.ToString("F")}-{Guid.NewGuid()}" : System.IO.Path.GetFileNameWithoutExtension(file.FilePath);
            _fileNameToSave = $"{saveFileName}-{_pdfPage.PageNumber + 1}";
            _thumbnail = thumbnail;
        }
        /// <summary>
        /// ファクトリメソッド
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pdfPage"></param>
        /// <returns></returns>
        public static async Task<PdfPageAdpter> CreateAsync(IParentPdfFileItem parent, IPdfPage pdfPage)
        {
            var imageDec = await WinThumbnailHelper.ImageDecorator.CreateAsync(pdfPage.ImagePath, parent.ThumbnailSide);
            return new PdfPageAdpter(parent, pdfPage, imageDec.Thumbnail);
        }

        /// <summary>
        /// イメージのバーコードをファイル名変換
        /// </summary>
        /// <returns></returns>
        public async Task ReadBarcodeFromFileAsync()
        {
            try
            {
                
                IsBusy = true;
                if (_pdfPage is null)
                {
                    return;
                }

                var image = await _pdfPage.GetImageAsync();
                if (image is null)
                {
                    return;
                }

                var result = await image.GetBarcodeResult(
                    new Progress<int>(t=>
                    {
                        ProgressValue = t;
                        IsBusy = false;
                    }));
                if (result.IsSucces)
                {
                    FileNameToSave = result.Value;
                }
            }
            finally
            {
                IsBusy = false;
                ProgressValue = 0;
            }
        }
        /// <summary>
        /// PDFに変換して保存
        /// </summary>
        /// <param name="outputDir"></param>
        /// <returns></returns>
        public async Task SaveToPdfAsync(string outputDir)
        {
            try
            {
                IsBusy = true;
                if (PdfPages is null || !System.IO.Directory.Exists(outputDir))
                {
                    return;
                }
                var image = await PdfPages.GetImageAsync();
                if (image is null)
                {
                    return;
                }

                var files = System.IO.Directory.GetFiles(outputDir, "*.pdf");
                var saveFileName = FileNameHelper.CreateNumberAppendToNewname(
                    files.Select(System.IO.Path.GetFileNameWithoutExtension)!,
                    FileNameToSave);
                var saveFullpath = System.IO.Path.Combine(outputDir, $"{saveFileName}.pdf");
                await image.SaveFitImageToPdfAsync(saveFullpath);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Dispose()
        {
            if (PdfPages is IDisposable diposable)
            {
                diposable.Dispose();
            }
            if (_thumbnail is not null)
            {
                _thumbnail.Dispose();
            }
        }
    }
}
