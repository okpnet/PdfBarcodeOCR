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
        string _regularFilter;
        public string RegularExpressionFilter 
        {
            get => _regularFilter;
            set
            {
                if (_regularFilter == value)
                {
                    return;
                }
                _regularFilter = value;
                OnPropertyChanged(nameof(RegularExpressionFilter));
            }
        }
        
        int _thumbnailRate = AppDefine.THUMBNAIL_SIZE;
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

        int _shreddedRate=AppDefine.SHREDDED_HEIGHT;
        public int ShreddedRate
        {
            get => _shreddedRate;
            set
            {
                if(_shreddedRate == value)
                {
                    return;
                }
                _shreddedRate = value;
                OnPropertyChanged(nameof(ShreddedRate));
            }
        }

        double _strechLength = AppDefine.STRECH_WIDTH;
        public double StretchLength
        {
            get => _strechLength;
            set
            {
                if(_strechLength == value)
                {
                    return;
                }
                _strechLength = value;
                OnPropertyChanged(nameof(StretchLength));
            }
        }

        public AppSetting()
        {
            _regularFilter = "";
            _thumbnailRate = AppDefine.THUMBNAIL_SIZE;
            _shreddedRate= AppDefine.SHREDDED_HEIGHT;
        }

        public AppSetting(string regularExpressionFilter, int thumbnailRate, int shreddedRate)
        {
            _regularFilter = regularExpressionFilter;
            _shreddedRate = shreddedRate;
            _thumbnailRate = thumbnailRate;
        }
    }
}
