using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace SosoVision.Extensions
{
    /// <summary>
    /// 程序序列化
    /// </summary>
    public class SerializationData
    {
        public ObservableCollection<ProcedureParam> ProcedureParams { get; set; }
    }
}
