/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Communicate event                        *
*********************************************************************/
namespace Communicate
{
    /// <summary>
    /// 通讯事件基类
    /// </summary>
    public class Notify
    {
        /// <summary>
        /// 发送委托函数
        /// </summary>
        /// <param name="strLog"></param>
        public delegate void SendData(string strLog);
        /// <summary>
        /// 发送事件
        /// </summary>
        public event SendData DoSend;
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="strLog"></param>
        public void NotifyData(string strLog)
        {
                DoSend?.Invoke(strLog);
        }
        /// <summary>
        /// 返回类引用
        /// </summary>
        /// <returns></returns>
        public Notify GetObject()
        {
            return this;
        }
    }
}
