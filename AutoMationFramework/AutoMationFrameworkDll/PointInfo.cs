/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-06                               *
*                                                                    *
*           ModifyTime:     2021-08-06                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Station base class                       *
*********************************************************************/
namespace AutoMationFrameworkDll
{
    /// <summary>
    /// 点位信息
    /// </summary>
    public class PointInfo
    {
        /// <summary>
        /// 各轴的位置
        /// </summary>
        public double[] Pos = new double[8];

        /// <summary>
        /// 点位名称
        /// </summary>
        public string StrName;

        /// <summary>
        /// X轴脉冲位置
        /// </summary>
        public double X
        {
            get
            {
                return Pos[0];
            }
            set
            {
                Pos[0] = value;
            }
        }

        /// <summary>
        /// Y轴脉冲位置
        /// </summary>
        public double Y
        {
            get
            {
                return Pos[1];
            }
            set
            {
                Pos[1] = value;
            }
        }

        /// <summary>
        /// Z轴脉冲位置
        /// </summary>
        public double Z
        {
            get
            {
                return Pos[2];
            }
            set
            {
                Pos[2] = value;
            }
        }

        /// <summary>
        /// U轴脉冲位置
        /// </summary>
        public double U
        {
            get
            {
                return Pos[3];
            }
            set
            {
                Pos[3] = value;
            }
        }

        /// <summary>
        /// A轴脉冲位置
        /// </summary>
        public double A
        {
            get
            {
                return Pos[4];
            }
            set
            {
                Pos[4] = value;
            }
        }

        /// <summary>
        /// B轴脉冲位置
        /// </summary>
        public double B
        {
            get
            {
                return Pos[5];
            }
            set
            {
                Pos[5] = value;
            }
        }

        /// <summary>
        /// C轴脉冲位置
        /// </summary>
        public double C
        {
            get
            {
                return Pos[6];
            }
            set
            {
                Pos[6] = value;
            }
        }

        /// <summary>
        /// D轴脉冲位置
        /// </summary>
        public double D
        {
            get
            {
                return Pos[7];
            }
            set
            {
                Pos[7] = value;
            }
        }
    }
}
