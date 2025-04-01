using DrageeScales.Core;
using DrageeScales.Views.Dtos;
using PdfConverer.PdfProcessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DrageeScales.Presentation.Services
{
    /// <summary>
    /// バインドするPDFページのコレクション
    /// </summary>
    public class PdfFileItemCollection : ObservableCollection<PdfPageAdpter>, IParentPdfFileItem, IPdfFileItemCollection, IDisposable, IProcessingModel
    {
        private IList<IPdf> _pdfList = new List<IPdf>();

        public bool IsAny => this.Any();
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
        public IEnumerable<PdfPageAdpter> PdfFileItems => this;

        public bool IsBusy { get; set; }

        public async Task AddItemAsync(string filePath, IProgress<(int, int)>? progress = null)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                IsBusy = true;
                var tmpPath = Path.Combine(TmpDir, Guid.NewGuid().ToString());
                await AddItem(filePath, tmpPath);
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
                var tmpPath = Path.Combine(TmpDir, Guid.NewGuid().ToString());
                tasks.Add(AddItem(filePath, tmpPath));
            }
            try
            {
                IsBusy = true;
                await Task.WhenAll(tasks);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ForeachWhenall(Func<PdfPageAdpter, Task> funcWhenAll)
        {
            var tasks = new List<Task>();
            foreach (var pdfPage in this)
            {
                tasks.Add(funcWhenAll(pdfPage));
            }
            try
            {
                IsBusy = true;
                await Task.WhenAll(tasks);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddItem(string filePath, string tmpPath)
        {
            var item = new PdfItem(filePath, tmpPath);
            _pdfList.Add(item);
            await item.InitilizePageAsync();
            foreach (var page in item.Pages)
            {
                var addItem = await PdfPageAdpter.CreateAsync(this, page);
                Add(addItem);
            }
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
