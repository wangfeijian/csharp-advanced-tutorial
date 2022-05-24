namespace OmronFinsTCP.Net
{
    /// <summary>
    /// 寄存器类型,十六进制表示形式
    /// </summary>
    public enum PlcMemory
    {
        //CIO_Word = 0xB0,
        //CIO_Bit = 0x30,
        //WR_Word = 0xB1,
        //WR_Bit = 0x31,
        //HR_Word =0xB2,
        //HR_Bit = 0x32,
        //AR_Word =0xB3,
        //AR_Bit = 0x33,
        //DM_Word = 0x82,
        //DM_Bit = 0x02
        /// <summary>
        /// CIO Area
        /// </summary>
        Cio,
        /// <summary>
        /// Work Area
        /// </summary>
        Wr,
        /// <summary>
        /// Holding Bit Area
        /// </summary>
        Hr,
        /// <summary>
        /// Ausiliary Bit Area
        /// </summary>
        Ar,
        /// <summary>
        /// DM Area
        /// </summary>
        Dm
    }

    /// <summary>
    /// 地址类型
    /// </summary>
    public enum MemoryType
    {
        /// <summary>
        /// 位
        /// </summary>
        Bit,
        /// <summary>
        /// 字
        /// </summary>
        Word
    }

    /// <summary>
    /// 数据类型,PLC字为16位数，最高位为符号位，负数表现形式为“取反加一”
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// byte
        /// </summary>
        Bit,
        /// <summary>
        /// short
        /// </summary>
        Int16,
        /// <summary>
        /// double
        /// </summary>
        Real
    }

    /// <summary>
    /// bit位开关状态，on=1，off=0
    /// </summary>
    public enum BitState
    {
        /// <summary>
        /// 1
        /// </summary>
        On = 1,
        /// <summary>
        /// 0
        /// </summary>
        Off = 0
    }

    /// <summary>
    /// 区分指令的读写类型
    /// </summary>
    public enum RorW
    {
        /// <summary>
        /// 读
        /// </summary>
        Read,
        /// <summary>
        /// 写
        /// </summary>
        Write
    }
}
