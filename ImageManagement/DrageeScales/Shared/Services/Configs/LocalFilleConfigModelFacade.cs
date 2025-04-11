using DrageeScales.Shared.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.Configs
{
    public class LocalFilleConfigModelFacade : IConfigModelFacade<AppSetting>
    {
        readonly ILogger? _logger;
        string _fileName = "conf.json";
        string _saveDir = string.Empty;

        public string FileName => System.IO.Path.Combine(_saveDir, _fileName);

        public LocalFilleConfigModelFacade()
        {
            _saveDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        }

        public LocalFilleConfigModelFacade(ILogger logger):this()
        {
            _logger = logger;
        }


        public AppSetting Load()
        {
            try
            {
                _logger?.LogInformation("LOAD CONFIGURATION FROM FILE: {path}", FileName);
                if (!System.IO.File.Exists(FileName))
                {
                    var result = new AppSetting();
                    Save(result);
                    _logger?.LogInformation("NO FILE.CREATE NEEW CONFIG.");
                    return result;
                }


                using var stresm = new System.IO.StreamReader(FileName, Encoding.UTF8);
                var buffer = stresm.ReadToEnd();
                stresm.Close();
                _logger?.LogInformation("READ BUFFER FROM FILE.{buffer}", buffer);
                var settings = buffer is (null or "") ? new() : System.Text.Json.JsonSerializer.Deserialize<AppSetting>(buffer, GetOption());

                if (settings is null)
                {
                    settings = new AppSetting();
                }
                Save(settings);
                _logger?.LogInformation("LOADED FILE.{item}", buffer);
                return settings;
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex,"CONFIG SERVICE EXCEPTION");
                throw;
            }

        }

        public void Save(AppSetting data)
        {
            using var stream=new System.IO.StreamWriter(FileName,false,Encoding.UTF8);
            var buffer = System.Text.Json.JsonSerializer.Serialize(data,GetOption());
            _logger?.LogInformation("SAVED CONFIG FILE.{item}", buffer);
            stream.Write(buffer);
        }

        System.Text.Json.JsonSerializerOptions GetOption()
        {
            return new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                TypeInfoResolver= new DefaultJsonTypeInfoResolver()
            };
        }
    }
}
