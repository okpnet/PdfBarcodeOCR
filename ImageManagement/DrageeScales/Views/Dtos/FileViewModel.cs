using DrageeScales.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfConverer.PdfProcessing;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace DrageeScales.Views.Dtos
{
    public class FileViewModel: NotifyPropertyChangedBase,INotifyPropertyChanged
    {
        readonly ILogger? _logger;
        PdfPageAdpter _pdfAdapter=new();
        public PdfPageAdpter PdfAdapter
        {
            get => _pdfAdapter;
            set
            {
                if (_pdfAdapter == value)
                {
                    return;
                }
                _pdfAdapter = value;
                OnPropertyChanged(nameof(PdfAdapter));
            }
        }

        public int ThumbnailSideLength => AppDefine.THUMBNAIL_SIZE;//一辺の長さ

        public int PanelWitdth => AppDefine.THUMBNAIL_SIZE * 2 + 8;//+Margin

        public FileViewModel()
        {
            _logger= App.Services.GetService<ILogger<FileViewModel>>();
        }

        public async Task SaveFileAsync(string dirPath)
        {
            await PdfAdapter.SaveToPdfAsync(dirPath);
            await Task.Delay(500);
            PdfAdapter.Dispose();
        }

        public void RemoveItem()
        {
            PdfAdapter.Dispose();
        }

        public bool HasFileExists(string dirPath)
        {
            var filePath = System.IO.Path.Combine(dirPath, $"{PdfAdapter.FileNameToSave}.pdf");
            return System.IO.File.Exists(filePath);
        }

        public async void OpenPdfFile()
        {
            if (PdfAdapter.PdfPages.Parent is not IPdfFile pdfFile)
            {
                return;
            }
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(pdfFile.BaseFilePath);
                if (file != null)
                {
                    bool success = await Launcher.LaunchFileAsync(file);
                    if (!success)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"CAN NOT OPEN FILE {pdfFile.BaseFilePath}");
            }
        }
    }
}
