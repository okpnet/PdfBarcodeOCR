using DrageeScales.Helper;
using DrageeScales.Presentation.Dtos;
using DrageeScales.Views.Dtos;
using Microsoft.Extensions.Logging;
using PdfConverer.PdfProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DrageeScales.Presentation.Services
{
    public class PdfImageAdapterService : IDisposable
    {
        readonly ILogger _logger;
        CompositeDisposable _disposables = new();
        private ISubject<ImageMangementArgBase> _subject = new Subject<ImageMangementArgBase>();

        public PdfFileItemCollection Collection { get; } 

        public PdfImageAdapterService(PdfFileItemCollection pdfPageAdpters)
        {
            var path = CreateTempPath();
            Collection = pdfPageAdpters;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger"></param>
        public PdfImageAdapterService(PdfFileItemCollection pdfPageAdpters,ILogger<PdfImageAdapterService> logger):this(pdfPageAdpters)
        {
            _logger= logger;
            var path = CreateTempPath();
            Collection.TmpDir = path;
        }

        protected string CreateTempPath()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), AppDefine.CASH_DIR_NAME);
            try
            {
                if (!Directory.Exists(path))
                {
                    
                    Directory.CreateDirectory(path);
                    _logger?.LogInformation("CREATE DIR {path}", path);
                }
                else
                {
                    
                    new DirectoryInfo(path).DeleteFilesAndDirExceptRootDirectory();
                    _logger?.LogInformation("DLETE FIRN IN DIR {path}", path);
                }
                return path;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "DIR ERROR {path}", path);
                throw;
            }
        }

        protected async Task OnRemovePdfPageAdapter(PdfPageAdpter removeItem)
        {
            if (!Collection.PdfFileItems.Any(t => Equals(t, removeItem)))
            {
                return;
            }
            await Task.Run(() => Collection.Remove(removeItem));
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="pdfPageAdpter"></param>
        /// <param name="outDir"></param>
        /// <returns></returns>
        protected async Task OnSaveToPdfImages(PdfPageAdpter pdfPageAdpter, string outDir)
        {
            if (!System.IO.Directory.Exists(outDir))
            {
                return;
            }
            await pdfPageAdpter.SaveToPdfAsync(outDir);
            _logger?.LogInformation($"TOPDF FROM {pdfPageAdpter.FileNameToSave} FILE.");
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="outDir"></param>
        /// <returns></returns>
        public async Task OnSaveToPdfAllImages(IProgress<int> progress, string outDir)
        {
            if (!System.IO.Directory.Exists(outDir))
            {
                return;
            }

            var saveItems = Collection.Where(t => !t.IsBusy && t.ProgressValue == 0);
            var numOfSaveItems= saveItems.Count();
            var numOfComplete = 0;

            var tasks=Collection.Where(t => !t.IsBusy && t.ProgressValue == 0).Select(async t =>
            {
                var done = Interlocked.Increment(ref numOfComplete);
                var percent = numOfComplete == 0 ? 0 : done * 100 / numOfSaveItems;
                progress.Report(percent);
                _logger?.LogInformation($"TOPDF FROM {t.FileNameToSave} FILE.");
                await t.SaveToPdfAsync(outDir);
                t.Dispose();
            });

            await Task.WhenAll(tasks);
            await Task.Delay(500);
            _logger?.LogInformation($"COMPLETE SAVE {numOfSaveItems} FILES.");
        }
        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public async Task OnGetPdfItemsAsync(IProgress<int> progress,params string[] paths)
        {
            var enableFiles = GetFilesPahh(paths);
            if (!enableFiles.Any())
            {
                return;
            }
            try
            {
                var numOfTasks = paths.Length;
                var numOfComplete = 0;
                var tasks=paths.Select(async t =>
                {
                    try
                    {
                        var done = Interlocked.Increment(ref numOfComplete);
                        var percent = numOfComplete == 0 ? 0 : done * 100 / numOfTasks;
                        progress.Report(percent);
                        await Collection.AddItemAsync(t);
                        await Task.Delay(1000);
                        _logger?.LogInformation($"CONVERT PDF TO IMAGE FILE:{System.IO.Path.GetFileNameWithoutExtension(t)}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "CL:{class} M:{method} EXCEPTION",this.GetType().Name,nameof(PdfImageAdapterService.OnGetPdfItemsAsync));
                        throw;
                    }

                });

                await Task.WhenAll(tasks);
                await Task.Delay(1000);
                _logger?.LogInformation($"COMPLETE ADD {numOfTasks} FILES.");
            }catch(Exception ex)
            {
                _logger.LogWarning(ex, "CL:{class} M:{method} EXCEPTION", this.GetType().Name, nameof(PdfImageAdapterService.OnGetPdfItemsAsync));
                throw;
            }
        }

        public async Task OnReadBarcodeFromImage(IProgress<int> progress)
        {
            var numOfTasks = Collection.PdfFileItems.Count();
            var numOfComplete = 0;
            var tasks=Collection.PdfFileItems.Where(t=>!t.IsInit).Select(async t =>
            {
                await t.ReadBarcodeFromFileAsync();
                var done = Interlocked.Increment(ref numOfComplete);
                var percent = numOfComplete == 0 ? 0 : done * 100 / numOfTasks;
                progress.Report(percent);
                _logger?.LogInformation($"READ BARCODE FILE:{System.IO.Path.GetFileNameWithoutExtension(((IPdfFile)t.PdfPages.Parent).BaseFilePath)} PAGE:{(t.PdfPages.PageNumber + 1)}");
            });
            await Task.WhenAll(tasks);
            await Task.Delay(1000);
            _logger?.LogInformation($"COMPLETE READ BARCODE {numOfTasks} FILES.");
        }

        /// <summary>
        /// 文字列から
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        protected IEnumerable<string> GetFilesPahh(params string[] paths)
        {
            if (paths.Length == 0)
            {
                yield break;
            }
            foreach (var path in paths)
            {
                switch (GetUsageType(path))
                {
                    case UsageType.File:
                        yield return path;
                        break;
                    case UsageType.Dirictory:
                        foreach (var file in Directory.EnumerateFiles(path, "*.pdf"))
                        {
                            yield return file;
                        }
                        yield break;
                    default:
                        yield break;
                }
            }
        }


        public UsageType GetUsageType(string value)
        {
            if (value is null or "")
            {
                return UsageType.None;
            }
            if (System.IO.Directory.Exists(value))
            {
                return UsageType.Dirictory;
            }
            if (System.IO.File.Exists(value))
            {
                return UsageType.File;
            }
            return UsageType.None;
        }

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}
