using ImageManagement.Adapter;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DrageeScales.Views.Controls
{
    public sealed partial class FileItemView : UserControl,IDisposable
    {
        CompositeDisposable disposables = new();

        public PdfPageAdpter PdfAdapter { get; set; }

        public int TumbnailSlideRate { get; set; } = 256;

        public int BrusThikness { get; set; } = 0;

        public System.Drawing.Color BorderColors { get; set; } = Color.Transparent;

        public FileItemView()
        {
            this.InitializeComponent();
            disposables.Add(
                Observable.FromEventPattern<PropertyChangedEventArgs>(PdfAdapter,nameof(PdfAdapter.PropertyChanged)).
                Where(t=>t.EventArgs.PropertyName==nameof(PdfPageAdpter.IsBarcodeReadFail)).
                Subscribe(t =>
                {
                    if (PdfAdapter.IsBarcodeReadFail)
                    {
                        BorderColors = Color.FromArgb(255, 129, 129);
                        BrusThikness = 5;
                    }
                    else
                    {
                        BorderColors = Color.Transparent;
                        BrusThikness = 0;
                    }
                })
            );
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
