﻿using QuestPDF.Fluent;
using System.Drawing;
using System.Drawing.Imaging;

namespace PdfConverer
{
    public static class ImageToPdfConverter
    {
        /// <summary>
        /// イメージをシングルページPDFに保存
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filename"></param>
        /// <param name="dpi">300DPIで保存した画像は100が適切</param>
        public static void SaveFitImageToPdf(this Image image, string filename,float dpi=100.0f)
        {
            var width=image.Width.ChangeSize(dpi);
            var height=image.Height.ChangeSize(dpi);
            using var memstream = new MemoryStream();
            image.Save(memstream, ImageFormat.Bmp);
            QuestSetting();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(width, height);
                    page.Content().Image(memstream.ToArray());
                });
            }).GeneratePdf(filename);
        }
        /// <summary>
        /// ミリ変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dpi"></param>
        /// <returns></returns>
        internal static float ChangeSize(this int value, float dpi) => value / dpi * 25.4f;
        /// <summary>
        /// ライセンス
        /// </summary>
        internal static void QuestSetting()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }
    }
}
