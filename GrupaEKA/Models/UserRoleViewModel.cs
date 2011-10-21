using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrupaEka.Models
{
    public class UserRoleViewModel
    {
        public String UserName { get; set; }
        public String newUserName { get; set; }
        public String Email { get; set; }
        public String Role { get; set; }
        public bool IsApproved { get; set; }
    }
}