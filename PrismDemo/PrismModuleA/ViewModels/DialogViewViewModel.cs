using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using PrismModuleA.Events;

namespace PrismModuleA.ViewModels
{
    public class DialogViewViewModel : IDialogAware
    {
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DialogViewViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            CancelCommand = new DelegateCommand(Cancel);
            SaveCommand = new DelegateCommand(Save);
        }

        private void Save()
        {
            OnDialogClosed();
        }

        private void Cancel()
        {
            _eventAggregator.GetEvent<MessageEvent>().Publish("publish");
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            DialogParameters keys = new DialogParameters();
            keys.Add("Value", "Hello");
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, keys));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
                Title = parameters.GetValue<string>("Value");
        }

        public string Title { get; set; }
        public event Action<IDialogResult> RequestClose;
    }
}
