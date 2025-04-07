using DrageeScales.Presentation.Services;
using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;
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

        bool _isCollectionAny = false;
        public bool IsCollectionAny 
        {
            get => _isCollectionAny;
            private set
            {
                if (_isCollectionAny == value)
                {
                    return;
                }
                _isCollectionAny = value;
                _subject.OnNext(value);
                OnPropertyChanged(nameof(IsCollectionAny));
            }
        }

        public IObservable<bool> CollectionAnyEvent { get; }

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

        public MainWindowModel(IConfigService<AppSetting> configService, PdfImageAdapterService service,ILogger<MainWindowModel> logger):base(configService)
        {
            Service = service;
            _logger = logger;
            ModalOptionBases = new BusyModalOption();
            CollectionAnyEvent=_subject.AsObservable<object>().OfType<bool>();
            _disposables.Add(
                Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(
                Service.Collection, $"{nameof(ObservableCollection<PdfPageAdpter>.CollectionChanged)}").
                Subscribe(t => 
                {
                    IsCollectionAny = Service.Collection.IsAny;
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
                
                ModalOptionBases = new ProgressModalOption(progressTotal);
                await Task.Delay(200);
                await Service.OnReadBarcodeFromImage(progressBarcode);
            }
            finally
            {
                ModalOptionBases.IsEnabled = false;
                var message = $"{unContainsFiles.Length} ファイル処理しました" + (numOfDuplicateFiles > 0 ? $"。 {numOfDuplicateFiles} ファイルは登録されています。":""); 
                ToastItems.Add(new ToastItem(Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success, message));
            }
        }

        public async Task OnSaveAllFile(string path) 
        {
            try
            {
                var progressTotal = new Progress<int>();
                ModalOptionBases = new ProgressModalOption(progressTotal);
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
