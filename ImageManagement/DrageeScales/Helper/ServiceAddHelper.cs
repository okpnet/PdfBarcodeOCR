using DrageeScales.Presentation.Services;
using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;
using DrageeScales.Shared.Services.NetspakleUpdate;
using DrageeScales.Views.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddConfigService(()=>new LocalFilleConfigModelFacade());

            services.AddSingleton(provider =>
            {
                var app = provider.GetRequiredService<AppService>();
                var logger=provider.GetService<ILogger<NetSparkleService>>();
                var keypath = new FileInfo(AppDefine.ED25529_FILE_PATH);
                var url = new Uri(AppDefine.APPCAST_URL);
                var service = new NetSparkleService(logger, () => app.Apps.Exit(), keypath, url);
                return service;
            });

            return services;
        }
    }
}
