using DrageeScales.Core;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

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

        Action? _beforeCloseDelegate;
        public Action? BeforeCloseDelegate
        {
            get => _beforeCloseDelegate;
            set
            {
                if (ReferenceEquals(_beforeCloseDelegate, value))
                {
                    return;
                }
                _beforeCloseDelegate = value;
                OnPropertyChanged(nameof(BeforeCloseDelegate));
            }
        }

        ToastItemBtn _toastItemBtn = new(string.Empty,()=>{ return; });

        public ToastItemBtn ItemBtn
        {
            get => _toastItemBtn;
            set
            {
                if(ReferenceEquals(_toastItemBtn, value))
                {
                    return;
                }
                _toastItemBtn = value;
                _toastItemBtn.Parent = this;
                OnPropertyChanged(nameof(ToastItemBtn));
            }
        }

        public ToastItem(InfoBarSeverity severity, string message, bool isClosebtn=true, uint duration=5000)
        {
            Severity = severity;
            Message = message;
            IsClosebtn = isClosebtn;
            Duration = duration;
        }

        public ToastItem(InfoBarSeverity severity, string message,ToastItemBtn btn, bool isClosebtn = true, uint duration = 5000):this(severity,message,isClosebtn,duration)
        {
            ItemBtn = btn;
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
            _beforeCloseDelegate?.Invoke();
            RemoveAction(this);
        }
    }
}
