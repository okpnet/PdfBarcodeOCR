using DrageeScales.Core;
using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;
using DrageeScales.Views.Dtos;
using Microsoft.Extensions.Logging;
using PdfConverer.PdfProcessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrageeScales.Presentation.Services
{
    /// <summary>
    /// バインドするPDFページのコレクション
    /// </summary>
    public class PdfFileItemCollection : ObservableCollection<PdfPageAdpter>, IParentPdfFileItem, IPdfFileItemCollection, IDisposable, IProcessingModel
    {
        private IList<IPdf> _pdfList = new List<IPdf>();
        private ReaderWriterLockSlim _lock = new();
        readonly ILogger _logger;
        readonly IConfigService<AppSetting> _config;
        public bool IsAny => this.Any();
        /// <summary>
        /// 一時フォルダ
        /// </summary>
        public string TmpDir { get; set; } = Path.GetTempPath();
        /// <summary>
        /// PDFのページをコレクション
        /// </summary>
        public IEnumerable<PdfPageAdpter> PdfFileItems => this;

        public bool IsBusy { get; set; }
        public PdfFileItemCollection()
        {

        }
        public PdfFileItemCollection(ILogger<PdfFileItemCollection> logger,IConfigService<AppSetting> config):this()
        {
            _config = config;
            _logger = logger;
        }

        public async Task AddItemAsync(string filePath, IProgress<(int, int)>? progress = null)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                IsBusy = true;
                await AddItem(filePath, TmpDir);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task AddRangeAsyn(IEnumerable<string> filePaths, IProgress<(int, int)>? progress = null)
        {
            var tasks = new List<Task>();
            

            try
            {
                foreach (var filePath in filePaths)
                {
                    tasks.Add(AddItem(filePath, TmpDir));
                }
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

        private async Task AddItem(string filePath, string tmpDirPath)
        {
            if(System.IO.Path.GetExtension(filePath)?.ToLower() != ".pdf")
            {
                return;
            }
            var item = new PdfItem(filePath, tmpDirPath);
            _pdfList.Add(item);
            await item.InitilizePageAsync();

            var tasks=new List<Task>();
            foreach (var page in item.Pages)
            {
                var addItem = new PdfPageAdpter(this, page,_config.Config,(t)=>this.Remove(t));
                try
                {
                    _lock.EnterWriteLock();
                    Add(addItem);
                    _logger.LogInformation("ADD PDF ITEM {item}",addItem.FileNameToSave);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
                tasks.Add(addItem.Initialize());
            }
            await Task.WhenAll(tasks);
            _logger.LogInformation("PDF ITEM INITIALIZED");
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
