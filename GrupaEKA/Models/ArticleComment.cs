using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GrupaEka.Models
{
    public class ArticleComment
    {
        public int ID { get; set; }

        public int Article_ID { get; set; }

        [ForeignKey("Article_ID")]
        public virtual Article Article { get; set; }

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