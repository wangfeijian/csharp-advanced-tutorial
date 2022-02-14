using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SosoVision.Extensions;

namespace SosoVision.Common
{
    public class ConfigureService : IConfigureService
    {
        public SerializationData SerializationData { get; set; }

        public ConfigureService()
        {
            Configure();
        }

        public void Configure(bool isSave = false)
        {
            if (File.Exists("config.json"))
            {
                SerializationData = JsonConvert.DeserializeObject<SerializationData>(File.ReadAllText("config.json"));
            }
        }
    }
}
