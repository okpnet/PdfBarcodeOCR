using DrageeScales.Shared.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.Configs
{
    public class LocalFilleConfigModelFacade : IConfigModelFacade<AppSetting>
    {
        string _fileName = "conf.json";
        string _saveDir = string.Empty;

        public string FileName => System.IO.Path.Combine(_saveDir, _fileName);

        public LocalFilleConfigModelFacade()
        {
            _saveDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public AppSetting Load()
        {
            if (!System.IO.File.Exists(FileName))
            {
                var result = new AppSetting();
                Save(result);
                return result;
            }

            using var stresm=new System.IO.StreamReader(FileName,Encoding.UTF8);
            var buffer=stresm.ReadToEnd();
            if(buffer is (null or ""))
            {
                return new AppSetting();
            }

            var settings = System.Text.Json.JsonSerializer.Deserialize<AppSetting>(buffer);

            if(settings is null)
            {
                System.IO.File.Delete(FileName);
                settings = new AppSetting();
            }
            Save(settings);
            return settings;
        }

        public void Save(AppSetting data)
        {
            using var stream=new System.IO.StreamWriter(FileName,false,Encoding.UTF8);
            var buffer = System.Text.Json.JsonSerializer.Serialize(data,GetOption());
            stream.Write(buffer);
        }

        System.Text.Json.JsonSerializerOptions GetOption()
        {
            return new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
        }
    }
}
