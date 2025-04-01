using DrageeScales.Presentation.Services;
using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DrageeScales.Views.Dtos
{
    public class MainWindowModel: ViewModelBase,IDisposable
    {
        CompositeDisposable _disposables = new();

        public bool IsCollectionAny { get; set; }

        public PdfImageAdapterService Service { get; }

        public ModalOptionBase ModalOptionBases { get; set; }

        public ToastItemCollction ToastItems { get; set; }

        public MainWindowModel(IConfigService<AppSetting> configService, PdfImageAdapterService service):base(configService)
        {
            Service = service;
            _disposables.Add(
                Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(
                Service.Collection, $"{nameof(ObservableCollection<PdfPageAdpter>.CollectionChanged)}").
                Subscribe(t => IsCollectionAny = Service.Collection.IsAny)
                );
        }

        public async Task OnOpenSource(IEnumerable<string> souecees)
        {
            try
            {
                int fileProgress = 0;
                int barcodeProgress = 0;
                var progressTotal = new Progress<int>();
                var progressFile = new Progress<int>(t => { 
                    fileProgress = t;
                    ((IProgress<int>)progressTotal).Report((fileProgress / 2) + (barcodeProgress / 2));
                });
                var progressBarcode = new Progress<int>(t =>
                {
                    barcodeProgress = t;
                    ((IProgress<int>)progressTotal).Report((fileProgress / 2) + (barcodeProgress / 2));
                });

                ModalOptionBases = new ProgressModalOption(progressTotal);
                await Service.OnGetPdfItemsAsync(progressFile, souecees.ToArray());
                await Service.OnReadBarcodeFromImage(progressBarcode);
            }
            finally
            {

            }
        }

        public async Task OnSaveAllFile(string path) 
        {
            await Service.OnSaveToPdfAllImages(path);
        }

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}
