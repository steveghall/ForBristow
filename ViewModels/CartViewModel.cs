using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupplyStuff.ViewModels
{
    public class CartViewModel
    {
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}