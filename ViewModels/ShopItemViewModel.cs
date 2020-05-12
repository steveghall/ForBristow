using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace SupplyStuff.ViewModels
{
    public class ShopItemViewModel
    {
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int AddNo { get; set; }
    }
}   