using ImageManagement.Adapter;
using ImageManagement.Base;
using PdfConverer.PdfProcessing;
using System.Collections.ObjectModel;

namespace ImageManagement.Collection
{
    /// <summary>
    /// バインドするPDFページのコレクション
    /// </summary>
    public class PdfFileItemCollection : ObservableCollection<IPageItem>, IParentPdfFileItem, IPdfFileItemCollection, IDisposable, IProcessingModel
    {
        private IList<IPdf> _pdfList = new List<IPdf>();
        /// <summary>
        /// サムネイルの一辺の最大長さ
        /// </summary>
        public int ThumbnailSide { get; set; }
        /// <summary>
        /// 一時フォルダ
        /// </summary>
        public string TmpDir { get; protected set; } = Path.GetTempPath();
        /// <summary>
        /// PDFのページをコレクション
        /// </summary>
        public IEnumerable<IPageItem> PdfFileItems => this;

        public bool IsBusy { get; set; }

        public async Task AddItemAsync(string filePath, IProgress<(int, int)>? progress = null)
        {
            if (!File.Exists(filePath)) return;
            try{
                IsBusy = true;
                var tmpPath = Path.Combine(TmpDir, Guid.NewGuid().ToString());
                var item = new PdfItem(filePath, tmpPath);
                _pdfList.Add(item);
                await item.InitilizePageAsync();
                foreach (var page in item.Pages)
                {
                    var addItem = new PdfPageAdpter(this, page);
                    Add(addItem);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task AddRangeAsyn(IEnumerable<string> filePaths, IProgress<(int, int)>? progress = null)
        {
            var tasks = new List<Task>();
            foreach (var filePath in filePaths)
            {
                tasks.Add(AddItemAsync(filePath, progress));
            }
            await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            foreach (var item in _pdfList.OfType<IDisposable>())
            {
                item.Dispose();
            }
        }
    }
}
