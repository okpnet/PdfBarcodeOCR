using DrageeScales.Shared.Dtos;
using DrageeScales.Views.Dtos;
using ImageManagement.Adapter;
using ImageManagement.Service;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        public ProgressModalOption ProgressModalOption { get; set; }

        public MainWindow(MainWindowModel mainWindowModel)
        {
            this.InitializeComponent();
            WindowModel = mainWindowModel;
            ToastItems = new ();
            ProgressModalOption = new(new());
        }

        private async void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            var folder = await picker.PickSingleFolderAsync();
            if (folder is null)
            {
                return;
            }
            WindowModel.OnOpenSource(folder.Name);
        }

        private void BarcodeAnalysisBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            WindowModel.Service.SaveToPdfromAllImagesObserver.OnNext(new )
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
            WindowModel.OnOpenSource(items.Select(t => t.Path).ToArray());
        }
    }
}
