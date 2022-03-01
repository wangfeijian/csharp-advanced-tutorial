using System.Collections.ObjectModel;
using SosoVisionCommonTool.ConfigData;

namespace SosoVision.Extensions
{
    /// <summary>
    /// 程序序列化
    /// </summary>
    public class SerializationData
    {
        public ObservableCollection<ProcedureParam> ProcedureParams { get; set; }
        public ObservableCollection<string> ShowListCollection {
            get
            {
                ObservableCollection<string> temp = new ObservableCollection<string>();
                foreach (var procedureParam in ProcedureParams)
                {
                    temp.Add(procedureParam.Name);
                }

                return temp;
            }
        }

        public ObservableCollection<CameraParam> CameraParams { get; set; }
        public ObservableCollection<ServerParam> ServerParams { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
