using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GrupaEka.Models
{
    public class NewsCategory
    {
        public NewsCategory()
        {
            this.News = new List<News>();
            this.Profiles = new List<Profile>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Podaj nazwę kategorii")]
        [MaxLength(30)]
        public string Name { get; set; }

        public virtual ICollection<News> News { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }
    }
}