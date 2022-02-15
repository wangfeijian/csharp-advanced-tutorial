using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prism.Ioc;
using SosoVision.Extensions;
using SosoVision.ViewModels;
using SosoVision.Views;

namespace SosoVision.Common
{
    public class ConfigureService : IConfigureService
    {
        private readonly IContainerProvider _containerProvider;
        public SerializationData SerializationData { get; set; }

        public ConfigureService(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
            Configure();
        }

        public void Configure(bool isSave = false)
        {
            if (!isSave)
            {
                SerializationData = File.Exists("config/config.json") ?
                    JsonConvert.DeserializeObject<SerializationData>(File.ReadAllText("config/config.json"))
                    : new SerializationData { ProcedureParams = new ObservableCollection<ProcedureParam>() };
            }
            else
            {
                File.WriteAllText("config/config.json", JsonConvert.SerializeObject(SerializationData));

                foreach (var param in SerializationData.ProcedureParams)
                {
                    string dir = $"config/Vision/{param.Name}";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    string fileName = $"{dir}/{param.Name}.json";

                    var view = _containerProvider.Resolve(typeof(VisionProcessView), param.Name) as VisionProcessView;
                    var viewModel = view?.DataContext as VisionProcessViewModel;
                    if (viewModel != null)
                    {
                        File.WriteAllText(fileName, JsonConvert.SerializeObject(viewModel));
                    }
                }
            }
        }
    }
}
