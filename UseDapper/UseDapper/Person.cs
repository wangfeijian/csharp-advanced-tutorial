using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseDapper
{
    public class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Mail { get; set; }
        public string Address { get; set; }
    }
}
