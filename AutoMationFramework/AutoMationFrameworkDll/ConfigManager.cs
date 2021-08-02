/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    System config manager class              *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTools.Manager;
using CommonTools.Servers;

namespace AutoMationFrameworkDll
{
    /// <summary>
    /// 配置文件管理类
    /// </summary>
    public class ConfigManager : SingletonPattern<ConfigManager>
    {
        /// <summary>
        /// 加载系统配置
        /// </summary>
        /// <returns></returns>
        public bool LoadConfigFile(string fileName, IBuildConfig buildConfig)
        {
            bool runBool = RunInforManager.GetInstance().ReadXmlConfig(fileName);
            IoManager.GetInstance().ReadCfgFromFile(fileName, buildConfig);
            MotionManager.GetInstance().ReadCardFromCfg();
            return runBool;
        }
    }
}
