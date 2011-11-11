﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GrupaEka.Models
{
    public class GrupaEkaDB : DbContext
    {
        public DbSet<News> News { get; set; }
        public DbSet<NewsCategory> NewsCategories { get; set; }
        public DbSet<NewsComment> NewsComments { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
    }
}