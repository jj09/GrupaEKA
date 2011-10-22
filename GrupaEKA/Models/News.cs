using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GrupaEka.Models
{
    public class News
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Data jest wymagana")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Tytuł jest wymagany")]
        public string Title { get; set; }        

        [Required(ErrorMessage = "Treść wiadomości jest wymagana")]
        [MinLength(20)]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Text { get; set; }

        public string Author { get; set; }
    }
}