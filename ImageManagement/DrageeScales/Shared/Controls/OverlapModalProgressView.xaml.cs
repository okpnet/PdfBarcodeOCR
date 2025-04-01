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
    public sealed partial class OverlapModalProgressView : UserControl
    {
        bool isProgress;

        ModalOptionBase _modalOptionBase;
        public ModalOptionBase BaseOption 
        {
            get => _modalOptionBase;
            set
            {
                _modalOptionBase = value;
                isProgress = _modalOptionBase is ProgressModalOption;
                VisualStateManager.GoToState(this, _modalOptionBase is ProgressModalOption ? "Progress":"Busy", true);
            }
        }

        public ProgressModalOption ProgressModalOption => (ProgressModalOption)_modalOptionBase;

        public BusyModalOption BusyModalOption => (BusyModalOption)_modalOptionBase;

        public OverlapModalProgressView()
        {
            this.InitializeComponent();
        }
    }
}
