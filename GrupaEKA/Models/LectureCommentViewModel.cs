using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GrupaEka.Models
{
    public class LectureCommentViewModel
    {
        public int LectureID { get; set; }

        [Required(ErrorMessage = "Treść komentarza jest wymagana.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Treść")]
        public string Text { get; set; }


    }
}