using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using AutoBuildConfig.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using ComboBox = System.Windows.Controls.ComboBox;

namespace AutoBuildConfig.ViewModel
{
    public class ExtentionViewModel : ViewModelBase
    {
        public ICommand SaveConfigCommand { get; set; }
        public ICommand LoadConfigCommand { get; set; }
        public ICommand SaveAsConfigCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand AddDataGroupCommand { get; set; }
        public ICommand AddDataEnableCommand { get; set; }
        public ICommand UpdateDataGroupCommand { get; set; }
        public ICommand ComboBoxSelectedCommand { get; set; }
        //public ICommand ComBoxEnterCommand { get; set; }

        private IBuildConfig BulidConfig;

        private OtherConfig otherConfigs;

        public OtherConfig OtherConfigs
        {
            get { return otherConfigs; }
            set { Set(ref otherConfigs, value); }
        }

        private AllDataClass allDataClass;

        public AllDataClass AllDataClass
        {
            get { return allDataClass; }
            set { Set(ref allDataClass, value); }
        }

        private DataClassTitle dataClassTitle;

        public DataClassTitle DataClassTitle
        {
            get { return dataClassTitle; }
            set { Set(ref dataClassTitle , value); }
        }

        private ObservableCollection<DataInfo> dataInfos;

        public ObservableCollection<DataInfo> DataInfos
        {
            get { return dataInfos; }
            set { Set(ref dataInfos , value); }
        }

        public ExtentionViewModel(IBuildConfig buildConfig)
        {
            BulidConfig = buildConfig;
            OtherConfigs = BulidConfig.LoadConfig<OtherConfig>("otherConfig");
            InitCommand();
        }

        public List<string> Auth => new List<string> {"Operator", "Adjust", "Engineer", "Adminstrator"};

        private void InitCommand()
        {
            SaveConfigCommand = new RelayCommand<object>(SaveConfig);
            LoadConfigCommand = new RelayCommand<object>(LoadConfigFromFile);
            SaveAsConfigCommand = new RelayCommand<object>(SaveAsConfig);
            SelectionChangedCommand = new RelayCommand<object>(SelectChange);
            AddDataGroupCommand = new RelayCommand<object>(AddDataGroup);
            AddDataEnableCommand = new RelayCommand<object>(AddDataEnalbe);
            UpdateDataGroupCommand = new RelayCommand<object>(UpdateDataGroup);
            ComboBoxSelectedCommand = new RelayCommand<SelectionChangedEventArgs>(ComboBoxSelectChanged);
            //ComBoxEnterCommand = new RelayCommand<object>(obj => { ((ComboBox)obj).PreviewKeyDown += ComBoxKeyDown; });
        }

        private void AddDataEnalbe(object obj)
        {
            var checkBox = obj as CheckBox;
            
            if ((bool)checkBox.IsChecked)
            {
                DataClassTitle = new DataClassTitle();
                DataInfos = new ObservableCollection<DataInfo>();
            }
        }
        private void UpdateDataGroup(object obj)
        {
            ComboBox comboBox = obj as ComboBox;

            if (comboBox.SelectedIndex==-1)
            {
                MessageBox.Show("没有选择数据！");
                return;
            }
            string value = comboBox.SelectedItem.ToString();

            var dataTilte = DeepCopyByXml(DataClassTitle);
            var dataInfo = DeepCopyByXml(DataInfos);
            AllDataClass.DataTitleDictionary[value] = dataTilte;
            AllDataClass.DataInfoDictionary[value] = dataInfo;

            BulidConfig.SaveConfig(AllDataClass, "dataType.json");
            BulidConfig.SaveConfig(DataClassTitle, "dataTitle.json");
            BulidConfig.SaveConfig(DataInfos, "dataInfo.json");
            MessageBox.Show($"{value} 更新成功");

        }

