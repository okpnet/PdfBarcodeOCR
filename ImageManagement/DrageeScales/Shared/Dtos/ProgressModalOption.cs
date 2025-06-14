using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace DrageeScales.Shared.Dtos
{
    public class ProgressModalOption: ModalOptionBase, INotifyPropertyChanged ,IDisposable
    {
        private IDisposable _progressEvent = default;
        private double _progressValue=0;

        public double ProgressValue
        {
            get => _progressValue;
            protected set
            {
                if (_progressValue == value)
                {
                    return;
                }
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        private Progress<int> _progress;
        public IProgress<int> Progress
        {
            get => _progress;
            set
            {
                _progressEvent?.Dispose();
                _progress = value as Progress<int>;
                if( _progress is null)
                {
                    return;
                }
                _progressEvent = Observable.FromEventPattern<int>(
                    _progress, nameof(Progress<int>.ProgressChanged)).
                    Subscribe(t => 
                    {
                        ProgressValue = Convert.ToDouble(t.EventArgs);
                        System.Diagnostics.Debug.WriteLine($"[SCR] {nameof(Progress)} = {t.EventArgs}");
                    });
                OnPropertyChanged(nameof(Progress));
            }
        }

        public bool IsPercentTextVisible { get; set; }

        public ProgressModalOption(Progress<int> progress)
        {
            Progress = progress;
        }

        public void Dispose()
        {
            _progressEvent?.Dispose();
        }
    }
}
