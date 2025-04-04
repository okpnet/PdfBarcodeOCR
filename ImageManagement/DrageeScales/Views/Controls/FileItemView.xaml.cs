using DrageeScales.Shared.Helper;
using DrageeScales.Views.Dtos;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Reactive.Linq;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DrageeScales.Views.Controls
{
    public sealed partial class FileItemView : UserControl
    {
        FileViewModel _fileViewModel = new();
        FileViewModel FileViewModels => _fileViewModel;

        public PdfPageAdpter PdfAdapter { get=> _fileViewModel.PdfAdapter; set=> _fileViewModel.PdfAdapter=value; }

        public FileItemView()
        {
            this.InitializeComponent();
            _fileViewModel = new();
        }

        private async void HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            var window = (App.Current as App)?.MainWindow;
            if (window is null)
            {
                return;
            }
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(folderPicker, hwnd);
            var result=await folderPicker.PickSingleFolderAsync();
            if(result is null || result.Path is (null or ""))
            {
                return;
            }
            if (FileViewModels.HasFileExists(result.Path) && 
                await Content.XamlRoot.FileOverWriteConfirmAsync(PdfAdapter.FileNameToSave) == DialogHelperResultYesNo.No)
            {
                return;
            }
            await FileViewModels.SaveFileAsync(result.Path);
        }

        private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(await XamlRoot.RemoveConfirmAsync(FileViewModels.PdfAdapter.FileNameToSave) == DialogHelperResultYesNo.No)
            {
                return;
            }
            FileViewModels.RemoveItem();
        }

        private void Button_Click_1(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FileViewModels.OpenPdfFile();
        }
    }
}
