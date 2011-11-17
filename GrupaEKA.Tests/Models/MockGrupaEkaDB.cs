using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrupaEka.Models;
using System.Data.Entity;
using System.Reflection;
using GrupaEka.Tests.Models;

namespace GrupaEka.Tests.Models
{
    public class MockGrupaEkaDB : IGrupaEkaDB
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

        public bool saved = false;

        public MockGrupaEkaDB()
        {
            // Set up your collections
            News = new MockDbSet<News>();
            NewsCategories = new MockDbSet<NewsCategory>();
            NewsComments = new MockDbSet<NewsComment>();
            Lectures = new MockDbSet<Lecture>();
            LectureComments = new MockDbSet<LectureComment>();
            Articles = new MockDbSet<Article>();
            ArticleComments = new MockDbSet<ArticleComment>();
            Projects = new MockDbSet<Projects>
            {
                new Projects {ID=1, Content="Nasze projekty"}
            };
            Profiles = new MockDbSet<Profile>();
            PasswordResets = new MockDbSet<PasswordReset>();
        }

        public IDbSet<T> Set<T>() where T : class
        {
            foreach (PropertyInfo property in typeof(MockGrupaEkaDB).GetProperties())
            {
                if (property.PropertyType == typeof(IDbSet<T>))
                    return property.GetValue(this, null) as IDbSet<T>;
            }
            throw new Exception("Type collection not found");
        }

        public virtual int SaveChanges()
        {
            saved = true;
            return 0;
        }        
    }

}
