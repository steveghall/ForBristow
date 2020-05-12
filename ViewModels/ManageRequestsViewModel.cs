using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SupplyStuff.ViewModels
{
    public class ManageRequestsViewModel
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Order date")]
        public DateTime Orderdate { get; set; }
        public int Quantity { get; set; }
        public bool Status { get; set; }
    }
}