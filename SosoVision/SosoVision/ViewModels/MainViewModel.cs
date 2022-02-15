using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DryIoc;
using ImTools;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SosoVision.Common;
using SosoVision.Extensions;
using SosoVision.Views;

namespace SosoVision.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {
        public SerializationData SerializationData { get; set; }

        private readonly IRegionManager _regionManager;
        private readonly IContainerProvider _containerProvider;
        private readonly IDialogService _dialogService;

        private readonly IConfigureService _configureService;
        public DelegateCommand<string> NavigateCommand { get; }
        public DelegateCommand<string> ShowDialogCommand { get; }
        public DelegateCommand HomeCommand { get; }

        private ObservableCollection<LogStruct> _logStructs;

        public ObservableCollection<LogStruct> LogStructs
        {
            get { return _logStructs; }
            set { _logStructs = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ProcedureParam> _procedureParamCollection;

        public ObservableCollection<ProcedureParam> ProcedureParamCollection
        {
            get { return _procedureParamCollection; }
            set { _procedureParamCollection = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<string> _showListCollection;

        public ObservableCollection<string> ShowListCollection
        {
            get { return _showListCollection; }
            set { _showListCollection = value; RaisePropertyChanged();}
        }



        public MainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IContainerProvider containerProvider, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _containerProvider = containerProvider;
            _dialogService = dialogService;

            LogStructs = new ObservableCollection<LogStruct>();

            _configureService = containerProvider.Resolve<IConfigureService>();

            eventAggregator.GetEvent<MessageEvent>().Subscribe((logStruct) =>
            {
                if (LogStructs.Count > 500)
                {
                    LogStructs.RemoveAt(0);
                }

                LogStructs.Add(logStruct);
            });

            NavigateCommand = new DelegateCommand<string>(Navigate);
            ShowDialogCommand = new DelegateCommand<string>(ShowDialog);
            HomeCommand = new DelegateCommand(() =>
            {
                _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("HomeView");
            });

            ShowListCollection = _configureService.SerializationData.ShowListCollection;
            ProcedureParamCollection = _configureService.SerializationData.ProcedureParams;
        }

        private void ShowDialog(string obj)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return;
            }

            _dialogService.ShowDialog(obj);
        }

        private void Navigate(string obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj))
            {
                return;
            }
            var view = _regionManager.Regions[PrismManager.MainViewRegionName].GetView(obj) as VisionProcessView;
            if (view == null)
            {
                MessageBox.Show("新添加视觉流程后，需要重启软件，才能打开对就的流程窗口");
                return;
            }

            _regionManager.Regions[PrismManager.MainViewRegionName].Activate(view);
        }

        public void Configure(bool isSave = false)
        {
            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("HomeView");

            if (ProcedureParamCollection == null || ProcedureParamCollection.Count <= 0)
            {
                return;
            }

            foreach (var procedureParam in ProcedureParamCollection)
            {
                var view = _containerProvider.Resolve(typeof(VisionProcessView), procedureParam.Name) as VisionProcessView;

                string file = $"config/Vision/{procedureParam.Name}/{procedureParam.Name}.json";

                if (view != null)
                {
                    view.DataContext = File.Exists(file)
                        ? JsonConvert.DeserializeObject<VisionProcessViewModel>(File.ReadAllText(file))
                        : new VisionProcessViewModel(_containerProvider, procedureParam.Name);

                    _regionManager.Regions[PrismManager.MainViewRegionName].Add(view, procedureParam.Name);
                }
            }
        }
    }
}
