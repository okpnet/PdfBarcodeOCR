using DrageeScales.Presentation.Services;
using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;
using DrageeScales.Shared.Services.NetspakleUpdate;
using Microsoft.Extensions.Logging;
using PdfConverer.PdfProcessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace DrageeScales.Views.Dtos
{
    public class MainWindowModel: ViewModelBase,IDisposable,INotifyPropertyChanged
    {
        readonly ILogger? _logger;

        CompositeDisposable _disposables = new();
        Subject<object> _subject = new();

        bool _isEnable = false;
        public bool IsEnable 
        {
            get => _isEnable;
            private set
            {
                if (_isEnable == value)
                {
                    return;
                }
                _isEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }

        public PdfImageAdapterService Service { get; }

        public PdfFileItemCollection Collection => Service.Collection;

        ModalOptionBase _modalOptionBases;
        public ModalOptionBase ModalOptionBases 
        {
            get => _modalOptionBases;
            set
            {
                if (Equals(_modalOptionBases, value))
                {
                    return;
                }
                _modalOptionBases = value;
                OnPropertyChanged(nameof(ModalOptionBases));
            }
        }

        ToastItemCollction _toastItems = new ToastItemCollction();
        public ToastItemCollction ToastItems 
        {
            get => _toastItems;
            set
            {
                if (Equals(_toastItems, value))
                {
                    return;
                }
                OnPropertyChanged(nameof(ToastItems));
            }
        }

        int _collectionCount;
        public int CollectionCount
        {
            get => _collectionCount;
            set
            {
                if(_collectionCount == value)
                {
                    return;
                }
                _collectionCount = value;
                OnPropertyChanged(nameof(CollectionCount));
                _subject.OnNext(_collectionCount);
            }
        }

        public IObservable<int> CollectionCountChangeEvent { get; }

        public MainWindowModel(IConfigService<AppSetting> configService, PdfImageAdapterService service, ILogger<MainWindowModel> logger) : base(configService)
        {
            Service = service;
            _logger = logger;
            ModalOptionBases = new BusyModalOption();
            CollectionCountChangeEvent = _subject.AsObservable<object>().OfType<int>();

            _disposables.Add(
                Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(service.Collection, nameof(service.Collection.CollectionChanged)).Subscribe
                (t =>
                {
                    if (CollectionCount == service.Collection.Count)
                    {
                        return;
                    }
                    CollectionCount = service.Collection.Count;
                })
            );
        }

        public async Task OnOpenSource(IEnumerable<string> source)
        {
            var registerFiles = Collection.Select(t => t.PdfPages.Parent).OfType<IPdfFile>().Select(t => t.BaseFilePath);
            var unContainsFiles = source.Where(t=>!registerFiles.Contains(t)).ToArray();
            var numOfDuplicateFiles = registerFiles.Join(source, a => a, b => b, (a, b) => a).Count();
            if (!unContainsFiles.Any())
            {
                return;
            }
            try
            {
                IsEnable = false;
                var progressTotal = new Progress<int>();
                var progressFile = new Progress<int>(t => { 
                    _logger.LogInformation("CONVERTING OnGetPdfItemsAsync {percent}%.", t);
                });
                var progressBarcode = new Progress<int>(t =>
                {
                    var report = t;
                    _logger.LogInformation("CONVERTING OnReadBarcodeFromImage {percent}%.", report);
                    ((IProgress<int>)progressTotal).Report(t);
                });
                
                ModalOptionBases = new BusyModalOption();
                ModalOptionBases.IsEnabled = true;
                await Task.Delay(200);
                await Service.OnGetPdfItemsAsync(progressFile, unContainsFiles);
                ModalOptionBases.IsEnabled = false;

                ModalOptionBases = new ProgressModalOption(progressTotal);
                await Task.Delay(200);
                ModalOptionBases.IsEnabled = true;
                await Service.OnReadBarcodeFromImage(progressBarcode);

                ModalOptionBases = new BusyModalOption();
                ModalOptionBases.IsEnabled = true;
                await Task.Delay(200);
                await Service.OnFileNameCheckingAsync(ConfigService.Config.GlueChar);
            }
            finally
            {
                ModalOptionBases.IsEnabled = false;
                var message = $"{unContainsFiles.Length} ファイル処理しました" + (numOfDuplicateFiles > 0 ? $"。 {numOfDuplicateFiles} ファイルは登録されています。":""); 
                ToastItems.Add(new ToastItem(Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success, message));
                IsEnable = true;
            }
        }

        public async Task OnSaveAllFile(string path) 
        {
            try
            {
                var progressTotal = new Progress<int>();
                ModalOptionBases = new BusyModalOption();
                ModalOptionBases.IsEnabled = true;
                await Service.OnSaveToPdfAllImages(progressTotal, path);
            }
            finally
            {
                ModalOptionBases.IsEnabled = false;
                var message = $"できるかぎりファイルを保存しました";
                ToastItems.Add(new ToastItem(Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success, message));
            }
        }

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}
