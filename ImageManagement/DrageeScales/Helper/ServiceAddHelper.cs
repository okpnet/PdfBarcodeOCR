using DrageeScales.Views.Dtos;
using ImageManagement.Service;
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
            services.AddSingleton<PdfImageAdapterService>();
            services.AddTransient<MainWindowModel>();//VM
            return services;
        }
    }
}
