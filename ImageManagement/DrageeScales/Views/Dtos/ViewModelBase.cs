﻿using DrageeScales.Core;
using DrageeScales.Shared.Dtos;
using DrageeScales.Shared.Services.Configs;

namespace DrageeScales.Views.Dtos
{
    public abstract class ViewModelBase: NotifyPropertyChangedBase
    {
        public IConfigService<AppSetting> ConfigService { get; }

        protected ViewModelBase(IConfigService<AppSetting> configService)
        {
            ConfigService = configService;
        }
    }
}
