using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SupplyStuff.Models
{
    public class Order
    {
        [DisplayName("Order number")]
        public int OrderId { get; set; }
        [DisplayName("User")]
        public string UserId { get; set; }
        [DisplayName("Order date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }
        public Boolean Complete { get; set; }
        public string Instructions { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}