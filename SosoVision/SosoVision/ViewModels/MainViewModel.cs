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
        public DelegateCommand<ProcedureParam> NavigateCommand { get; }
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

            NavigateCommand = new DelegateCommand<ProcedureParam>(Navigate);
            ShowDialogCommand = new DelegateCommand<string>(ShowDialog);
            HomeCommand = new DelegateCommand(() =>
            {
                _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("HomeView");
            });

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

        private void Navigate(ProcedureParam obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.Name))
            {
                return;
            }

            _regionManager.Regions[PrismManager.MainViewRegionName].Activate(_containerProvider.Resolve(typeof(VisionProcessView), obj.Name));
        }

        public void Configure(bool isSave = false)
        {
            foreach (var procedureParam in ProcedureParamCollection)
            {
                var view = _containerProvider.Resolve(typeof(VisionProcessView), procedureParam.Name) as VisionProcessView;
                // todo 这里首先读一下文件，看是否有配置
                view.DataContext = new VisionProcessViewModel(procedureParam.Name);
                _regionManager.Regions[PrismManager.MainViewRegionName].Add(view);
            }

            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("HomeView");
        }
    }
}
