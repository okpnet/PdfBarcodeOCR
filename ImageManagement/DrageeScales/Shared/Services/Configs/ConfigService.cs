using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.Configs
{
    public class ConfigService<T>: IConfigService<T>,IDisposable
    {
        readonly ILogger? _logger;
        readonly CompositeDisposable _disposables = new();
        IConfigModelFacade<T> _facade;
        T _config;

        public T Config 
        {
            get => _config;
            set
            {
                _config = value;
                if (!IsSaveToChangeAtOnce)
                {
                    return;
                }
                _facade.Save(_config);
            }
        }

        public bool IsSaveToChangeAtOnce { get; set; }

        public ConfigService(IConfigModelFacade<T> facade)
        {
            IsSaveToChangeAtOnce = true;
            _facade = facade;
            _config = facade.Load();

            if(_config is INotifyPropertyChanged changeEvent)
            {
                _disposables.Add(
                    Observable.FromEventPattern<PropertyChangedEventArgs>(changeEvent, nameof(changeEvent.PropertyChanged)).Subscribe(e =>
                    {
                        if(!IsSaveToChangeAtOnce)
                        {
                            return;
                        }
                        _facade.Save(_config);
                    })
                );
            }
        }

        public ConfigService(ILogger logger, IConfigModelFacade<T> facade):this(facade)
        {
            _logger = logger;
        }

        public async Task SaveAsync() => await Task.Run(()=> _facade.Save(_config));

        public async Task LoadAsync() => _config= await Task.Run(() => _facade.Load());

        public void Dispose()
        {
            _disposables.Clear();
            _facade.Save(Config);
            if(_facade is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if(Config is IDisposable condigDisposable)
            {
                condigDisposable.Dispose();
            }
            
        }
    }
}
