using SuperSimpleTcp;
using System.Text;
using Prism.Ioc;
using SosoVisionCommonTool.Log;
using SosoVision.Views;
using SosoVision.ViewModels;
using SosoVisionTool.Services;

namespace SosoVision.Server
{
    public class SosoVisionServerHelper
    {
        private ISosoLogManager _sosoLogManager;
        private SimpleTcpServer _sosoVisionServer;
        public SosoVisionServerHelper(string ip, string port)
        {
            _sosoLogManager = ContainerLocator.Container.Resolve<ISosoLogManager>();
            string str = $"{ip}:{port}";
            _sosoVisionServer = new SimpleTcpServer(str);

            _sosoVisionServer.Events.ClientConnected += ClientConnected;
            _sosoVisionServer.Events.ClientDisconnected += ClientDisconnected;
            _sosoVisionServer.Events.DataReceived += DataReceived;

            _sosoVisionServer.Start();
        }

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
            {
                _sosoLogManager.ShowLogInfo("[" + e.IpPort + "] client connected");
            });
        }

        private void ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
            {
                _sosoLogManager.ShowLogError("[" + e.IpPort + "] client disconnected: " + e.Reason.ToString());
            });
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
            {
                string data = Encoding.UTF8.GetString(e.Data);
                _sosoLogManager.ShowLogInfo($"Receive {data} from {e.IpPort} client");
                if (!data.Contains("\r") || !data.Contains("\n"))
                {
                    _sosoLogManager.ShowLogError($"{data} from {e.IpPort} client error");
                    return;
                }
                string[] receive = data.Substring(0, data.Length - 2).Split(',');
                var view = ContainerLocator.Container.Resolve<VisionProcessView>(receive[0]);

                var VisionRunData = ContainerLocator.Container.Resolve<AllVisionRunData>("GlobalData");

                if (receive.Length > 1)
                {
                    for (int i = 1; i < receive.Length; i++)
                    {
                        string key = $"{receive[0]}_param_{i}";
                        if (VisionRunData.VisionRunStringValue.ContainsKey(key))
                        {
                            VisionRunData.VisionRunStringValue[key] = receive[i];
                        }
                        else
                        {
                            VisionRunData.VisionRunStringValue.Add(key, receive[i]);
                        }
                    }
                }

                if (view == null)
                {
                    _sosoVisionServer.Send(e.IpPort, $"{receive[0]},0");
                    _sosoLogManager.ShowLogError($"View 不存在");
                    return;
                }

                var viewModel = view.DataContext as VisionProcessViewModel;
                if (view.ToolRun.Run())
                {
                    _sosoVisionServer.Send(e.IpPort, $"{receive[0]},1,{viewModel.ResultStr}");
                    _sosoLogManager.ShowLogInfo($"Send {receive[0]},1,{viewModel.ResultStr} to {e.IpPort} client");
                }
                else
                {
                    _sosoVisionServer.Send(e.IpPort, $"{receive[0]},0");
                    _sosoLogManager.ShowLogInfo($"Send {receive[0]},0 to {e.IpPort} client");
                }
            });
        }
    }
}
