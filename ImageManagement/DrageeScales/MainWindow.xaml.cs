using DrageeScales.Shared.Dtos;
using DrageeScales.Views.Dtos;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Reactive.Disposables;
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
    public sealed partial class MainWindow : Window,IDisposable
    {
        CompositeDisposable _disposables = new();
        /// <summary>
        /// インジェクション
        /// </summary>
        public MainWindowModel WindowModel { get; }

        public MainWindow(MainWindowModel mainWindowModel)
        {
            this.InitializeComponent();
            WindowModel = mainWindowModel;
            _disposables.Add( WindowModel.CollectionAnyEvent.Subscribe(t => StateChange(t)));
        }

        private void StateChange(bool isCollectionAny)
        {
            var state = isCollectionAny ? "Collections" : "Infomation";
            VisualStateManager.GoToState(RootPage, state, false);
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
            var folder = await ShowFolderPicker();
            if(folder is null || folder.Path is (null or ""))
            {
                return;
            }
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

        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}
