using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SosoVision.Common;
using SosoVision.Extensions;

namespace SosoVision.ViewModels
{
    public class VisionProcessViewModel : BindableBase
    {
        private ProcedureParam _procedureParam;

        public ProcedureParam ProcedureParam
        {
            get { return _procedureParam; }
            set { _procedureParam = value; RaisePropertyChanged();}
        }

        public VisionProcessViewModel()
        {
            
        }

        public VisionProcessViewModel(IContainerProvider containerProvider,string title)
        {
            var config = containerProvider.Resolve<IConfigureService>();
            foreach (var param in config.SerializationData.ProcedureParams)
            {
                if (param.Name == title)
                {
                    ProcedureParam = param;
                }
            }
        }

    }
}
