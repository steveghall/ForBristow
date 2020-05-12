using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SupplyStuff.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Supplier { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Received { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Expiry { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
    }
}