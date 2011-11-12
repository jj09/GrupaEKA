using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GrupaEka.Models
{
    public class Article
    {
        public Article()
        {
            this.ArticleComments = new List<ArticleComment>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Data jest wymagana")]
        [Display(Name="Data")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }        

        [Required(ErrorMessage = "Treść wiadomości jest wymagana")]
        [MinLength(20, ErrorMessage="Treść wiadomości musi mieć conajmniej 20 znaków.")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        [Display(Name = "Treść")]
        public string Text { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        public virtual ICollection<ArticleComment> ArticleComments { get; set; }
    }
}