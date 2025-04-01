using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.Configs
{
    public interface IConfigService<T>
    {
        T Config{ get; }

        bool IsSaveToChangeAtOnce { get; }

        Task SaveAsync();

        Task LoadAsync();
    }
}
