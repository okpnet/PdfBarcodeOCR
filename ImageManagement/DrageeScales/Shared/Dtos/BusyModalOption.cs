using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Dtos
{
    public class BusyModalOption : ModalOptionBase, INotifyPropertyChanged
    {
        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value)
                {
                    return;
                }
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
    }
}
