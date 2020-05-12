using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupplyStuff.ViewModels
{
    public class EditUserViewModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Reference { get; set; }
        public string RoleId { get; set; }
        public string Id { get; set; }
    }
}