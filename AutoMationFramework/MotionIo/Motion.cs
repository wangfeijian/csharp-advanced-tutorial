using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MotionIO
{
    /// <summary>
    /// 回原点模式
    /// </summary>
    public enum HomeMode
    {
        /// <summary>
        /// 原点信号+正方向
        /// </summary>
        ORG_P = 0,

        /// <summary>
        /// 原点信号+负方向
        /// </summary>
        ORG_N,    

        /// <summary>
        /// 正限位信号为原点，正方向回原点
        /// </summary>
        PEL,      

        /// <summary>
        /// 负限位信号为原点，负方向回原点
        /// </summary>
        MEL,      


        /// <summary>
        /// EZ信号为原点信号，正方向回原点
        /// </summary>
        EZ_PEL,   

        /// <summary>
        /// EZ信号为原点信号，负方向回原点
        /// </summary>
        EZ_MEL,   

        /// <summary>
        /// 原点信号+正方向+EZ
        /// </summary>
        ORG_P_EZ,

        /// <summary>
        /// 原点信号+负方向+EZ
        /// </summary>
        ORG_N_EZ,

        /// <summary>
        /// 正限位信号为原点+EZ
        /// </summary>
        PEL_EZ,

        /// <summary>
        /// 负限位信号为原点+EZ
        /// </summary>
        MEL_EZ,

        /// <summary>
        /// 其他模式，直接从卡中读取回原点模式，无需指定参数
        /// </summary>
        CARD = 999,

        /// <summary>
        /// 其他总线型，不能从卡中读取回原点方式，基于此值累加，例如汇川总线型，选择回原点方式1，则实际传入的参数为1000 + 1 = 1001
        /// </summary>
        BUS_BASE = 1000,
    }

    /// <summary>
    /// 运动控制卡封装基类
    /// </summary>
    public abstract class Motion
    {
        /// <summary>
        /// 判定卡是否启用或初始化成功
        /// </summary>
        protected bool m_bEnable = false;
        /// <summary>
        /// 当前卡在系统中的索引号
        /// </summary>
        protected int m_nCardIndex = 0;
        /// <summary>
        /// 卡类型名称
        /// </summary>
        private string m_strName = string.Empty;
        /// <summary>
        /// 卡类型支持的最小轴号
        /// </summary>
        private int m_nMinAxisNo = 0;
        /// <summary>
        /// 卡类型支持的最大轴号
        /// </summary>
        private int m_nMaxAxisNo=255;
        /// <summary>
        /// 存储连续运动缓存表与板卡对应关系的字典
        /// </summary>
        public Dictionary<int, int> m_dicBoard = new Dictionary<int, int>();

        /// <summary>
        /// 4轴还是8轴或者12轴，16轴
        /// </summary>
        public int m_nAxisNum = 4;  

        /// <summary>
        /// 构造初始化
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public Motion(int nCardIndex,string strName, int nMinAxisNo, int nMaxAxisNo)
        {
            m_nCardIndex = nCardIndex;
            m_strName = strName;
            m_nMinAxisNo = nMinAxisNo;
            m_nMaxAxisNo = nMaxAxisNo;

            m_nAxisNum = ((m_nMaxAxisNo - m_nMinAxisNo) / 4 + 1) * 4; //计算当前卡支持的轴数
        }
        /// <summary>
        /// 卡是否启用成功
        /// </summary>
        /// <returns></returns>
        public bool IsEnable()
        {
            return m_bEnable;

        }

        /// <summary>
        ///以绝对位置移动 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool AbsMove(int nAxisNo, int nPos, int nSpeed);
        /// <summary>
        /// 带所有速度参数的绝对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="fPos"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public virtual bool AbsMove(int nAxisNo, double fPos, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            return false;
        }

        /// <summary>
        ///相对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool RelativeMove(int nAxisNo, int nPos, int nSpeed);
        /// <summary>
        /// 带所有速度参数的相对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="fOffset"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public virtual bool RelativeMove(int nAxisNo, double fOffset, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            return false;
        }
        /// <summary>
        ///速度模式
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool VelocityMove(int nAxisNo, int nSpeed);
        
        /// <summary>
        ///jog运动 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="bPositive"></param>
        /// <param name="bStrat"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public abstract bool JogMove(int axis, bool bPositive, int bStrat, int nSpeed);
       
        /// <summary>
        ///获取轴卡运动状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract long GetMotionState(int nAxisNo);
        
        /// <summary>
        ///获取轴卡运动IO信号 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract long GetMotionIoState(int nAxisNo);
        /// <summary>
        ///获取轴的当前位置 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract double GetAixsPos(int nAxisNo);
       
        /// <summary>
        ///轴是否正常停止 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract int IsAxisNormalStop(int nAxisNo);

        /// <summary>
        /// 轴号是否在范围内
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nInPosError">到位误差</param>
        /// <returns></returns>
        public abstract int IsAxisInPos(int nAxisNo,int nInPosError = 1000);

        /// <summary>
        ///回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nMode">回原点参数, 对于8254，此参数代表回原点的方向</param>
        /// <returns></returns>
        public abstract bool Home(int nAxisNo, int nMode);

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nMode"></param>
        /// <param name="vm"></param>
        /// <param name="vo"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="offset">原点偏移</param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public virtual bool Home(int nAxisNo, int nMode, double vm,double vo,double acc,double dec,double offset = 0,double sFac = 0)
        {
            return false;
        }

        /// <summary>
        ///位置置零 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool SetPosZero(int nAxisNo);

        /// <summary>
        ///清除错误
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public virtual bool ClearError(int nAxisNo)
        {
            return false;
        }

        /// <summary>
        /// 设置单轴的某一运动参数
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">参数:1:加速度 2:减速度 3:梯形或S曲线(8254) 3:起跳速度(S曲线) 4:结束速度(S曲线) 5:平滑时间(固高卡S曲线) 其它：自定义扩展</param>
        /// <param name="nData">参数值</param>
        /// <returns></returns>
        public virtual bool SetAxisParam(int nAxisNo, int nParam, int nData)
        {
            return false;
        }

        /// <summary>
        /// 设置单轴的某一运动参数(浮点型)
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">参数:自定义扩展</param>
        /// <param name="fData">参数值</param>
        /// <returns></returns>
        public virtual bool SetAxisParam(int nAxisNo, int nParam, double fData)
        {
            return false;
        }

        /// <summary>
        ///开启使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool ServoOn(int nAxisNo);

        /// <summary>
        ///断开使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool ServoOff(int nAxisNo);

        /// <summary>
        /// 读取伺服使能状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool GetServoState(int nAxisNo);
        
        /// <summary>
        ///轴正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool StopAxis(int nAxisNo);

        /// <summary>
        ///急停 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract bool StopEmg(int nAxisNo);

        /// <summary>
        ///轴卡初始化 
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();

        /// <summary>
        ///关闭轴卡 
        /// </summary>
        /// <returns></returns>
        public abstract bool DeInit();
        /// <summary>
        /// 回原点是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public abstract int IsHomeNormalStop(int nAxisNo);

        /// <summary>
        ///获取运动控制卡名称 
        /// </summary>
        /// <returns></returns>
        public string GetCardName() { return m_strName; }

        /// <summary>
        /// 获取最小轴号
        /// </summary>
        /// <returns></returns>
        public int GetMinAxisNo() { return m_nMinAxisNo; }

        /// <summary>
        /// //获取最大轴号
        /// </summary>
        /// <returns></returns>
        public int GetMaxAxisNo() { return m_nMaxAxisNo; }    

        /// <summary>
        /// 获取系统轴号
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        protected int  GetSysAxisNo(int nAxisNo)
        {
            return nAxisNo + GetMinAxisNo();
        }
        
        /// <summary>
        /// 判断当前轴号是否属于此卡
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public bool AxisInRang(int nAxisNo)
        {
            return nAxisNo >= m_nMinAxisNo && nAxisNo <= m_nMaxAxisNo;
        }

        /// <summary>
        /// 获取卡的系统索引号
        /// </summary>
        /// <returns></returns>
        public int GetCardIndex()
        {
            return m_nCardIndex;
        }


        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray">轴数组</param>
        /// <param name="nPosArray">目标点的绝对座标位置</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public virtual bool AbsLinearMove(ref int[] nAixsArray, ref double[] nPosArray, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            return false;
        }

        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fPosOffsetArray">目标点的相对座标位置</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public virtual bool RelativeLinearMove(ref int[] nAixsArray, ref double[] fPosOffsetArray, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            return false;
        }

        /// <summary>
        /// 以当前点位为起始点进行两轴圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fCenterArray">圆心的绝对座标位置</param>   
        /// <param name="fEndArray">终点的绝对座标位置</param>   
        /// <param name="Dir">圆弧的方向，　0:正向，　１：负向</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public virtual bool AbsArcMove(ref int[] nAixsArray, ref double[] fCenterArray, ref double[] fEndArray, int Dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            return false;
        }

        /// <summary>
        /// 以当前点位为起始点，以偏差位置为圆心，进行多轴圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fCenterOffsetArray">圆心的绝对座标位置</param>   
        /// <param name="fEndArray">终点的绝对座标位置</param>   
        /// <param name="Dir">圆弧的方向，　0:正向，　１：负向</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public virtual bool RelativeArcMove(ref int[] nAixsArray, ref double[] fCenterOffsetArray, ref double[] fEndArray, int Dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            return false;
        }

        /// <summary>
        /// 配置一个连续运动的缓冲表(2000点的buff需要更新fimeware)
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="nAixsArray">轴号数组</param>
        /// <param name="bAbsolute">true:绝对位置模式，　false:相对位置模式</param>
        /// <returns></returns>
        public virtual bool ConfigPointTable(int nPointTableIndex, ref int[] nAixsArray, bool bAbsolute)
        {
            return false;
        }

        /// <summary>
        /// 向连续运动的缓冲表插入一个直线插补运动（多轴）
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="positionArray">目标位置数组，需要轴号数组匹配</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="vm">最大速度</param>
        /// <param name="ve">终点速度</param>
        /// <param name="sf">S曲线因子</param>
        /// <returns></returns>
        public virtual bool PointTable_Line_Move(int nPointTableIndex, ref double[] positionArray, double acc, double dec, double vs, double vm, double ve, double sf)
        {
            return false;
        }

        /// <summary>
        /// 向连续运动的缓冲表插入一个圆弧插补运动（两轴）
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="centerArray">圆弧中心点位置</param>
        /// <param name="endArray">圆弧结束点位置</param>
        /// <param name="dir">圆弧的方向, 0:顺时针，-1:逆时针</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="vm">最大速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sf">S曲线因子</param>
        /// <returns></returns>
        public virtual bool PointTable_ArcE_Move(int nPointTableIndex, ref double[] centerArray, ref double[] endArray, short dir,
                    double acc, double dec, double vs, double vm, double ve, double sf)
        {
            return false;
        }

        /// <summary>
        /// 向连续运动缓冲表插入一个与运动同步的IO控制(IO控制在运动之前)
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="nChannel">IO点的序号</param>
        /// <param name="bOn">1：on, 0: off</param>
        /// <returns></returns>
        public virtual bool PointTable_IO(int nPointTableIndex, int nChannel, int bOn)
        {
            return false;
        }
        /// <summary>
        /// 启动或停止一个连续运动
        /// </summary>
        /// <param name="nPointTableIndex">连续运动列表的序号</param>
        /// <param name="bStart">true:开始运行, false:停止运行</param>
        /// <returns></returns>
        public virtual bool PointTable_Start(int nPointTableIndex, bool bStart)
        {
            return false;
        }

        /// <summary>
        /// 确定连续运动列表的BUFF是否可以插入新的运动
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <returns>true: 可以插入　，　false: BUFF已满</returns>
        public virtual bool PointTable_IsIdle(int nPointTableIndex)
        {
            return false;
        }

        /// <summary>
        /// 向缓冲区插入一个延时指令
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <param name="nMillsecond">需要延时的毫秒值</param>
        /// <returns></returns>
        public virtual bool PointTable_Delay(int nPointTableIndex, int nMillsecond)
        {
            return false;
        }

        /// <summary>
        /// 启用软件正限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public virtual bool SetSPELEnable(int nAxisNo,bool bEnable)
        {
            return false;
        }

        /// <summary>
        /// 启用软件负限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public virtual bool SetSMELEnable(int nAxisNo,bool bEnable)
        {
            return false;
        }

        /// <summary>
        /// 设置软件正限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool SetSPELPos(int nAxisNo,double pos)
        {
            return false;
        }

        /// <summary>
        /// 设置软件负限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool SetSMELPos(int nAxisNo,double pos)
        {
            return false;
        }
    }
}
