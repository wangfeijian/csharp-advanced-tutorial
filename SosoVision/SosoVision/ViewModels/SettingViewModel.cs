using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SosoVision.Common;
using SosoVision.Extensions;
using SosoVisionCommonTool.ConfigData;

namespace SosoVision.ViewModels
{
    public class SettingViewModel : BindableBase, IDialogAware
    {
        static readonly System.Collections.Specialized.NameValueCollection CaptureStr = ConfigurationManager.AppSettings;
        static readonly string CaptureKey = CaptureStr["Capture"];

        private readonly IConfigureService _configureService;
        private ObservableCollection<ProcedureParam> _procedureParams;
        private ObservableCollection<CameraParam> _cameraParams;
        private ObservableCollection<ServerParam> _serverParams;
        private ObservableCollection<CalibrationParam> _calibParams;
        private bool _isOkToClose;
        private List<ProcedureParam> _deleteProcedureParamCount;
        private List<CameraParam> _deleteCameraParamCount;
        private List<ServerParam> _deleteServerParamCount;
        private List<CalibrationParam> _deleteCalibParamCount;
        public int Row { get; set; }
        public int Col { get; set; }
        public ObservableCollection<ProcedureParam> ProcedureParams
        {
            get { return _procedureParams; }
            set { _procedureParams = value; }
        }

        public ObservableCollection<ProcedureParam> OldParams { get; set; }

        public ObservableCollection<CameraParam> CameraParams
        {
            get { return _cameraParams; }
            set { _cameraParams = value; }
        }

        public ObservableCollection<CameraParam> OldCameraParams { get; set; }

        public ObservableCollection<ServerParam> ServerParams
        {
            get { return _serverParams; }
            set { _serverParams = value; }
        }

        public ObservableCollection<CalibrationParam> OldCalibParams { get; set; }

        public ObservableCollection<CalibrationParam> CalibParams
        {
            get { return _calibParams; }
            set { _calibParams = value; }
        }

        private IEnumerable<string> _cameraBands;

        public IEnumerable<string> CameraBands
        {
            get 
            {
                List<string> temp = new List<string>();
                foreach (var item in CaptureKey.Split(','))
                {
                    temp.Add(item);
                }

                _cameraBands = temp;
                return _cameraBands;
            }
        }

        public IEnumerable<string> CalibTypes => new[] { "单相机标定", "上下相机映射","多点标定","偏移标定"};
        public ObservableCollection<ServerParam> OldServerParams { get; set; }
        public DelegateCommand Confim { get; }
        public DelegateCommand Cancel { get; }
        public DelegateCommand<object> Delete { get; }

        public SettingViewModel(IConfigureService configureService)
        {
            _configureService = configureService;
            ProcedureParams = _configureService.SerializationData.ProcedureParams;
            CameraParams = _configureService.SerializationData.CameraParams;
            ServerParams = _configureService.SerializationData.ServerParams;
            CalibParams = _configureService.SerializationData.CalibParams;
            Row = _configureService.SerializationData.Row;
            Col = _configureService.SerializationData.Col;
            Confim = new DelegateCommand(() => { _isOkToClose = true; RequestClose?.Invoke(new DialogResult(ButtonResult.OK)); });
            Cancel = new DelegateCommand(() => { _isOkToClose = false; RequestClose?.Invoke(new DialogResult(ButtonResult.No)); });
            Delete = new DelegateCommand<object>(DeleteItems);
        }

        private void DeleteItems(object obj)
        {
            var temp = obj as TabControl;
            if (temp != null)
            {
                var item = temp.SelectedItem as TabItem;
                if (item != null)
                    switch (item.Header.ToString())
                    {
                        case "检测设置":
                            DeleteProcedureParam();
                            break;
                        case "相机设置":
                            DeleteCameraParam();
                            break;
                        case "标定设置":
                            DeleteCalibParam();
                            break;
                        case "通讯设置":
                            DeleteServerParam();
                            break;
                    }
            }
        }

