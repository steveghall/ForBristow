using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SupplyStuff.Models;

namespace SupplyStuff.ViewModels
{
    public class ShopViewModel
    {
        public IEnumerable<ShopItemViewModel> Items { get; set; }
        public List<CartViewModel> CartItems { get; set; }
    }
}