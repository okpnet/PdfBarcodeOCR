using System;
using System.Drawing;

namespace DrageeScales.Views.Dtos
{
    /// <summary>
    /// バーコードの読み取り結果
    /// </summary>
    public class BarcodeParameter
    {
        //読み取り中の例外
        readonly Exception? _lastException = null;
        
        public bool IsSucces { get; }

        public bool HasException => _lastException is not null;

        public string Value { get; }

        public bool IsTrimming { get; }

        public Rectangle Rectangles { get; }

        public Exception? LastException => _lastException;

        public static BarcodeParameter FromSuccess(string value, Rectangle rectangle, bool shredded = false) => new(null, true, value, shredded, rectangle);

        public static BarcodeParameter FromUnableRed() => new(null, false, string.Empty, false, new());

        public static BarcodeParameter FromException(Exception exception) => new(exception, false, string.Empty, false, new());

        private BarcodeParameter(Exception? lastException, bool isSucces, string value, bool isTrimming, Rectangle rectangles)
        {
            _lastException = lastException;
            IsSucces = isSucces;
            Value = value;
            IsTrimming = isTrimming;
            Rectangles = rectangles;
        }
    }
}
