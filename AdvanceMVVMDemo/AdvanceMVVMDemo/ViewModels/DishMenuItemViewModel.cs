using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvanceMVVMDemo.Models;

namespace AdvanceMVVMDemo.ViewModels
{
    public class DishMenuItemViewModel
    {
        public Dish Dish { get; set; }
        public bool IsSelected { get; set; }
            
    }
}
