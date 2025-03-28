using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrageeScales.Core;

namespace DrageeScales.Shared.Dtos
{
    public class ToastItem:NotifyPropertyChangedBase,IDisposable,INotifyPropertyChanged
    {
        internal Action<ToastItem> RemoveAction {  get; set; }

        InfoBarSeverity _severity = InfoBarSeverity.Informational;
        public InfoBarSeverity Severity
        {
            get => _severity;
            set
            {
                if (_severity == value)
                {
                    return;
                }
                _severity = value;
                OnPropertyChanged(nameof(Severity));
            }
        }

        string _message;
        public string Message
        {
            get => _message;
            set
            {
                if (_message == value)
                {
                    return;
                }
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        bool _isClosebtn;
        public bool IsClosebtn
        {
            get => _isClosebtn;
            set
            {
                if (_isClosebtn == value)
                {
                    return;
                }
                _isClosebtn = value;
                OnPropertyChanged(nameof(IsClosebtn));
            }
        }

        uint _duration=3000;
        public uint Duration
        {
            get => _duration;
            set
            {
                if (_duration == value)
                {
                    return;
                }
                _duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }

        public ToastItem(InfoBarSeverity severity, string message, bool isClosebtn=true, uint duration=5000)
        {
            Severity = severity;
            Message = message;
            IsClosebtn = isClosebtn;
            Duration = duration;
        }

        internal async Task ShowToast()
        {
            if (Duration == 0)
            {
                IsClosebtn = true;
                return;
            }
            await Task.Delay((int)Duration);
            this.Dispose();
        }

        public void Dispose()
        {
            RemoveAction(this);
        }
    }
}
