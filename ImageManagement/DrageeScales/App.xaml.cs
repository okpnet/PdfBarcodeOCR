using DrageeScales.Helper;
using DrageeScales.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DrageeScales
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        private IHost _host;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            var logDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);  // フォルダがなければ作成
            //ログサービス
            Serilog.Log.Logger = new LoggerConfiguration().
                    Enrich.FromLogContext().
                    WriteTo.Debug().
                    MinimumLevel.Verbose().
                    MinimumLevel.Information().
                    WriteTo.File(System.IO.Path.Combine(logDirectory,"log.txt"), rollingInterval: RollingInterval.Day).
                    CreateLogger();
            //サービスホスト
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services=services.SetService();
                })
                .Build();

            Services = _host.Services;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = _host.Services.GetRequiredService<MainWindow>();
            var app=_host.Services.GetRequiredService<AppService>();
            app.Apps = this;
            m_window.Activate();
            using var stream = new StreamReader("dragee_key.pem");
            var buffer = stream.ReadToEnd();
            stream.Close();
        }

        private Window m_window;

        public Window MainWindow=>m_window;
    }
}
