using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GrupaEka.Models
{
    public class ArticleCommentViewModel
    {
        public int ArticleID { get; set; }

        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Treść")]
        public string Text { get; set; }
    }
}