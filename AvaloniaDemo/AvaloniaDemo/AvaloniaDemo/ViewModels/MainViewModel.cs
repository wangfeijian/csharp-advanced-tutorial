using AvaloniaDemo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaloniaDemo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _contentViewModel;
    public string Greeting => "Temperature Converter";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Fahrenheit))]
    private double _celsius;
    public double Fahrenheit => Celsius * (9d / 5d) + 32;
    public ICommand CalcCommand => new AsyncRelayCommand(() =>
    {
        Debug.WriteLine($"Clicked! Celsius={Celsius}");
        return Task.CompletedTask;
    });
    public ToDoListViewModel ToDoList { get; }

    public MainViewModel()
    {
        var services = new ToDoListService();
        ToDoList = new ToDoListViewModel(services.GetItems());
        //ContentViewModel = new ToDoListView() { DataContext = ToDoList };
        ContentViewModel = ToDoList;
    }

    public void AddItem()
    {
        ContentViewModel = new AddItemViewModel();
        //ContentViewModel = new AddItemView() { DataContext = new AddItemViewModel() };
    }
}
