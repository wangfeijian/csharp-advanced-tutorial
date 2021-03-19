using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class CustomersModel : BaseModel
    {
        [Remark("cust_id")]
        public override int Id { get; set; }
        public string cust_name { get; set; }
        public string cust_address { get; set; }
        public string cust_city { get; set; }
        public string cust_state { get; set; }
        public string cust_zip { get; set; }
        public string cust_country { get; set; }
        public string cust_contact { get; set; }
        public string cust_email { get; set; }
    }

    public class NewCustomersModel : BaseModel
    {
        [Remark("cust_newid")]
        public override int Id { get; set; }
        public string cust_name { get; set; }
        public string cust_address { get; set; }
        public string cust_city { get; set; }
        public string cust_state { get; set; }
        public string cust_zip { get; set; }
        public string cust_country { get; set; }
        public string cust_contact { get; set; }
        public string cust_email { get; set; }
    }

    public class OrderItemsModel : BaseModel
    {
        [Remark("order_num")]
        public override int Id { get; set; }
        public int order_item { get; set; }
        public string prod_id { get; set; }
        public int quantity { get; set; }
        public decimal item_price { get; set; }
    }

    public class OrdersModel : BaseModel
    {
        [Remark("order_num")]
        public override int Id { get; set; }
        public DateTime order_date { get; set; }
        public int cust_id { get; set; }
    }

    public class ProductNotesModel : BaseModel
    {
        [Remark("note_id")]
        public override int Id { get; set; }
        public string prod_id { get; set; }
        public DateTime note_date { get; set; }
        public string note_text { get; set; }
    }

    public class ProductsModel 
    {
        [Remark("prod_id")]
        public string Id { get; set; }
        public int vend_id { get; set; }
        public string prod_name { get; set; }
        public decimal prod_price { get; set; }
        public string prod_desc { get; set; }
    }

    public class VendorsModel : BaseModel
    {
        [Remark("vend_id")]
        public override int Id { get; set; }
        public string vend_name { get; set; }
        public string vend_address { get; set; }
        public string vend_city { get; set; }
        public string vend_state { get; set; }
        public string vend_zip { get; set; }
        public string vend_country { get; set; }
    }
}
