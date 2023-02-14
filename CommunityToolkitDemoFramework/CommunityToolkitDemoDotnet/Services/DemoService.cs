#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：DESKTOP-ARJBVM7
 * 公司名称：wangfeijian
 * 命名空间：CommunityToolkitDemoDotnet.Services
 * 唯一标识：2a55804d-1c92-48dc-9842-2749921fc4b4
 * 文件名：DemoService
 * 当前用户域：DESKTOP-ARJBVM7
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2023/2/14 13:32:40
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

namespace CommunityToolkitDemoDotnet.Services
{
    public class DemoService : IDemoService
    {
        public DemoService()
        {

        }

        public string GetInfo()
        {
            return "Demo Test";
        }
    }
}