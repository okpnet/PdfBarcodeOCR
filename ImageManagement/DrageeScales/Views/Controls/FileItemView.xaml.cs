using DrageeScales.Views.Dtos;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DrageeScales.Views.Controls
{
    public sealed partial class FileItemView : UserControl,IDisposable,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        CompositeDisposable disposables = new();

        PdfPageAdpter _pdfAdapter; 
        public PdfPageAdpter PdfAdapter 
        {
            get => _pdfAdapter;
            set
            {
               
                if(_pdfAdapter == value)
                {
                    return;
                }
                _pdfAdapter = value;
                OnPropertyChanged(nameof(PdfAdapter));
            }
        } 

        public int ThumbnailWidth=>AppDefine.THUMBNAIL_SIZE;

        public int PanelWitdth=>AppDefine.THUMBNAIL_SIZE*2;

        public int BrusThikness { get; set; } = 0;

        public System.Drawing.Color BorderColors { get; set; } = Color.Transparent;

        public FileItemView()
        {
            this.InitializeComponent();
        }

        public void Dispose()
        {
            disposables.Clear();
        }

        private async void HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var filePicker = new FileSavePicker();
            filePicker.SuggestedFileName = PdfAdapter.FileNameToSave;
            InitializeWithWindow.Initialize(filePicker, WindowNative.GetWindowHandle(this));
            var result=await filePicker.PickSaveFileAsync();
            if(result is null || result.Path is (null or ""))
            {
                return;
            }
            if (System.IO.File.Exists(result.Path))
            {
                var dialog = new ContentDialog
                {
                    Title = "上書きの確認",
                    Content = $"ファイル'{result.Name}'は存在します。上書きしますか？",
                    PrimaryButtonText = "はい",
                    CloseButtonText = "いいえ",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.Content.XamlRoot // 必須！
                };
                if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                {
                    return;
                }
            }
            await PdfAdapter.SaveToPdfAsync(result.Path);
        }
    }
}
