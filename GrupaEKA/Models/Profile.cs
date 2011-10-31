using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GrupaEka.Models
{
    public class Profile
    {
        public int ID { get; set; }

        [Required]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string About { get; set; }
        
        public int StudyYear { get; set; }
        public string StudyMajor { get; set; }
        public string Faculty { get; set; }        
    }
}