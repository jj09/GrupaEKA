using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrupaEka.Models
{
    public class PasswordReset
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}