/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-27                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Read write config file interface         *
*********************************************************************/

namespace AutoMationFrameWork.Servers
{
    public interface IBuildConfig
    {
        T LoadConfig<T>(string config);
        void SaveConfig<T>(T tCfg, string fileName);
        void SaveAsConfig<T>(T tCfg);
        T LoadConfigFromFile<T>(string config);
    }
}
