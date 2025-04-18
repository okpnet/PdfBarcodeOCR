using AppUpdater;
using DrageeScales.Presentation.Services;
using DrageeScales.Shared.Services.Configs;
using DrageeScales.Views.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace DrageeScales.Helper
{
    public static class ServiceAddHelper
    {
        public static IServiceCollection SetService(this IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddLogging(loggingBuilder =>loggingBuilder.AddSerilog(dispose: true));
            services.AddSingleton<PdfImageAdapterService>();
            services.AddSingleton<PdfFileItemCollection>();
            services.AddTransient<MainWindowModel>();//VM
            services.AddSingleton<AppService>();
            services.AddConfigService((provider)=>{
                var logger=provider.GetService<ILogger<LocalFilleConfigModelFacade>>();
                return new LocalFilleConfigModelFacade(logger);
                ;
            });

            services.AddSingleton((provider) =>{
                var logger = provider.GetService<ILogger<AppUpdateService>>();
                var updater = AppUpdateService.CreateAppUpdateService(
                    "7PPXEsJachIk8dtPoHk+NL8Hi2PfqNLtbSh+RSlAg2g=",
                    new Uri("https://github.com/okpnet/PdfBarcodeOCR/releases/download/0.0.0/appcast.xml"),
                    (() => App.Current.Exit()),
                    logger);

                return updater;
            });

            return services;
        }
    }
}
