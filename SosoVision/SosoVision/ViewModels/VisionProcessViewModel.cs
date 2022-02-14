using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using DryIoc;
using Prism.Mvvm;
using Prism.Regions;
using SosoVision.Extensions;

namespace SosoVision.ViewModels
{
    public class VisionProcessViewModel : BindableBase
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }

        public VisionProcessViewModel(string title)
        {
            Title = title;
        }

    }
}
