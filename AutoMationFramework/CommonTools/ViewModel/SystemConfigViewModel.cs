using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTools.Model;
using CommonTools.Servers;
using GalaSoft.MvvmLight;

namespace CommonTools.ViewModel
{
    public class SystemConfigViewModel : ViewModelBase
    {
        private readonly IBuildConfig _bulidConfig;

        public string FileDir { get; set; }

        private SystemCfg systemConfig;

        public SystemCfg SystemConfig
        {
            get { return systemConfig; }
            set { Set(ref systemConfig, value); }
        }

        public SystemConfigViewModel(IBuildConfig buildConfig, string dir)
        {
            FileDir = AppDomain.CurrentDomain.BaseDirectory + "Config\\" + dir;
            string fileName = FileDir + "\\systemCfg";
            _bulidConfig = buildConfig;
            SystemConfig = _bulidConfig.LoadConfig<SystemCfg>(fileName);
        }
    }
}
