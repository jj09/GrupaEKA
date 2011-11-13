using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GrupaEka.Models
{
    public class Projects
    {
        public int ID { get; set; }

        [AllowHtml]
        public string Content { get; set; }
    }
}