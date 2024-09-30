using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using TradeProject.Model.DBO;

namespace TradeProject.Model
{
    internal class TradeDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public TradeDbContext() { }
        public TradeDbContext(DbContextOptions<TradeDbContext> options)
        : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (baseDir.Contains("bin"))
            {
                int index = baseDir.IndexOf("bin");
                baseDir = baseDir.Substring(0, index);
            }
            optionsBuilder.ConfigureWarnings(w => w.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning)).UseLazyLoadingProxies().UseSqlite($"Data Source={baseDir}\\Trade.db");
        }
    }
}
