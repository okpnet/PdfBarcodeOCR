using PdfToImage;
using System.Collections.ObjectModel;

namespace ImageManagement
{
    /// <summary>
    /// バインドするPDFページのコレクション
    /// </summary>
    public class PdfFileItemCollection:ObservableCollection<IPageItem>, IParentPdfFileItem, IPdfFileItemCollection,IDisposable,IProcessingModel
    {
        private IList<IPdf> _pdfList=new List<IPdf>();
        /// <summary>
        /// サムネイルの一辺の最大長さ
        /// </summary>
        public int ThumbnailSide { get; set; }
        /// <summary>
        /// 一時フォルダ
        /// </summary>
        public string TmpDir { get; protected set; } = System.IO.Path.GetTempPath();

        public IEnumerable<IPageItem> PdfFileItems => this;

        public bool IsBusy { get; set; }

        public async Task AddItem(string filePath,IProgress<(int,int)>? progress=null)
        {
            if (!System.IO.File.Exists(filePath)) return ;
            var tmpPath=System.IO.Path.Combine(TmpDir,Guid.NewGuid().ToString());
            var item=new PdfItem(filePath, tmpPath);
            _pdfList.Add(item);
            await item.InitilizePageAsync();
            foreach(var page in item.Pages)
            {
                var addItem = new PdfFileIem(this, page);
                Add(addItem);
            }
        }

        public async Task AddRangeAsyn(IEnumerable<string> filePaths, IProgress<(int, int)>? progress = null)
        {
            var tasks=new List<Task>();
            foreach (var filePath in filePaths)
            {
                tasks.Add(AddItem(filePath, progress));
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
