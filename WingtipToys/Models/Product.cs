using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WingtipToys.Models
{
    public class Product
    {
        [ScaffoldColumn(false)]
        public int ProductID { get; set; }

        [Required, StringLength(100), Display(Name = "Name")]
        public String ProductName { get; set; }

        [Required, StringLength(10000), Display(Name = "Product Desctiption")]
        public String Description { get; set; }

        public string ImagePath { get; set; }

        [Display(Name = "Price")]
        public double? UnitPrice { get; set; }

        public int? CatagoryID { get; set; }

        public virtual Category Category { get; set; }
    }
}