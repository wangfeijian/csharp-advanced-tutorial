using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace MySqlDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var customerModel = MySqlHelper.FindSingleData<CustomersModel>(10006);
                Show(customerModel);

                var newCustomerModel = MySqlHelper.FindSingleData<ProductsModel>("ANV01");
                Show(newCustomerModel);

                var obj = MySqlHelper.FindAllData<OrderItemsModel>(20005);
                ShowAll(obj);

                var vendorList = MySqlHelper.FindAllData<VendorsModel>();
                ShowAll(vendorList);

                var products = MySqlHelper.FindAllData<ProductsModel>();
                ShowAll(products);

                Console.WriteLine(MySqlHelper.UpdateData<CustomersModel>("cust_name = 'Wangfeijian',cust_email = 'wangfeijianhao@163.com'",10005)
                        ? "update success": "update fail");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void Show<T>(T tObj)
        {
            foreach (var property in tObj.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Validate()}:{property.GetValue(tObj)}");
            }

            Console.WriteLine();
        }

        static void ShowAll<T>(List<T> tObj)
        {
            foreach (var item in tObj)
            {
                foreach (var property in item.GetType().GetProperties())
                {
                    Console.WriteLine($"{property.Validate()}:{property.GetValue(item)}");
                }

                Console.WriteLine();
            }
        }
    }
}
