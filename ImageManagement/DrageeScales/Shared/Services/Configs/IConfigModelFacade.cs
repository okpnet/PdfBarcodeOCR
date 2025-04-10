﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.Configs
{
    public interface IConfigModelFacade<T>
    {
        void Save(T data);

        T Load();
    }
}
