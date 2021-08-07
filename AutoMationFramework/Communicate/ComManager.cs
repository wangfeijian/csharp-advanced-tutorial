/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Com port manager class                   *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AutoMationFrameworkSystemDll;

namespace Communicate
{
    /// <summary>
    /// 串口系统管理类
    /// </summary>
    public class ComManager : SingletonPattern<ComManager>
    {
        /// <summary>
        /// 串口定义描述
        /// </summary>
        public static readonly string[] StrDescribe = {  "串口号", "串口定义", "波特率", "数据位", "校验位",
                                            "停止位", "流控制", "超时时间", "缓冲区大小","命令分隔" };
        /// <summary>
        /// 串口定义列表
        /// </summary>
        private List<ComLink> _listComLink = new List<ComLink>();

        /// <summary>
        /// 从xml文件中读取定义的串口信息
        /// </summary>
        /// <param name="doc">已打开的xml文档</param>
        public void ReadCfgFromXml(XmlDocument doc)
        {
            _listComLink.Clear();
            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "Com");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;

                        string strNo = xe.GetAttribute(StrDescribe[0]).Trim();
                        string strName = xe.GetAttribute(StrDescribe[1]).Trim();
                        string strBaudRate = xe.GetAttribute(StrDescribe[2]).Trim();
                        string strDataBit = xe.GetAttribute(StrDescribe[3]).Trim();
                        string strPartiy = xe.GetAttribute(StrDescribe[4]).Trim();
                        string strStopBit = xe.GetAttribute(StrDescribe[5]).Trim();
                        string strFlowCtrl = xe.GetAttribute(StrDescribe[6]).Trim(); 
                        string strTime = xe.GetAttribute(StrDescribe[7]).Trim();
                        string strBufferSize = xe.GetAttribute(StrDescribe[8]).Trim();
                        string strLine = xe.GetAttribute(StrDescribe[9]).Trim();

                        _listComLink.Add(new ComLink(Convert.ToInt32(strNo), strName,  Convert.ToInt32(strBaudRate),
                                    Convert.ToInt32(strDataBit), strPartiy, strStopBit, strFlowCtrl,
                                    Convert.ToInt32(strTime),Convert.ToInt32(strBufferSize), strLine));
                    }
                }
            }
        }

        /// <summary>
        /// 跟新内存参数到表格数据
        /// </summary>
        /// <param name="grid">界面串口表格控件</param>
        //public void UpdateGridFromParam(DataGrid grid)
        //{
        //    grid.Rows.Clear();
        //    if (_listComLink.Count > 0)
        //    {
                
        //        grid.Rows.AddCopies(0, _listComLink.Count);

        //        int i = 0;
        //        foreach (ComLink t in _listComLink)
        //        {
        //            int j = 0;
        //            grid.Rows[i].Cells[j++].Value = t.ComNo.ToString();
        //            grid.Rows[i].Cells[j++].Value = t.StrName;
        //            grid.Rows[i].Cells[j++].Value = t.BaudRate.ToString();
        //            grid.Rows[i].Cells[j++].Value = t.DataBit.ToString();
        //            grid.Rows[i].Cells[j++].Value = t.StrPartiy;
        //            grid.Rows[i].Cells[j++].Value = t.StrStopBit;
        //            grid.Rows[i].Cells[j++].Value = t.StrFlowCtrl;
        //            grid.Rows[i].Cells[j++].Value = t.Time.ToString();
        //            grid.Rows[i].Cells[j++].Value = t.BufferSzie.ToString();
        //            grid.Rows[i].Cells[j++].Value = t.StrLineFlag;

        //            i++;
        //        }
        //    }
        //}

        /// <summary>
        /// 更新表格数据到内存参数
        /// </summary>
        /// <param name="grid">界面串口表格控件</param>
        //public void UpdateParamFromGrid(DataGridView grid)
        //{
        //    int m = grid.RowCount;
        //    int n = grid.ColumnCount;

        //    _listComLink.Clear();

        //    for (int i = 0; i < m; ++i)
        //    {
        //        if (grid.Rows[i].Cells[0].Value == null)
        //            break;
        //        string strNo = grid.Rows[i].Cells[0].Value.ToString();
        //        string strName = grid.Rows[i].Cells[1].Value.ToString();
        //        string strBaudRate = grid.Rows[i].Cells[2].Value.ToString();
        //        string strDataBit = grid.Rows[i].Cells[3].Value.ToString();
        //        string strPartiy = grid.Rows[i].Cells[4].Value.ToString();
        //        string strStopBit = grid.Rows[i].Cells[5].Value.ToString();
        //        string strFlowCtrl = grid.Rows[i].Cells[6].Value.ToString();
        //        string strTime = grid.Rows[i].Cells[7].Value.ToString();
        //        string strBufferSize = grid.Rows[i].Cells[8].Value.ToString();
        //        string strLine = grid.Rows[i].Cells[9].Value.ToString();

        //        _listComLink.Add(new ComLink(Convert.ToInt32(strNo), strName, Convert.ToInt32(strBaudRate),
        //                Convert.ToInt32(strDataBit), strPartiy, strStopBit, strFlowCtrl,
        //                 Convert.ToInt32(strTime), Convert.ToInt32(strBufferSize), strLine));

        //    }
        //}

        /// <summary>
        /// 保存内存参数到xml文件
        /// </summary>
        /// <param name="doc">已打开的xml文档</param>
        public void SaveCfgXML(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");
            XmlNode root = doc.CreateElement("Com");
            xnl.AppendChild(root);

            foreach (ComLink t in _listComLink)
            {
                XmlElement xe = doc.CreateElement("Com");

                int j = 0;
                xe.SetAttribute(StrDescribe[j++], t.ComNo.ToString());
                xe.SetAttribute(StrDescribe[j++], t.StrName);
                xe.SetAttribute(StrDescribe[j++], t.BaudRate.ToString());
                xe.SetAttribute(StrDescribe[j++], t.DataBit.ToString());
                xe.SetAttribute(StrDescribe[j++], t.StrPartiy);
                xe.SetAttribute(StrDescribe[j++], t.StrStopBit);
                xe.SetAttribute(StrDescribe[j++], t.StrFlowCtrl);
                xe.SetAttribute(StrDescribe[j++], t.Time.ToString());
                xe.SetAttribute(StrDescribe[j++], t.BufferSzie.ToString());
                xe.SetAttribute(StrDescribe[j++], t.StrLineFlag);
                root.AppendChild(xe);
            }
        }

        /// <summary>
        /// 返回对应索引的对象
        /// </summary>
        /// <param name="nIndex">索引号</param>
        /// <returns></returns>
        public ComLink GetComLink(int nIndex)
        {
            if (nIndex < _listComLink.Count())
            {
                return _listComLink.ElementAt(nIndex);
            }
            return null;
        }

        /// <summary>
        /// 获取系统串口总数
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get{ return _listComLink.Count; }
        }

    }
}
