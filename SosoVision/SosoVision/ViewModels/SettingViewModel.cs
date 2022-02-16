using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SosoVision.Common;
using SosoVision.Extensions;

namespace SosoVision.ViewModels
{
    public class SettingViewModel : BindableBase, IDialogAware
    {
        private readonly IConfigureService _configureService;
        private ObservableCollection<ProcedureParam> _procedureParams;
        private bool _isOkToClose;
        private List<ProcedureParam> _deleteCount;
        public ObservableCollection<ProcedureParam> ProcedureParams
        {
            get { return _procedureParams; }
            set { _procedureParams = value; }
        }

        public ObservableCollection<ProcedureParam> OldParams { get; set; }

        public DelegateCommand Confim { get; }
        public DelegateCommand Cancel { get; }
        public DelegateCommand<object> Delete { get; }

        public SettingViewModel(IConfigureService configureService)
        {
            _configureService = configureService;
            ProcedureParams = _configureService.SerializationData.ProcedureParams;
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
                        case "显示设置":
                            break;
                        case "标定设置":
                            break;
                    }
            }
        }

        private void DeleteProcedureParam()
        {
            _deleteCount = new List<ProcedureParam>();
            foreach (var procedureParam in ProcedureParams)
            {
                if (procedureParam.Delete)
                {
                    _deleteCount.Add(procedureParam);
                }
            }
            foreach (var i in _deleteCount)
            {
                ProcedureParams.Remove(i);
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
                string file = $"config/backup_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.json";
                File.WriteAllText(file, JsonConvert.SerializeObject(OldParams));
                MessageBox.Show($"配置已备份到{file}文件中");
            }
            else
            {
                ProcedureParams.Clear();
                foreach (var procedureParam in OldParams)
                {
                    ProcedureParams.Add(procedureParam);
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
        }

        public string Title { get; } = "视觉配置";
        public event Action<IDialogResult> RequestClose;
    }
}
