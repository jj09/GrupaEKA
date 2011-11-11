using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GrupaEka.Models
{
    public class LectureTimeViewModel
    {
        public Lecture Lecture { get; set; }

        [MaxLength(2, ErrorMessage = "Godzina może się składać maksymalnie z dwóch cyfr.")]
        [Range(0, 23, ErrorMessage = "Podaj godzinę w zakresie 0-23")]
        public String Hour { get; set; }

        [MaxLength(2, ErrorMessage = "Minuty mogą się składać maksymalnie z dwóch cyfr.")]
        [Range(0, 59, ErrorMessage = "Podaj minuty w zakresie 0-59")]
        public String Minutes { get; set; }
    }
}
