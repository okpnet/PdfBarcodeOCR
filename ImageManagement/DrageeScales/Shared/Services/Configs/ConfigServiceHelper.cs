using DrageeScales.Shared.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.Configs
{
    public static class ConfigServiceHelper
    {
        public static IServiceCollection AddConfigService<T>(this IServiceCollection services,Func<IServiceProvider,IConfigModelFacade<T>> factory)
        {
            
            services.AddSingleton<IConfigService<T>, ConfigService<T>>(provider =>
            {
                var facade = factory.Invoke(provider);
                var logger = provider.GetService(typeof(ILogger<ConfigService<T>>)) as ILogger<ConfigService<T>>;
                return new ConfigService<T>(logger, facade);
            });
            return services;
        }
    }
}
