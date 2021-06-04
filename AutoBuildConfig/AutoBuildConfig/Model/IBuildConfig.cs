using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBuildConfig.ViewModel;

namespace AutoBuildConfig.Model
{
    public interface IBuildConfig
    {
        T LoadConfig<T>(string config);
        void SaveConfig<T>(T tCfg, string fileName);
        void SaveAsConfig<T>(T tCfg);
        T LoadConfigFromFile<T>();
    }
}
