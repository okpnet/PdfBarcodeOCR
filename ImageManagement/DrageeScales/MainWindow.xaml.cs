using DrageeScales.Shared.Dtos;
using DrageeScales.Views.Dtos;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DrageeScales
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// インジェクション
        /// </summary>
        public MainWindowModel WindowModel { get; }

        public ToastItemCollction ToastItems { get; set; }

        public ModalOptionBase ModalOptions { get; set; }

        public MainWindow(MainWindowModel mainWindowModel)
        {
            this.InitializeComponent();
            WindowModel = mainWindowModel;
            ToastItems = new ();
        }

        private async Task<StorageFolder> ShowFolderPicker()
        {
            var picker = new FolderPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            var result = await picker.PickSingleFolderAsync();
            return result;
        }

        private async void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            var files = await picker.PickMultipleFilesAsync();
            if (files is null)
            {
                return;
            }
            await WindowModel.OnOpenSource(files.Select(t=>t.Path));
        }


        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var hasDir = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.ContainsItem(AppDefine.CASH_OUTPUTDIR_KEY);
            StorageFolder folder;

            if (!hasDir)
            {
                folder = await ShowFolderPicker();
            }
            else
            {
                folder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync(AppDefine.CASH_OUTPUTDIR_KEY);
                var dialog = new ContentDialog
                {
                    Title = "フォルダ選択の確認",
                    Content = $"このまま'{folder.Name}'へ保存します",
                    PrimaryButtonText = "はい",
                    CloseButtonText = "いいえ",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.Content.XamlRoot // 必須！
                };

                if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                {
                    folder =await ShowFolderPicker();
                }
            }

            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace(AppDefine.CASH_OUTPUTDIR_KEY, folder);
            await WindowModel.OnSaveAllFile(folder.Path);
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.None;
                return;
            }
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                return;
            }
            var items = await e.DataView.GetStorageItemsAsync();
            await WindowModel.OnOpenSource(items.Select(t => t.Path));
        }
    }
}
