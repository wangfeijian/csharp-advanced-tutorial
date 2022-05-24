using System;
using System.Net.Sockets;
using System.Threading;

namespace OmronFinsTCP.Net
{
    /// <summary>
    /// PLC Fins协议实现类（欧姆龙）
    /// </summary>
    public class EtherNetPlc
    {
        /// <summary>
        /// PLC节点号，调试方法，一般不需要使用
        /// </summary>
        public string PlcNode => BasicClass.PlcNode.ToString();

        /// <summary>
        /// PC节点号，调试方法，一般不需要使用
        /// </summary>
        public string PcNode => BasicClass.PcNode.ToString();

        /// <summary>
        /// 实例化PLC操作对象
        /// </summary>
        public EtherNetPlc()
        {
            BasicClass.Client = new TcpClient();
        }

        /// <summary>
        /// 与PLC建立TCP连接
        /// </summary>
        /// <param name="ip">PLC的IP地址</param>
        /// <param name="port">端口号，默认9600</param>
        /// <param name="timeOut">超时时间，默认3000毫秒</param>
        /// <returns></returns>
        public short Link(string ip, short port, short timeOut = 3000)
        {
            //连接超时
            if (!BasicClass.PingCheck(ip, timeOut))
                return -1;

            BasicClass.Client.Connect(ip, port);
            BasicClass.Stream = BasicClass.Client.GetStream();
            Thread.Sleep(10);

            if (BasicClass.SendData(FinsClass.HandShake()) != 0)
                return -1;

            //开始读取返回信号
            byte[] buffer = new byte[24];
            if (BasicClass.ReceiveData(buffer) != 0)
                return -1;

            if (buffer[15] != 0) //TODO:这里的15号是不是ERR信息暂时不能完全肯定
                return -1;

            BasicClass.PcNode = buffer[19];
            BasicClass.PlcNode = buffer[23];
            return 0;
        }