        private void DeleteCalibParam()
        {
            _deleteCalibParamCount = new List<CalibrationParam>();
            foreach (var calibParam in CalibParams)
            {
                if (calibParam.Delete)
                {
                    _deleteCalibParamCount.Add(calibParam);
                }
            }
            foreach (var i in _deleteCalibParamCount)
            {
                CalibParams.Remove(i);
            }
        }
        private void DeleteProcedureParam()
        {
            _deleteProcedureParamCount = new List<ProcedureParam>();
            foreach (var procedureParam in ProcedureParams)
            {
                if (procedureParam.Delete)
                {
                    _deleteProcedureParamCount.Add(procedureParam);
                }
            }
            foreach (var i in _deleteProcedureParamCount)
            {
                ProcedureParams.Remove(i);
            }
        }

        private void DeleteCameraParam()
        {
            _deleteCameraParamCount = new List<CameraParam>();
            foreach (var cameraParam in CameraParams)
            {
                if (cameraParam.Delete)
                {
                    _deleteCameraParamCount.Add(cameraParam);
                }
            }
            foreach (var i in _deleteCameraParamCount)
            {
                CameraParams.Remove(i);
            }
        }

        private void DeleteServerParam()
        {
            _deleteServerParamCount = new List<ServerParam>();
            foreach (var serverParam in ServerParams)
            {
                if (serverParam.Delete)
                {
                    _deleteServerParamCount.Add(serverParam);
                }
            }
            foreach (var i in _deleteServerParamCount)
            {
                ServerParams.Remove(i);
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            if (_isOkToClose)
            {
                string dir = "config";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }


                string file = $"{dir}/backup_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.json";
                SerializationData serialization = new SerializationData { CameraParams = OldCameraParams, ProcedureParams = OldParams, ServerParams = OldServerParams,CalibParams = OldCalibParams, Row = Row, Col = Col };
                File.WriteAllText(file, JsonConvert.SerializeObject(serialization));
                _configureService.SerializationData.Row = Row;
                _configureService.SerializationData.Col = Col;
                MessageBox.Show($"配置已备份到{file}文件中");
            }
            else
            {
                ProcedureParams.Clear();
                foreach (var procedureParam in OldParams)
                {
                    ProcedureParams.Add(procedureParam);
                }

                CameraParams.Clear();
                foreach (var cameraParam in OldCameraParams)
                {
                    CameraParams.Add(cameraParam);
                }

                ServerParams.Clear();
                foreach (var serverParam in OldServerParams)
                {
                    ServerParams.Add(serverParam);
                }

                CalibParams.Clear();
                foreach (var calibParam in OldCalibParams)
                {
                    CalibParams.Add(calibParam);
                }
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _isOkToClose = false;
            OldParams = new ObservableCollection<ProcedureParam>();
            ProcedureParam[] temp = new ProcedureParam[ProcedureParams.Count];
            ProcedureParams.CopyTo(temp, 0);
            temp.ToList().ForEach(p => OldParams.Add(p));

            OldCameraParams = new ObservableCollection<CameraParam>();
            CameraParam[] tempCamera = new CameraParam[CameraParams.Count];
            CameraParams.CopyTo(tempCamera, 0);
            tempCamera.ToList().ForEach(p => OldCameraParams.Add(p));

            OldServerParams = new ObservableCollection<ServerParam>();
            ServerParam[] tempServer = new ServerParam[ServerParams.Count];
            ServerParams.CopyTo(tempServer, 0);
            tempServer.ToList().ForEach(p => OldServerParams.Add(p));

            OldCalibParams = new ObservableCollection<CalibrationParam>();
            CalibrationParam[] tempCalib = new CalibrationParam[CalibParams.Count];
            CalibParams.CopyTo(tempCalib, 0);
            tempCalib.ToList().ForEach(p => OldCalibParams.Add(p));
        }

        public string Title { get; } = "视觉配置";
        public event Action<IDialogResult> RequestClose;
    }
}
