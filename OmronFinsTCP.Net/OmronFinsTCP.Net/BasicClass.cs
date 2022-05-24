using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace OmronFinsTCP.Net
{
    class BasicClass
    {
        internal static TcpClient Client;
        internal static NetworkStream Stream;
        internal static byte PcNode, PlcNode;

        /// <summary>
        /// 检查PLC链接状况
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        internal static bool PingCheck(string ip,int timeOut)
        {
            Ping ping = new Ping();
            PingReply pr = ping.Send(ip, timeOut);

            return pr != null && pr.Status == IPStatus.Success;
        }

        /// <summary>
        /// 内部方法，发送数据
        /// </summary>
        /// <param name="sd">发送数据给plc</param>
        /// <returns></returns>
        internal static short SendData(byte[] sd)
        {
            try
            {
                Stream.Write(sd, 0, sd.Length);
                return 0;
            }
            catch(Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 内部方法，接收数据
        /// </summary>
        /// <param name="rd">从tcp buffer中接收的数据</param>
        /// <returns></returns>
        internal static short ReceiveData(byte[] rd)
        {
            try
            {
                int index = 0;
                do
                {
                    int len = Stream.Read(rd, index, rd.Length - index);
                    if (len == 0)
                        return -1;//这里控制读取不到数据时就跳出,网络异常断开，数据读取不完整。
                    index += len;
                } while (index < rd.Length);
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
