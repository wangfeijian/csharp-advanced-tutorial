using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStringConvertByte
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] temp = ReadMultiData(8).ToArray();
        }

        private static List<byte> ReadMultiData( ushort channel,string startAddr="03FC")
        {
            /*
             * 力准传感器modbus tcp发送字节数组如下
             *  事务处理标识符 协议标识符   长度  单元标识符   功能码 起始地址    数据长度
             *      0000        0000    0006    01        03    03FC      0002
             */
            string header = "0000000000060103";
            int length = channel * 2;
            string sendstr = header + startAddr + "00" + length.ToString("X02");

            //获取数据体字节数组，并计算CRC
            byte[] dataBytes = ASCIIStringToByte(sendstr, " ");

            //拼接数据体字节数组和CRC字节数组
            List<byte> listSendBytes = dataBytes.ToList();

            return listSendBytes;

        }
        #region 功能函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteData"></param>
        /// <returns></returns>
        protected static byte[] GetCRC16ex(byte[] byteData)
        {
            byte[] CRC = new byte[2];

            UInt16 wCrc = 0xFFFF;
            for (int i = 0; i < byteData.Length; i++)
            {
                wCrc ^= Convert.ToUInt16(byteData[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) == 1)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;//异或多项式
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }
            }

            CRC[1] = (byte)((wCrc & 0xFF00) >> 8);//高位在后
            CRC[0] = (byte)(wCrc & 0x00FF);       //低位在前
            return CRC;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected static byte[] GetCRC16(byte[] bytes)
        {
            ushort value;
            ushort crcData = 0xffff, In_value;
            int count = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                value = (ushort)bytes[i];
                crcData = (ushort)(Convert.ToInt32(value) ^ Convert.ToInt32(crcData));
                In_value = 0xA001;
                while (count < 8)
                {
                    if (Convert.ToInt32(crcData) % 2 == 1)//判断最低位是否为1
                    {
                        crcData -= 0x00001;
                        crcData = (ushort)(Convert.ToInt32(crcData) / 2);//右移一位
                        count++;//计数器加一
                        crcData = (ushort)(Convert.ToInt32(crcData) ^ Convert.ToInt32(In_value));//异或操作
                    }
                    else
                    {
                        crcData = (ushort)(Convert.ToInt32(crcData) / 2);//右移一位
                        count++;//计数器加一
                    }
                }
                count = 0;
            }

            byte[] crcBytes = new byte[2];
            crcBytes[0] = (byte)(crcData & 0x00FF);
            crcBytes[1] = (byte)((crcData & 0xFF00) >> 8);

            return crcBytes;
        }

        /// <summary>
        /// 16进制ASCII字符串转换为字节数组
        /// </summary>
        /// <param name="strASCII"></param>
        /// <returns></returns>
        protected static byte[] ASCIIStringToByte(string strASCII)
        {
            int nLastLen = 0;
            List<byte> list = new List<byte>();

            //判断是否存在Hex：123这样的16进制字符串，需特殊处理
            if (strASCII.Length % 2 == 1)
            {
                nLastLen = 1;
            }
            byte[] array = new byte[(strASCII.Length / 2) + nLastLen];

            string strHex = string.Empty;
            int nIndex = 0;
            for (int i = 0; i < strASCII.Length; i++)
            {
                strHex += strASCII[i];

                //如果是长度为2，或者是最后一个字符（此时strHex有可能为长度为1），则直接转换
                if (strHex.Length == 2 || i == strASCII.Length - 1)
                {
                    array[nIndex] = Convert.ToByte(strHex, 16);
                    strHex = "";
                    nIndex++;
                }
            }

            return array;
        }

        /// <summary>
        /// 16进制ASCII字符串转换为字节数组
        /// </summary>
        /// <param name="strASCII"></param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        protected static byte[] ASCIIStringToByte(string strASCII, string split)
        {
            strASCII = strASCII.Trim();
            string strTemp = strASCII.Replace(split, "");

            return ASCIIStringToByte(strTemp);
        }

        /// <summary>
        /// 字节数组转换为16进制ASCII字符串
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        protected static string ByteToASCIIString(byte[] array)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte data in array)
            {
                sb.Append(data.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 字节数组转换为16进制ASCII字符串
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        protected static string ByteToASCIIString(byte[] array, int startIndex, int length, char split = ' ')
        {
            StringBuilder sb = new StringBuilder();

            for (int i = startIndex; i < length; i++)
            {
                sb.Append(array[i].ToString("X2"));

                if (i < length - 1)
                {
                    sb.Append(split);
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}
