using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using SupplyStuff.Models;

namespace SupplyStuff.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [DisplayName("Hub address")]
        public string Reference { get; set; }
        public string Role { get; set; }
        public string Id { get; set; }
    }
}