using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DrageeScales.Shared.Dtos;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DrageeScales.Shared.Controls
{
    public sealed partial class Toast : UserControl
    {
        public ToastItemCollction Notifications { get; set; } = new();

        public Toast()
        {
            this.InitializeComponent();
        }

        private void OnToastLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid &&
                grid.FindName("InfoBarElement") is InfoBar infoBar &&
                infoBar.RenderTransform is TranslateTransform translate)
            {
                // スライドインアニメーション
                var slideAnim = new DoubleAnimation
                {
                    From = 50,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                var sb = new Storyboard();
                Storyboard.SetTarget(slideAnim, translate);
                Storyboard.SetTargetProperty(slideAnim, "Y");
                sb.Children.Add(slideAnim);
                sb.Begin();
            }
        }
        private void InfoBar_Closed(InfoBar sender, InfoBarClosedEventArgs args)
        {
            // 手動で閉じた場合も Dispose を呼び出して削除
            if (sender.DataContext is ToastItem toast)
            {
                toast.Dispose();
            }
        }
    }
}
