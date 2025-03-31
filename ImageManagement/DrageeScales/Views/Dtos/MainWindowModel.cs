using ImageManagement.Adapter;
using ImageManagement.Service;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace DrageeScales.Views.Dtos
{
    public class MainWindowModel:IDisposable
    {
        CompositeDisposable _disposables = new();

        public bool IsCollectionAny { get; set; }

        public PdfImageAdapterService Service { get; }

        public MainWindowModel(CompositeDisposable disposables, PdfImageAdapterService service)
        {
            _disposables = disposables;
            Service = service;
            _disposables.Add(
                Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(
                Service.Collection, $"{nameof(ObservableCollection<PdfPageAdpter>.CollectionChanged)}").
                Subscribe(t => IsCollectionAny = Service.Collection.IsAny)
                );
        }

        public void OnOpenSource(params string[] souecees)=>Service.FileOrDirOpenObserver.OnNext(new ImageManagement.Service.Argments.FileOpenArg(souecees));

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}
