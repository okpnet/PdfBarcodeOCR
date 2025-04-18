using DrageeScales.Core;
using DrageeScales.Shared.Commands;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DrageeScales.Shared.Dtos
{
    public class ToastItemBtn: NotifyPropertyChangedBase, IDisposable, INotifyPropertyChanged
    {
        Visibility _isVisiblity = Visibility.Collapsed;
        public Visibility IsVisibility
        {
            get => _isVisiblity;
            set
            {
                if(_isVisiblity == value)
                {
                    return;
                }
                _isVisiblity = value;
                OnPropertyChanged(nameof(IsVisibility));
            }
        }

        string _label=string.Empty;
        public string Label
        {
            get => _label;
            set
            {
                if (_label == value)
                {
                    return;
                }
                _label = value;
                OnPropertyChanged(nameof(Label));
            }
        }

        Action _clickDelegate;
        public Action ClickDelegate
        {
            get => _clickDelegate;
            set
            {
                if( _clickDelegate == value)
                {
                    return;
                }
                _clickDelegate = value;
                OnPropertyChanged(nameof(ClickDelegate));
            }
        }
        public ICommand ClickCommand { get; set; }

        internal ToastItem Parent {  get; set; }

        public IObservable<ToastItem> ClickEvent { get; }

        public ToastItemBtn(string label,Action clickAction)
        {
            Label = label;
            ClickCommand = new RelayCommand(clickAction);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
