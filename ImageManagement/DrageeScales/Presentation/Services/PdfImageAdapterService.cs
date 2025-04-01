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
using System.Threading;
using System.Threading.Tasks;

namespace DrageeScales.Presentation.Services
{
    public class PdfImageAdapterService : IDisposable
    {
        CompositeDisposable _disposables = new();
        private ISubject<ImageMangementArgBase> _subject = new Subject<ImageMangementArgBase>();

        readonly ILogger _logger;

        public IPdfFileItemCollection Collection { get; }

        public IObserver<FileOpenArg> FileOrDirOpenObserver { get; }

        public IObserver<SaveToPdfromAllImagesArg> SaveToPdfromAllImagesObserver { get; }

        public IObserver<SaveToPdfromImagesArg> SaveToPdfromImagesObserver { get; }

        public IObserver<RemovePdfPageAdapterArg> RemovePdfPageAdapterObserver { get; }

        public PdfImageAdapterService()
        {
            Collection = new PdfFileItemCollection();
            FileOrDirOpenObserver = _subject.AsObserver<FileOpenArg>();
            SaveToPdfromAllImagesObserver = _subject.AsObserver<SaveToPdfromAllImagesArg>();
            SaveToPdfromImagesObserver = _subject.AsObserver<SaveToPdfromImagesArg>();
            RemovePdfPageAdapterObserver = _subject.AsObserver<RemovePdfPageAdapterArg>();

            _disposables.Add(_subject.OfType<FileOpenArg>().Subscribe(async t => await OnGetPdfItemsAsync(t.FileOrDirPaths)));
            _disposables.Add(_subject.OfType<SaveToPdfromAllImagesArg>().Subscribe(async t => await OnSaveToPdfAllImages(t.OutDir)));
            _disposables.Add(_subject.OfType<SaveToPdfromImagesArg>().Subscribe(async t => await OnSaveToPdfImages(t.PdfPageAdpter, t.OutDir)));
            _disposables.Add(_subject.OfType<RemovePdfPageAdapterArg>().Subscribe(async t => await OnRemovePdfPageAdapter(t.PdfPageAdpter)));
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger"></param>
        public PdfImageAdapterService(ILogger<PdfImageAdapterService> logger) : this() => _logger = logger;


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
        public async Task OnSaveToPdfAllImages(string outDir)
        {
            if (!System.IO.Directory.Exists(outDir))
            {
                return;
            }
            await Collection.ForeachWhenall(t =>
            {
                _logger?.LogInformation($"TOPDF FROM {t.FileNameToSave} FILE.");
                return t.SaveToPdfAsync(outDir);
            });
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
            var numOfTasks = paths.Length;
            var numOfComplete = 0;
            var tasks=paths.Select(async t =>
            {
                await Collection.AddItemAsync(t);
                var done = Interlocked.Increment(ref numOfComplete);
                var percent = numOfComplete == 0 ? 0 : done * 100 / numOfTasks;
                progress.Report(percent);
                _logger?.LogInformation($"CONVERT PDF TO IMAGE FILE:{System.IO.Path.GetFileNameWithoutExtension(t)}");
            });

            await Task.WhenAll(tasks);
            await Task.Delay(1000);
            _logger?.LogInformation($"COMPLETE ADD {numOfTasks} FILES.");
        }

        public async Task OnReadBarcodeFromImage(IProgress<int> progress)
        {
            var numOfTasks = Collection.PdfFileItems.Count();
            var numOfComplete = 0;
            var tasks=Collection.PdfFileItems.Select(async t =>
            {
                await t.ReadBarcodeFromFileAsync();
                var done = Interlocked.Increment(ref numOfComplete);
                var percent = numOfComplete == 0 ? 0 : done * 100 / numOfTasks;
                progress.Report(percent);
                _logger?.LogInformation($"READ BARCODE FILE:{System.IO.Path.GetFileNameWithoutExtension(((IPdfFile)t.PdfPages.Parent).FilePath)} PAGE:{(t.PdfPages.PageNumber + 1)}");
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
