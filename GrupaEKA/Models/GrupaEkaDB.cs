using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace GrupaEka.Models
{
    public class GrupaEkaDB : DbContext
    {
        public DbSet<News> News { get; set; }
    }
}