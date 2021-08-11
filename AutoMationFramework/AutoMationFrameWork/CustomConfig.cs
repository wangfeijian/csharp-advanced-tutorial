using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutoMationFrameworkDll;
using AutoMationFrameworkSystemDll;
using AutoMationFrameWork.View;
using CommonTools.Tools;

namespace AutoMationFrameWork
{
    static class CustomConfig
    {

        public static bool ConfigApp(string ConfigDir)
        {
            try
            {
                //ErrorCodeMgr.GetInstance();
                RunInforManager.GetInstance();

                if (ConfigManager.GetInstance().LoadConfigFile(ConfigDir))
                {
                    StationManager.GetInstance().LoadPointFromCfg();
                    return true;
                }

                MessageBox.Show(LocationServices.GetLang("SysInitError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(LocationServices.GetLang("SysInitError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        public static void AddStation()
        {
            // 当配置的是英文界面就需要将站名改成英文
            StationManager.GetInstance().AddStation(new StationTemplateControl(), new StationBase("TurntableStation"));
            StationManager.GetInstance().AddStation(new StationTemplateControl(), new StationBase("CCDInspectOneStation"));
            StationManager.GetInstance().AddStation(new StationTemplateControl(), new StationBase("CCDInspectTwoStation"));
            StationManager.GetInstance().AddStation(new StationTemplateControl(), new StationBase("LaserOneStation"));
            StationManager.GetInstance().AddStation(new StationTemplateControl(), new StationBase("LaserTwoStation"));
            StationManager.GetInstance().AddStation(new StationTemplateControl(), new StationBase("LaserThreeStation"));
        }
    }
}