        private void ComboBoxSelectChanged(SelectionChangedEventArgs e)
        {
            ComboBox comboBox = e.Source as ComboBox;
            string value = comboBox.SelectedItem.ToString();

            DataClassTitle = AllDataClass.DataTitleDictionary[value];
            DataInfos = AllDataClass.DataInfoDictionary[value];
            e.Handled = true;
        }
        private void AddDataGroup(object obj)
        {
            TextBox textBox = obj as TextBox;

            string value = textBox.Text;
            if (value.Trim() == "")
            {
                return;
            }
            if (!AllDataClass.ComboxStrList.Contains(value))
            {
                AllDataClass.ComboxStrList.Add(value);
                var dataTilte = DeepCopyByXml(DataClassTitle);
                var dataInfo = DeepCopyByXml(DataInfos);
                AllDataClass.DataTitleDictionary[value] = dataTilte;
                AllDataClass.DataInfoDictionary[value] = dataInfo;
                MessageBox.Show($"{value} 添加成功");
            }
            else
            {
                MessageBox.Show($"{value} 已存在！请重新添加！");
            }

        }

        private T DeepCopyByXml<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        private void ComBoxKeyDown(object obj, KeyEventArgs e)
        {
            TextBox comboBox = obj as TextBox;

            if (e.Key == Key.Enter)
            {
                string value = comboBox.Text;
                AllDataClass.ComboxStrList.Add(value);
                AllDataClass.DataTitleDictionary[value] = new DataClassTitle { Name = value };
                AllDataClass.DataInfoDictionary[value] = new ObservableCollection<DataInfo>();
            }
        }

        private void SelectChange(object obj)
        {
            TabControl control = obj as TabControl;
            TabItem tabItem = control?.SelectedItem as TabItem;

            if (tabItem != null)
                switch (tabItem.Header.ToString())
                {
                    case "其它配置":
                        OtherConfigs = BulidConfig.LoadConfig<OtherConfig>("otherConfig");
                        break;
                    case "数据分类":
                        if (AllDataClass == null)
                        {
                            AllDataClass = BulidConfig.LoadConfig<AllDataClass>("dataClass");
                            DataClassTitle = BulidConfig.LoadConfig<DataClassTitle>("dataClassTitle");
                            DataInfos = BulidConfig.LoadConfig<ObservableCollection<DataInfo>>("dataInfo");
                        }
                        break;
                    case "数据显示":
                        break;
                    case "数据保存":
                        break;
                }
        }
        private void SaveConfig(object tabControl)
        {
            TabControl control = tabControl as TabControl;
            TabItem tabItem = control?.SelectedItem as TabItem;

            if (tabItem != null)
                switch (tabItem.Header.ToString())
                {
                    case "其它配置":
                        BulidConfig.SaveConfig(OtherConfigs, "systemCfgEx.json");
                        MessageBox.Show("保存成功！");
                        break;
                    case "数据分类":
                        BulidConfig.SaveConfig(AllDataClass, "dataType.json");
                        BulidConfig.SaveConfig(DataClassTitle, "dataTitle.json");
                        BulidConfig.SaveConfig(DataInfos, "dataInfo.json");
                        MessageBox.Show("保存成功！");
                        break;
                    case "数据显示":
                        break;
                    case "数据保存":
                        break;
                }
        }

        private void LoadConfigFromFile(object tabControl)
        {
            TabControl control = tabControl as TabControl;
            if (control != null)
            {
                TabItem tabItem = control.SelectedItem as TabItem;
                try
                {
                    if (tabItem != null)
                        switch (tabItem.Header.ToString())
                        {
                            case "其它配置":
                                OtherConfigs = BulidConfig.LoadConfigFromFile<OtherConfig>();
                                break;
                            case "数据分类":
                                AllDataClass = BulidConfig.LoadConfigFromFile<AllDataClass>();
                                break;
                            case "数据显示":
                                break;
                            case "数据保存":
                                break;
                        }
                }
                catch (Exception e)
                {
                    MessageBox.Show("未选择文件！" + e);
                }
            }
        }

