/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Station point class                      *
*********************************************************************/

using System.Collections.Generic;

namespace AutoMationFrameworkModel
{
    public class PointInfo
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string XPos { get; set; }
        public string YPos { get; set; }
        public string ZPos { get; set; }
        public string UPos { get; set; }
        public string APos { get; set; }
        public string BPos { get; set; }
        public string CPos { get; set; }
        public string DPos { get; set; }
    }

    public class StationPoint
    {
        public string Name { get; set; }
        public string EngName { get; set; }
        public List<PointInfo> PointInfos { get; set; }
    }
}