        /// <summary>
        /// 关闭PLC操作对象的TCP连接
        /// </summary>
        /// <returns></returns>
        public short Close()
        {
            try
            {
                BasicClass.Stream.Close();
                BasicClass.Client.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 读值方法（多个连续值）
        /// </summary>
        /// <param name="mr">地址类型枚举</param>
        /// <param name="ch">起始地址</param>
        /// <param name="cnt">地址个数</param>
        /// <param name="reData">返回值</param>
        /// <returns></returns>
        public short ReadWords(PlcMemory mr, short ch, short cnt, out short[] reData)
        {
            reData = new short[cnt];//储存读取到的数据
            int num = 30 + cnt * 2;//接收数据(Text)的长度,字节数
            byte[] buffer = new byte[num];//用于接收数据的缓存区大小
            byte[] array = FinsClass.FinsCmd(RorW.Read, mr, MemoryType.Word, ch, 00, cnt);

            if (BasicClass.SendData(array) != 0) return -1;

            if (BasicClass.ReceiveData(buffer) != 0) return -1;

            //命令返回成功，继续查询是否有错误码，然后在读取数据
            bool succeed = true;
            if (buffer[11] == 3)
                succeed = ErrorCode.CheckHeadError(buffer[15]);

            if (!succeed) return -1;

            //endcode为fins指令的返回错误码
            if (!ErrorCode.CheckEndCode(buffer[28], buffer[29])) return -1;

            //完全正确的返回，开始读取返回的具体数值
            for (int i = 0; i < cnt; i++)
            {
                //返回的数据从第30字节开始储存的,
                //PLC每个字占用两个字节，且是高位在前，这和微软的默认低位在前不同
                //因此无法直接使用，reData[i] = BitConverter.ToInt16(buffer, 30 + i * 2);
                //先交换了高低位的位置，然后再使用BitConverter.ToInt16转换
                byte[] temp = { buffer[30 + i * 2 + 1], buffer[30 + i * 2] };
                reData[i] = BitConverter.ToInt16(temp, 0);
            }
            return 0;

        }

        /// <summary>
        /// 读值字符串
        /// </summary>
        /// <param name="mr">地址类型枚举</param>
        /// <param name="ch">起始地址</param>
        /// <param name="cnt">地址个数</param>
        /// <param name="reData">返回值</param>
        /// <returns></returns>
        public short ReadString(PlcMemory mr, short ch, short cnt, out string reData)
        {
            reData = "";
            byte[] reDatabyte = new byte[cnt * 2];//储存读取到的数据
            int num = 30 + cnt * 2;//接收数据(Text)的长度,字节数
            byte[] buffer = new byte[num];//用于接收数据的缓存区大小
            byte[] array = FinsClass.FinsCmd(RorW.Read, mr, MemoryType.Word, ch, 00, cnt);
            if (BasicClass.SendData(array) != 0) return -1;

            if (BasicClass.ReceiveData(buffer) != 0) return -1;

            //命令返回成功，继续查询是否有错误码，然后在读取数据
            bool succeed = true;
            if (buffer[11] == 3)
                succeed = ErrorCode.CheckHeadError(buffer[15]);
            if (!succeed) return -1;

            //endcode为fins指令的返回错误码
            if (!ErrorCode.CheckEndCode(buffer[28], buffer[29])) return -1;

            //完全正确的返回，开始读取返回的具体数值
            for (int i = 0; i < cnt; i++)
            {
                //返回的数据从第30字节开始储存的,
                //PLC每个字占用两个字节，且是高位在前，这和微软的默认低位在前不同
                //因此无法直接使用，reData[i] = BitConverter.ToInt16(buffer, 30 + i * 2);
                //先交换了高低位的位置，然后再使用BitConverter.ToInt16转换
                byte[] temp = { buffer[30 + i * 2 + 1], buffer[30 + i * 2] };
                reDatabyte[i * 2]     = temp[0];
                reDatabyte[i * 2 + 1] = temp[1];
            }

            reData = System.Text.Encoding.UTF8.GetString(reDatabyte);
            return 0;

        }

        /// <summary>
        /// 读单个字方法
        /// </summary>
        /// <param name="mr"></param>
        /// <param name="ch"></param>
        /// <param name="reData"></param>
        /// <returns></returns>
        public short ReadWord(PlcMemory mr, short ch, out short reData)
        {
            short[] temp;
            reData = new short();
            short re = ReadWords(mr, ch, 1, out temp);
            if (re != 0)
                return -1;
            reData = temp[0];
            return 0;
        }

        /// <summary>
        /// 写值方法（多个连续值）
        /// </summary>
        /// <param name="mr">地址类型枚举</param>
        /// <param name="ch">起始地址</param>
        /// <param name="cnt">地址个数</param>
        /// <param name="inData">写入值</param>
        /// <returns></returns>
        public short WriteWords(PlcMemory mr, short ch, short cnt, short[] inData)
        {
            byte[] buffer = new byte[30];
            byte[] arrayhead = FinsClass.FinsCmd(RorW.Write, mr, MemoryType.Word, ch, 00, cnt);//前34字节和读指令基本一直，还需要拼接下面的输入数据数组
            byte[] wdata = new byte[cnt * 2];
            //转换写入值到wdata数组
            for (int i = 0; i < cnt; i++)
            {
                byte[] temp = BitConverter.GetBytes(inData[i]);
                wdata[i * 2] = temp[1];//转换为PLC的高位在前储存方式
                wdata[i * 2 + 1] = temp[0];
            }

            //拼接写入数组
            byte[] array = new byte[cnt * 2 + 34];
            arrayhead.CopyTo(array, 0);
            wdata.CopyTo(array, 34);
            if (BasicClass.SendData(array) != 0) return -1;

            if (BasicClass.ReceiveData(buffer) != 0) return -1;

            //命令返回成功，继续查询是否有错误码，然后在读取数据
            bool succeed = true;
            if (buffer[11] == 3)
                succeed = ErrorCode.CheckHeadError(buffer[15]);
            if (!succeed) return -1;

            //endcode为fins指令的返回错误码
            if (ErrorCode.CheckEndCode(buffer[28], buffer[29]))
            {
                //完全正确的返回0
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// 写值方法（多个连续值）
        /// </summary>
        /// <param name="mr">地址类型枚举</param>
        /// <param name="ch">起始地址</param>
        /// <param name="inData">写入值</param>
        /// <returns></returns>
        public short WriteString(PlcMemory mr, short ch, string inData)
        {
            byte[] buffer = new byte[30];
            byte[] arrayhead = FinsClass.FinsCmd(RorW.Write, mr, MemoryType.Word, ch, 00, (short)inData.Length);//前34字节和读指令基本一直，还需要拼接下面的输入数据数组
            byte[] wdata = System.Text.Encoding.UTF8.GetBytes(inData);

            // PLC中字存储的高低位跟windows中相反，因此需要交换高低位
            for (int i = 0; i < wdata.Length; i = i + 2)
            {
                if (i == wdata.Length - 1)
                {
                    break;
                }
                byte temp = wdata[i];
                wdata[i] = wdata[i + 1];
                wdata[i + 1] = temp;
            }

            byte[] rdata = new byte[wdata.Length];

            // 将需要发送的数组复制到待发送的数组中，如果长度为单数，需要在最后一位增加，并与0交换位置
            if (wdata.Length % 2 == 1)
            {
                rdata = new byte[wdata.Length + 1];
                for (int i = 0; i < wdata.Length; i++)
                {
                    rdata[i] = wdata[i];
                }

                rdata[wdata.Length] = wdata[wdata.Length - 1];
                rdata[wdata.Length - 1] = 0;
            }

            //拼接写入数组
            byte[] array = new byte[wdata.Length * 2 + 34];
            arrayhead.CopyTo(array, 0);

           // 根据字符串的长度是否为单数来指定复制到发送字节数组中的数组
            if (wdata.Length % 2 == 1)
                rdata.CopyTo(array, 34);
            else
                wdata.CopyTo(array, 34);

            if (BasicClass.SendData(array) != 0) return -1;

            if (BasicClass.ReceiveData(buffer) != 0) return -1;

            //命令返回成功，继续查询是否有错误码，然后在读取数据
            bool succeed = true;
            if (buffer[11] == 3)
                succeed = ErrorCode.CheckHeadError(buffer[15]);
            if (!succeed) return -1;

            //endcode为fins指令的返回错误码
            if (ErrorCode.CheckEndCode(buffer[28], buffer[29]))
            {
                //完全正确的返回0
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// 写单个字方法
        /// </summary>
        /// <param name="mr"></param>
        /// <param name="ch"></param>
        /// <param name="inData"></param>
        /// <returns></returns>
        public short WriteWord(PlcMemory mr, short ch, short inData)
        {
            short[] temp = { inData };
            short re = WriteWords(mr, ch, 1, temp);
            if (re != 0)
                return -1;

            return 0;
        }

        /// <summary>
        /// 读值方法-按字读取WR存储区
        /// </summary>
        /// <param name="mr">地址类型枚举</param>
        /// <param name="ch">地址W000，W可以省略，直接填地址</param>
        /// <param name="bs">返回这个地址中16位所有的状态，按十进制输出，可以在调用后转为二进制</param>
        /// <returns></returns>
        public short GetBitState(PlcMemory mr, string ch, out short bs)
        {
            bs = new short();
            byte[] buffer = new byte[32];//用于接收数据的缓存区大小
            short cnInt = short.Parse(ch);

            // 按字节读取WR区的位状态
            byte[] array = FinsClass.FinsCmd(RorW.Read, mr, MemoryType.Word, cnInt, 00, 1);

            if (BasicClass.SendData(array) != 0) return -1;

            if (BasicClass.ReceiveData(buffer) != 0) return -1;

            //命令返回成功，继续查询是否有错误码，然后在读取数据
            bool succeed = true;

            if (buffer[11] == 3)
                succeed = ErrorCode.CheckHeadError(buffer[15]);
            if (!succeed) return -1;

            //endcode为fins指令的返回错误码
            if (!ErrorCode.CheckEndCode(buffer[28], buffer[29])) return -1;

            //完全正确的返回，开始读取返回的具体数值
            byte[] temp = { buffer[31], buffer[30] };

            bs = BitConverter.ToInt16(temp, 0);
            return 0;

        }

        /// <summary>
        /// 写值方法-按位bit（单个）
        /// 一般在实际项目中都不会操作PLC的位地址
        /// 所以，此方法不建议使用
        /// </summary>
        /// <param name="mr">地址类型枚举</param>
        /// <param name="ch">地址000.00</param>
        /// <param name="bs">开关状态枚举EtherNetPLC.BitState，0/1</param>
        /// <returns></returns>
        [Obsolete]
        public short SetBitState(PlcMemory mr, string ch, BitState bs)
        {
            byte[] buffer = new byte[30];
            short cnInt = short.Parse(ch.Split('.')[0]);
            short cnBit = short.Parse(ch.Split('.')[1]);
            byte[] arrayhead = FinsClass.FinsCmd(RorW.Write, mr, MemoryType.Bit, cnInt, cnBit, 1);
            byte[] array = new byte[35];
            arrayhead.CopyTo(array, 0);
            array[34] = (byte)bs;
            if (BasicClass.SendData(array) != 0) return -1;

            if (BasicClass.ReceiveData(buffer) != 0) return -1;

            //命令返回成功，继续查询是否有错误码，然后在读取数据
            bool succeed = true;
            if (buffer[11] == 3)
                succeed = ErrorCode.CheckHeadError(buffer[15]);
            if (!succeed) return -1;

            //endcode为fins指令的返回错误码
            if (ErrorCode.CheckEndCode(buffer[28], buffer[29]))
            {
                //完全正确的返回0
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// 读一个浮点数的方法，单精度，在PLC中占两个字
        /// </summary>
        /// <param name="mr">地址类型枚举</param>
        /// <param name="ch">起始地址，会读取两个连续的地址，因为单精度在PLC中占两个字</param>
        /// <param name="reData">返回一个float型</param>
        /// <returns></returns>
        public short ReadReal(PlcMemory mr, short ch, out float reData)
        {
            reData = new float();
            int num = 30 + 2 * 2;//接收数据(Text)的长度,字节数
            byte[] buffer = new byte[num];//用于接收数据的缓存区大小
            byte[] array = FinsClass.FinsCmd(RorW.Read, mr, MemoryType.Word, ch, 00, 2);

            if (BasicClass.SendData(array) != 0) return -1;

            if (BasicClass.ReceiveData(buffer) != 0) return -1;

            //命令返回成功，继续查询是否有错误码，然后在读取数据
            bool succeed = true;
            if (buffer[11] == 3)
                succeed = ErrorCode.CheckHeadError(buffer[15]);
            if (!succeed) return -1;

            //endcode为fins指令的返回错误码
            if (!ErrorCode.CheckEndCode(buffer[28], buffer[29])) return -1;

            //完全正确的返回，开始读取返回的具体数值
            byte[] temp = { buffer[30 + 1], buffer[30], buffer[30 + 3], buffer[30 + 2] };
            reData = BitConverter.ToSingle(temp, 0);
            return 0;
        }
    }
}