using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;

namespace DrageeScales.Views.Dtos
{
    public abstract class ViewModelBase
    {
        public IConfigService<AppSetting> ConfigService { get; }

        protected ViewModelBase(IConfigService<AppSetting> configService)
        {
            ConfigService = configService;
        }
    }
}
