using DrageeScales.Presentation.Services;
using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;
using DrageeScales.Views.Dtos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            services.AddSingleton<PdfImageAdapterService>();
            services.AddTransient<MainWindowModel>();//VM
            services.AddConfigService(()=>new LocalFilleConfigModelFacade());
            return services;
        }
    }
}
