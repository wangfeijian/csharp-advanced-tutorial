using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ImTools;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SosoVision.Common;
using SosoVision.Extensions;

namespace SosoVision.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {
        private readonly IRegionManager _regionManager;
        private readonly IContainerRegistry _containerRegistry;
        public DelegateCommand<string> NavigateCommand { get; }

        public DelegateCommand AddVisionStepCommand { get; }
        public DelegateCommand SubVisionStepCommand { get; }

        private ObservableCollection<LogStruct> _logStructs;

        public ObservableCollection<LogStruct> LogStructs
        {
            get { return _logStructs; }
            set { _logStructs = value; RaisePropertyChanged(); }
        }

        private string _addVisionKeyWord;

        public string AddVisionKeyWord
        {
            get { return _addVisionKeyWord; }
            set { _addVisionKeyWord = value; RaisePropertyChanged();}
        }

        private ObservableCollection<string> _visionStepCollection;

        public ObservableCollection<string> VisionStepCollection
        {
            get { return _visionStepCollection; }
            set { _visionStepCollection = value; RaisePropertyChanged();}
        }


        public MainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IContainerRegistry containerRegistry)
        {
            _regionManager = regionManager;
            _containerRegistry = containerRegistry;

            LogStructs = new ObservableCollection<LogStruct>();

            VisionStepCollection = new ObservableCollection<string>();

            eventAggregator.GetEvent<MessageEvent>().Subscribe((logStruct) =>
            {
                if (LogStructs.Count > 500)
                {
                    LogStructs.RemoveAt(0);
                }

                LogStructs.Add(logStruct);
            });

            NavigateCommand = new DelegateCommand<string>(Navigate);
            AddVisionStepCommand = new DelegateCommand(AddVisionStep);
            SubVisionStepCommand = new DelegateCommand(SubVisionStep);
        }

        private void SubVisionStep()
        {
            if (string.IsNullOrWhiteSpace(AddVisionKeyWord))
            {
                return;
            }

            if (!VisionStepCollection.Contains(AddVisionKeyWord))
            {
                MessageBox.Show("不存在此流程，请输入一个已有的流程", "提示");
            }

            VisionStepCollection.Remove(AddVisionKeyWord);
        }

        private void AddVisionStep()
        {
            if (string.IsNullOrWhiteSpace(AddVisionKeyWord))
            {
                return;
            }

            if (VisionStepCollection.Contains(AddVisionKeyWord))
            {
                MessageBox.Show("已经存在此流程，请输入不同的名称", "提示");
            }

            VisionStepCollection.Add(AddVisionKeyWord);

        }

        private void Navigate(string obj)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return;
            }
            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj);
        }

        public void Configure()
        {
            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("HomeView");
        }
    }
}
