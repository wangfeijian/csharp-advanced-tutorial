using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SosoVisionCommonTool.ConfigData
{
    public class ServerParam:BindableBase
    {
        /// <summary>
        /// 删除该行
        /// </summary>
        private bool _delete;

        public bool Delete
        {
            get { return _delete; }
            set { _delete = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        private string _serverName;

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 服务Ip
        /// </summary>
        private string _serverIp;

        public string ServerIp
        {
            get { return _serverIp; }
            set { _serverIp = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 服务端口
        /// </summary>
        private string _serverPort;

        public string ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; RaisePropertyChanged(); }
        }
    }
}
