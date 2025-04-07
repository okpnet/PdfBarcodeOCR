using BarcodeImageReader.Shared;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.Windows.Compatibility;

namespace BarcodeImageReader
{
    public static class BarcodeReader
    {
        static int RETRY = 10;
        public static IBarcodeItem ReadFromFile(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath))
            {
                return BarcodeItem.FromUnableToRead();
            }
            using var image=Image.FromFile(imagePath);
            if(image is null)
            {
                return BarcodeItem.FromUnableToRead();
            }
            return ReadFromImage(image);
        }

        public static IBarcodeItem ReadFromImage(System.Drawing.Image image)
        {
            if(image is Bitmap bitmap)
            {
                return ReadFromBitmap(bitmap);
            }

            using var memoryStream= new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            using var bmap = new Bitmap(memoryStream);
            return ReadFromBitmap(bmap);
        }

        public static IBarcodeItem ReadFromBitmap(Bitmap bitmap)
        {
            try
            {
                var rebitmap = _correction(bitmap);
                if(!new ZXing.BarcodeReaderGeneric().TryGetValue(RETRY,100,out var reader,() => new ZXing.BarcodeReaderGeneric()))
                {
                    return BarcodeItem.FromUnableToRead();
                }

                if(!new BitmapLuminanceSource(rebitmap).TryGetValue(RETRY,100,out var source,()=> new BitmapLuminanceSource(rebitmap)))
                {
                    return BarcodeItem.FromUnableToRead();
                }
                reader.Options.TryInverted = true;

                var result = reader.Decode(source);//NullReferenceException
                return result is null ? BarcodeItem.FromUnableToRead() : BarcodeItem.FromResult(new BarcodeResult(result));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return BarcodeItem.FromException(new BarcodeReadException(ex));
            }
        }

        internal static Bitmap _correction(Bitmap img)
        {
            int width = img.Width;
            int height = img.Height;

            // 出力用のビットマップ
            var dstImg = new Bitmap(width, height);

            // ピクセルに直接アクセスする
            var bmpread = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var bmpwrite = dstImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int blackness = 255;
            int whiteness = -1;

            for (int i = 0; i < (width * height * 3);)
            {
                var r = Marshal.ReadByte(bmpread.Scan0, i++);
                var g = Marshal.ReadByte(bmpread.Scan0, i++);
                var b = Marshal.ReadByte(bmpread.Scan0, i++);

                int ave = (int)new List<int> { r, g, b }.Average();

                // 最も暗い色を抜き出して「黒」を定義
                blackness = Math.Min(blackness, ave);

                // 128以上の値を平均して「白」を定義
                if (ave >= 128)
                {
                    if (whiteness == -1)
                        whiteness = ave;
                    else
                        whiteness = (whiteness + ave) / 2;
                }
            }

            // 暗さの調整
            int darkness = 255 - whiteness;
            whiteness -= darkness;

            // 明るさの調整
            blackness += blackness / 2;

            // 黒と白の数値の差を取り閾値を作る
            int adjust = (whiteness - blackness) / 3;

            blackness += adjust;
            whiteness -= adjust;

            int diff = whiteness - blackness;

            for (int i = 0, j = 0; i < (width * height * 3);)
            {
                var r = Marshal.ReadByte(bmpread.Scan0, i++);
                var g = Marshal.ReadByte(bmpread.Scan0, i++);
                var b = Marshal.ReadByte(bmpread.Scan0, i++);

                // 黒の閾値以下ならそのピクセルは黒
                if (r < blackness ||
                    g < blackness ||
                    b < blackness)
                {
                    Marshal.WriteByte(bmpwrite.Scan0, j++, 0);
                    Marshal.WriteByte(bmpwrite.Scan0, j++, 0);
                    Marshal.WriteByte(bmpwrite.Scan0, j++, 0);
                }
                // 白の閾値以上ならそのピクセルは白
                else if (r > whiteness ||
                            g > whiteness ||
                            b > whiteness)
                {
                    Marshal.WriteByte(bmpwrite.Scan0, j++, 255);
                    Marshal.WriteByte(bmpwrite.Scan0, j++, 255);
                    Marshal.WriteByte(bmpwrite.Scan0, j++, 255);
                }
                else
                {
                    double v = new List<int> { r, g, b }.Average();
                    v -= diff;
                    v *= 3;

                    v = Math.Min(255, Math.Max(0, v));

                    Marshal.WriteByte(bmpwrite.Scan0, j++, (byte)v);
                    Marshal.WriteByte(bmpwrite.Scan0, j++, (byte)v);
                    Marshal.WriteByte(bmpwrite.Scan0, j++, (byte)v);
                }
            }

            dstImg.UnlockBits(bmpwrite);
            img.UnlockBits(bmpread);

            return dstImg;
        }
    }
}
