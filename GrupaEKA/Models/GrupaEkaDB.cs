using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GrupaEka.Models
{
    public class GrupaEkaDB : DbContext, IGrupaEkaDB
    {
        public IDbSet<News> News { get; set; }
        public IDbSet<NewsCategory> NewsCategories { get; set; }
        public IDbSet<NewsComment> NewsComments { get; set; }
        public IDbSet<Lecture> Lectures { get; set; }
        public IDbSet<LectureComment> LectureComments { get; set; }
        public IDbSet<Article> Articles { get; set; }
        public IDbSet<ArticleComment> ArticleComments { get; set; }
        public IDbSet<Projects> Projects { get; set; }
        public IDbSet<Profile> Profiles { get; set; }
        public IDbSet<PasswordReset> PasswordResets { get; set; }
    }
}