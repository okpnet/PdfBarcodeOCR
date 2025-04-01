using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Dtos
{
    public sealed class AppSetting: ModalOptionBase,INotifyPropertyChanged
    {
        string _outputDir;
        public string OutputDir 
        {
            get => _outputDir;
            set
            {
                if (_outputDir == value)
                {
                    return;
                }
                _outputDir = value;
                OnPropertyChanged(nameof(OutputDir));
            }
        }
        string _fileOpenDir;
        public string FileOpenDir
        {
            get => _fileOpenDir;
            set
            {
                if (_fileOpenDir == value)
                {
                    return;
                }
                _fileOpenDir = value;
                OnPropertyChanged(nameof(FileOpenDir));
            }
        }

        int _thumbnailRate = 256;
        public int ThumbnailRate 
        {
            get => _thumbnailRate;
            set
            {
                if (_thumbnailRate == value)
                {
                    return;
                }
                _thumbnailRate = value;
                OnPropertyChanged(nameof(ThumbnailRate));
            }
        }

        public AppSetting()
        {
            _outputDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _fileOpenDir= System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _thumbnailRate = 256;
        }

        public AppSetting(string outputDir, string fileOpenDir, int thumbnailRate)
        {
            _outputDir = outputDir;
            _fileOpenDir = fileOpenDir;
            _thumbnailRate = thumbnailRate;
        }
    }
}
