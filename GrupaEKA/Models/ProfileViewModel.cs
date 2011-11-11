using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GrupaEka.Models
{
    public class ProfileViewModel
    {
        public Profile Profile { get; set; }

        public Int32[] CategoryIDs { get; set; }
    }
}