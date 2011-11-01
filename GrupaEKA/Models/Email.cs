using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace GrupaEka.Models
{
    public class Email
    {
        public string From { get; set; }
        public string EmailAddress { get; set; }
        public string Message { get; set; }        
    }
}