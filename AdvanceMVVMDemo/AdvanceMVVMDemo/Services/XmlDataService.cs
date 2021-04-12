using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AdvanceMVVMDemo.Models;

namespace AdvanceMVVMDemo.Services
{
    class XmlDataService:IDataService
    {
        public List<Dish> GetAllDishes()
        {
            List<Dish> dishList = new List<Dish>();
            string xmlFileName = System.IO.Path.Combine(Environment.CurrentDirectory, @"Data\Data.xml");
            XDocument xDoc = XDocument.Load(xmlFileName);
            var dishes = xDoc.Descendants("Dish");
            foreach (var d in dishes)
            {
                Dish dish = new Dish
                {
                    Name     = d.Element("Name")?.Value,
                    Category = d.Element("Category")?.Value,
                    Comment  = d.Element("Comment")?.Value,
                    Score    = double.Parse(d.Element("Score").Value)
                };
                dishList.Add(dish);
            }

            return dishList;
        }
    }
}
