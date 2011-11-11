using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GrupaEka.Models
{
    public class Profile
    {
        public Profile()
        {
            this.Subscriptions = new List<NewsCategory>();
        }

        public int ID { get; set; }

        [Required]
        [Display(Name = "Użytkownik")]
        public string UserName { get; set; }

        [Display(Name="Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "O sobie")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string About { get; set; }

        [Display(Name = "Rok studiów")]
        public int? StudyYear { get; set; }

        [Display(Name = "Kierunek")]
        public string StudyMajor { get; set; }

        [Display(Name = "Wydział")]
        public string Faculty { get; set; }

        [Display(Name = "Zdjęcie")]
        public string Photo { get; set; }

        [Display(Name = "Subskybowane kategorie")]
        public virtual ICollection<NewsCategory> Subscriptions { get; set; }
    }
}