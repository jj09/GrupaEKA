using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GrupaEka.Models
{
    public interface IGrupaEkaDB
    {
        IDbSet<News> News { get; set; }
        IDbSet<NewsCategory> NewsCategories { get; set; }
        IDbSet<NewsComment> NewsComments { get; set; }
        IDbSet<Lecture> Lectures { get; set; }
        IDbSet<LectureComment> LectureComments { get; set; }
        IDbSet<Article> Articles { get; set; }
        IDbSet<ArticleComment> ArticleComments { get; set; }
        IDbSet<Projects> Projects { get; set; }
        IDbSet<Profile> Profiles { get; set; }
        IDbSet<PasswordReset> PasswordResets { get; set; }
        int SaveChanges();
    }
}