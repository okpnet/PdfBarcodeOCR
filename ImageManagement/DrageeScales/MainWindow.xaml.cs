using DrageeScales.Views.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics;
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
        readonly ILogger? _logger;
        CompositeDisposable _disposables = new();
        string _state = "Informations";
        /// <summary>
        /// インジェクション
        /// </summary>
        public MainWindowModel WindowModel { get; }


        public MainWindow(MainWindowModel mainWindowModel, ILogger<MainWindow> logger)
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);

            //Win32の設定
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var m_AppWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            //m_AppWindow.SetTaskbarIcon("Asssets/appicon.ico");
            m_AppWindow.SetIcon("Assets/appicon.ico");

            _logger = logger;
            WindowModel = mainWindowModel;
            _disposables.Add(WindowModel.CollectionCountChangeEvent.Subscribe(t =>
            {
                StateChange(t > 0);
                //DispatcherQueue.TryEnqueue(() => StateChange(t > 0));
            }));
            this.DispatcherQueue.TryEnqueue(() =>
            {
                // Window完全描画後の処理
                OnLoaded();
            });
            _logger.LogInformation("INITILIZED MAINWINDOW");
        }
        /// <summary>
        /// ウィンドウの初期化が完了
        /// </summary>
        private void OnLoaded()
        {
            WindowModel.CheckUpdateAsync();
        }

        private void StateChange(bool isChangeState)
        {
            var state = isChangeState ? "Collections" : "Informations";
            if(_state == state)
            {
                return;
            }
            _state = state;
            var result=VisualStateManager.GoToState(RootPage, _state, false);
            _logger.LogInformation("STATE CHANGED {state} IS {result}",state,result);
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
            if (files is null || files.Count==0)
            {
                return;
            }
            StateChange(true);
            await Task.Delay(200);
            await WindowModel.OnOpenSource(files.Select(t=>t.Path));
        }



        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var folder = await ShowFolderPicker();
            if(folder is null || folder.Path is (null or ""))
            {
                return;
            }
            StateChange(true);
            await Task.Delay(200);
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
            if (items.Count == 0)
            {
                return;
            }
            StateChange(true);
            await Task.Delay(200);
            await WindowModel.OnOpenSource(items.Select(t => t.Path));
        }

        public void Dispose()
        {
            _disposables.Clear();
        }

        private async void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            StateChange(true);
            await Task.Delay(200);
            await WindowModel.OnClearAll();
        }
    }
}
