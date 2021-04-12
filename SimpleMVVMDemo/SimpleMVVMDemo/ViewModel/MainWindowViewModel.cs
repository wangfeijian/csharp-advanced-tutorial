using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Microsoft.Win32;
using SimpleMVVMDemo.Commands;

namespace SimpleMVVMDemo.ViewModel
{
    class MainWindowViewModel : NotificationObject
    {
        private double input1;

        public double Input1
        {
            get { return input1; }
            set
            {
                input1 = value;
                OnPropertyChanged(nameof(Input1));
            }
        }

        private double input2;

        public double Input2
        {
            get { return input2; }
            set
            {
                input2 = value;
                OnPropertyChanged(nameof(Input2));
            }
        }

        private double result;

        public double Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        private string selectValue;

        public string SelectValue
        {
            get { return selectValue; }
            set
            {
                selectValue = value;
                OnPropertyChanged(nameof(SelectValue));
            }
        }


        public List<string> ListData { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand SelectCommand { get; set; }

        public MainWindowViewModel()
        {
            AddCommand = new DelegateCommand { ActionExecute = Add };
            SaveCommand = new DelegateCommand { ActionExecute = Save };
            SelectCommand = new DelegateCommand { ActionExecute = Select };
            ListData = new List<string> { "apple", "pear", "orange", "banana" };
        }

        private void Add(object parameter)
        {
            Result = Input1 + Input2;
        }

        private void Save(object parameter)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.ShowDialog();
        }

        private void Select(object parameter)
        {
            SelectValue = parameter.ToString();
        }
    }
}