        private void SaveAsConfig(object tabControl)
        {
            TabControl control = tabControl as TabControl;
            TabItem tabItem = control?.SelectedItem as TabItem;

            if (tabItem != null)
                switch (tabItem.Header.ToString())
                {
                    case "其它配置":
                        BulidConfig.SaveAsConfig(OtherConfigs);
                        break;
                    case "数据分类":
                        BulidConfig.SaveAsConfig(AllDataClass);
                        break;
                    case "数据显示":
                        break;
                    case "数据保存":
                        break;
                }
        }
    }
    public class CylinderInfo
    {
        public string Name { get; set; }
        public string EngName { get; set; }
        public string Type { get; set; }
        public string ExtendOutput { get; set; }
        public string RetractOutput { get; set; }
        public string ExtendInput { get; set; }
        public string RetractInput { get; set; }
        public string ExtendEnable { get; set; }
        public string RetractEnable { get; set; }
        public string TimeOut { get; set; }
    }

    public class LightInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Aisle { get; set; }
        public string AisleIndex { get; set; }
    }

    public class ReflectInfo
    {
        public string Name { get; set; }
        public string StationName { get; set; }
        public string MethodName { get; set; }
    }

    public class ServersInfo
    {
        public string Index { get; set; }
        public string IpAddress { get; set; }
        public string ListenPort { get; set; }
        public string Enable { get; set; }
    }

    public class OtherConfig
    {
        private List<CylinderInfo> cylinderInfos;

        public List<CylinderInfo> CylinderInfos
        {
            get { return cylinderInfos; }
            set { cylinderInfos = value; }
        }

        private List<LightInfo> lightInfos;

        public List<LightInfo> LightInfos
        {
            get { return lightInfos; }
            set { lightInfos = value; }
        }

        private List<ReflectInfo> calibInfos;

        public List<ReflectInfo> CalibInfos
        {
            get { return calibInfos; }
            set { calibInfos = value; }
        }

        private List<ReflectInfo> grrInfos;

        public List<ReflectInfo> GrrInfos
        {
            get { return grrInfos; }
            set { grrInfos = value; }
        }

        private List<ServersInfo> serversInfos;

        public List<ServersInfo> ServersInfos
        {
            get { return serversInfos; }
            set { serversInfos = value; }
        }

    }

    public class DataClassTitle
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public bool PdcaEnable { get; set; }
        public int Authority { get; set; }
    }

    public class DataInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string DataIndex { get; set; }
        public string StandardValue { get; set; }
        public string UpperValue { get; set; }
        public string LowerValue { get; set; }
        public string OffsetValue { get; set; }
        public string Unit { get; set; }

    }

    public class DataClass
    {
        public DataClass()
        {
            DataClassTitle = new DataClassTitle();
            DataInfos = new List<DataInfo>();
        }
        private DataClassTitle dataClassTitle;

        public DataClassTitle DataClassTitle
        {
            get { return dataClassTitle; }
            set { dataClassTitle = value; }
        }

        private List<DataInfo> dataInfos;

        public List<DataInfo> DataInfos
        {
            get { return dataInfos; }
            set { dataInfos = value; }
        }

    }

    public class AllDataClass
    {
        // 此处不能使用list，使用list会造成界面更新不及时
        private ObservableCollection<string> comboxStrList;

        public ObservableCollection<string> ComboxStrList
        {
            get { return comboxStrList; }
            set { comboxStrList = value; }
        }

        private Dictionary<string, DataClassTitle> dataTitleDictionary;

        public Dictionary<string, DataClassTitle> DataTitleDictionary
        {
            get { return dataTitleDictionary; }
            set { dataTitleDictionary = value; }
        }

        private Dictionary<string, ObservableCollection<DataInfo>> dataInfoDictionary;

        public Dictionary<string, ObservableCollection<DataInfo>> DataInfoDictionary
        {
            get { return dataInfoDictionary; }
            set { dataInfoDictionary = value; }
        }

        //private DataClassTitle dataClassTitle;

        //public DataClassTitle DataClassTitle
        //{
        //    get { return dataClassTitle; }
        //    set { dataClassTitle = value; }
        //}

        //private ObservableCollection<DataInfo> dataInfos;

        //public ObservableCollection<DataInfo> DataInfos
        //{
        //    get { return dataInfos; }
        //    set { dataInfos = value; }
        //}

    }

}
