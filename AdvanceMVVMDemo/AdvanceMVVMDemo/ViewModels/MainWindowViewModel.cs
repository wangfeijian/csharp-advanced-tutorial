using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AdvanceMVVMDemo.Models;
using AdvanceMVVMDemo.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace AdvanceMVVMDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand PlaceOrderCommand { get; set; }
        public ICommand SelectMenuItemCommand { get; set; }

        private int count;

        public int Count
        {
            get { return count; }
            set { Set(ref count, value); }
        }

        private Restaurant restaurant;

        public Restaurant Restaurant
        {
            get { return restaurant; }
            set { Set(ref restaurant, value); }
        }

        private List<DishMenuItemViewModel> dishMenu;

        public List<DishMenuItemViewModel> DishMenu
        {
            get { return dishMenu; }
            set { Set(ref dishMenu, value); }
        }


        public MainWindowViewModel()
        {
            LoadRestaurant();
            LoadDishMenu();
            PlaceOrderCommand = new RelayCommand(PlaceOrderCommandExecute);
            SelectMenuItemCommand = new RelayCommand(SelectMenuItemExecute);
        }

        private void LoadRestaurant()
        {
            Restaurant = new Restaurant
            {
                Name = "Crazy大象",
                Address = "北京市海淀区万泉河路紫金庄园1号楼1层113室（亲们：这地儿特难找！）",
                PhoneNumber = "15210365423 or 82650336"
            };
        }

        private void LoadDishMenu()
        {
            XmlDataService ds = new XmlDataService();
            var dishes = ds.GetAllDishes();
            DishMenu = new List<DishMenuItemViewModel>();
            foreach (var dish in dishes)
            {
                DishMenuItemViewModel item = new DishMenuItemViewModel { Dish = dish };
                DishMenu.Add(item);
            }
        }

        private void PlaceOrderCommandExecute()
        {
            var selectedDishes = DishMenu.Where(i => i.IsSelected).Select(i => i.Dish.Name).ToList();
            IOrderService orderService = new MockOrderService();
            orderService.PlaceOrder(selectedDishes);
            MessageBox.Show("订餐成功！");
        }

        private void SelectMenuItemExecute()
        {
            Count = DishMenu.Count(i => i.IsSelected);
        }
    }
}
