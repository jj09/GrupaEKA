using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace GrupaEka.Models
{
    public class LectureComment
    {
        public int ID { get; set; }

        public int Lecture_ID { get; set; }

        [ForeignKey("Lecture_ID")]
        public virtual Lecture Lecture { get; set; }

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
