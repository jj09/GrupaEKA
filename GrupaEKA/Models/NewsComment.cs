using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace GrupaEka.Models
{
    public class NewsComment
    {
        public int ID { get; set; }

        public int News_ID { get; set; }

        [ForeignKey("News_ID")]
        public virtual News News { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Treść")]
        public string Text { get; set; }

        
    }
}
