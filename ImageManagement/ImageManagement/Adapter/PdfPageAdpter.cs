using PdfConverer.PdfProcessing;
using System.ComponentModel;

namespace ImageManagement.Adapter
{
    public class PdfPageAdpter : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged=null;
        protected readonly IParentPdfFileItem _parent;
        bool _isBusy = false;
        string _fileNameToSave=string.Empty;
        IPdfPage? _pdfPage;
        /// <summary>
        /// PDFのアイテム
        /// </summary>
        public IPdfPage? PdfPages 
        {
            get=> _pdfPage;
            protected set
            {
                if ( Equals(_pdfPage,value))
                {
                    return;
                }
                _pdfPage = value;
                OnPropertyChanged(nameof(PdfPages));
                System.Diagnostics.Debug.WriteLine($"[UI] PdfPages = {_pdfPage?.PageNumber}");
            }
        }
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
                _fileNameToSave = value;
                OnPropertyChanged(nameof(FileNameToSave));
                System.Diagnostics.Debug.WriteLine($"[UI] FileNameToSave = {FileNameToSave}");
            }
        }

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


        public string? BaseFile 
        {
            get
            {
                if(PdfPages is null || PdfPages.Parent is not IPdfFile pdfFile)
                {
                    return string.Empty;
                }
                return System.IO.Path.GetFileNameWithoutExtension(pdfFile.FilePath);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PdfPageAdpter(IParentPdfFileItem parent, IPdfPage pdfPage)
        {
            _parent = parent;
            PdfPages = pdfPage;
            var saveFileName= PdfPages.Parent is not IPdfFile file ? $"{DateTime.Now.ToString("F")}-{Guid.NewGuid()}" : System.IO.Path.GetFileNameWithoutExtension(file.FilePath);
            FileNameToSave = $"{saveFileName}-{PdfPages.PageNumber + 1}.pdf";
        }

        public void Dispose()
        {
            if(PdfPages is IDisposable diposable)
            {
                diposable.Dispose();
            }
        }
    }
}
