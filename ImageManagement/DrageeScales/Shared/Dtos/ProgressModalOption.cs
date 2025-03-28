using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Dtos
{
    public class ProgressModalOption: ModalOptionBase, INotifyPropertyChanged ,IDisposable
    {
        private IDisposable _progressEvent = default;
        private double _progressValue=0;

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue == value)
                {
                    return;
                }
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        private Progress<uint> _progress;

        public IProgress<uint> Progress
        {
            get => _progress;
            set
            {
                if (_progressEvent is not null)
                {
                    _progressEvent.Dispose();
                }
                _progress = (Progress<uint>)value;
                _progressEvent = Observable.FromEventPattern<uint>(
                    _progress, nameof(Progress<uint>.ProgressChanged)).
                    Subscribe(t => 
                    {
                        ProgressValue = Convert.ToDouble(t.EventArgs);
                        System.Diagnostics.Debug.WriteLine($"[SCR] {nameof(Progress)} = {t.EventArgs}");
                    });
            }
        }

        public ProgressModalOption(Progress<uint> progress)
        {
            Progress = progress;
        }

        public void Dispose()
        {
            _progressEvent?.Dispose();
        }
    }
}
