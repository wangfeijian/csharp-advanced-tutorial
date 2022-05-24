using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace WriteExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            IWorkbook book1 = new XSSFWorkbook(new FileStream(@"D:\Work\工作计划\1.xlsx", FileMode.Open));
            //IWorkbook book2 = new XSSFWorkbook(new FileStream("file2.xls", FileMode.Open));
            IWorkbook product = new XSSFWorkbook(@"D:\Work\工作计划\TEST.xlsx");

            //for (int i = 0; i < book1.NumberOfSheets; i++)
            //{
                ISheet sheet1 = book1.GetSheetAt(0);
                sheet1.CopyTo(product, "test", true, true);
            //}

            //for (int j = 0; j < book2.NumberOfSheets; j++)
            //{
            //    ISheet sheet2 = book2.GetSheetAt(j);
            //    sheet2.CopyTo(product, sheet2.SheetName, true, true);
            //}

            product.Write(new FileStream(@"D:\Work\工作计划\TEST.xlsx", FileMode.Create, FileAccess.ReadWrite));
        }
    }
}
