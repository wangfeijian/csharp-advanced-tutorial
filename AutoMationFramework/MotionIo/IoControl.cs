/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-27                               *
*                                                                    *
*           ModifyTime:     2021-07-28                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Io control base class                    *
*********************************************************************/
namespace MotionIO
{
    /// <summary>
    /// IO卡基类
    /// </summary>
    public abstract class IoControl
    {
        /// <summary>
        ///卡在系统中的序号 
        /// </summary>
        public int Index;

        /// <summary>
        ///卡号 
        /// </summary>
        public int CardNo;

        /// <summary>
        ///输入点状态缓存区 
        /// </summary>
        public int InData;

        /// <summary>
        /// 输入缓冲区数据是否有变化
        /// </summary>
        public bool DataChange;

        /// <summary>
        ///输出点状态缓存区 
        /// </summary>
        public int OutData;

        /// <summary>
        ///卡的有无效状态 
        /// </summary>
        public bool Enable;       

        /// <summary>
        ///输入点名称向量 
        /// </summary>
        public string[]  StrArrayIn;

        /// <summary>
        ///输出点名称向量 
        /// </summary>
        public string[]  StrArrayOut;      

        /// <summary>
        /// 卡名称
        /// </summary>
        public string StrCardName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nCardNo"></param>
        public IoControl(int index, int nCardNo)
        {
            Index = index;
            CardNo = nCardNo;
            Enable = true;
        }

    
        /// <summary>
        /// 输入点总个数
        /// </summary>
        public int CountIoIn => StrArrayIn.Length;

        /// <summary>
        /// 输出点总个数 
        /// </summary>
        public int CountIoOut => StrArrayOut.Length;

        /// <summary>
        /// 卡是否启用成功
        /// </summary>
        /// <returns></returns>
        public bool IsEnable()
        {
            return Enable;
        }

        /// <summary>
        /// 判断输入缓冲区是否有变化
        /// </summary>
        /// <returns></returns>
        public bool IsDataChange()
        {
            return DataChange;
        }

        /// <summary>
        ///初始化 
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();

        /// <summary>
        ///释放IO卡 
        /// </summary>
        public abstract void DeInit();

        /// <summary>
        ///获取卡所有的输入信号 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool ReadIoIn(ref int data);

        /// <summary>
        ///获取卡所有的输出信号 
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public abstract bool ReadIoOut(ref int nData);

        /// <summary>
        ///按位获取输入信号 
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public abstract bool ReadIoInBit(int nIndex);

        /// <summary>
        ///按位获取输出信号 
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public abstract bool ReadIoOutBit(int nIndex);


        /// <summary>
        /// 按位输出信号 
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="bBit"></param>
        /// <returns></returns>
        public abstract bool WriteIoBit(int nIndex, bool bBit);

        /// <summary>
        /// 输出整个卡的信号 
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public abstract bool WriteIo(int nData);

        /// <summary>
        ///获取指定IO输入点的缓冲状态 
        /// </summary>
        /// <returns></returns>

        public bool GetIoInState(int nIndex)
        {
            return (InData & (1 << (nIndex - 1))) != 0;
        }
        /// <summary>
        ///获取指定IO输出点的缓冲状态  
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool GetIoOutState( int nIndex)
        {
            return (OutData & (1 << (nIndex - 1))) != 0;
        }

    }
}
