using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace DrageeScales.Shared.Helper
{
    public static class DialogHelper
    {

        public static async Task<DialogHelperResultYesNo> FileOverWriteConfirmAsync(this XamlRoot xamlRoot,string fileName)
        {
            var dialog=new ContentDialog
            {
                Title = "上書きの確認",
                Content = $"ファイル'{fileName}'は存在します。上書きしますか？",
                PrimaryButtonText = "はい",
                CloseButtonText = "いいえ",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = xamlRoot
            };
            var result=await dialog.ShowAsync();

            return result switch
            {
                ContentDialogResult.Primary => DialogHelperResultYesNo.Yes,
                _ => DialogHelperResultYesNo.No
            };
        }

        public static async Task<DialogHelperResultYesNo> RemoveConfirmAsync(this XamlRoot xamlRoot, string title)
        {
            var dialog = new ContentDialog
            {
                Title = "削除の確認",
                Content = $"'{title}' を削除しますか？",
                PrimaryButtonText = "はい",
                CloseButtonText = "いいえ",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = xamlRoot
            };
            var result = await dialog.ShowAsync();

            return result switch
            {
                ContentDialogResult.Primary => DialogHelperResultYesNo.Yes,
                _ => DialogHelperResultYesNo.No
            };
        }
    }
}
