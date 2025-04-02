using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using DrageeScales.Core;
using DrageeScales.Shared.Dtos;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DrageeScales.Shared.Controls
{
    public sealed partial class OverlapModalProgressView : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool isProgress;

        ModalOptionBase _modalOptionBase;
        public ModalOptionBase BaseOption 
        {
            get => _modalOptionBase;
            set
            {
                _modalOptionBase = value;
                isProgress = _modalOptionBase is ProgressModalOption;
                var state = _modalOptionBase switch
                {
                    ProgressModalOption progress=> "Progress",
                    BusyModalOption busy=> "Busy",
                    _=>"None"
                };
                if (IsLoaded)
                {
                    VisualStateManager.GoToState(this, state, false);
                }

                OnPropertyChanged(nameof(BaseOption));
                if (isProgress)
                {
                    OnPropertyChanged(nameof(ProgressModalOption));
                }
                else
                {
                    OnPropertyChanged(nameof(BusyModalOption));
                }
                
            }
        }

        public ProgressModalOption ProgressModalOption => (_modalOptionBase as ProgressModalOption)??new(new());

        public BusyModalOption BusyModalOption => (_modalOptionBase as BusyModalOption)??new();

        public OverlapModalProgressView()
        {
            this.InitializeComponent();
        }
    }
}
