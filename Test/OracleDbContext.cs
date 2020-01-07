using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsoleApp
{
    public class OracleDbContext : DbContext
    {
        //public OracleDbContext(DbContextOptions options) : base(options)
        //{
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var logger = new LoggerFactory();
            //logger.AddConsole();
            optionsBuilder.UseLoggerFactory(logger);
            optionsBuilder.UseOracle("DATA SOURCE=127.0.0.1:1521/orcl;USER ID=scott;PASSWORD=tiger;PERSIST SECURITY INFO=True;",
                b => b.UseOracleSQLCompatibility("11"));
            base.OnConfiguring(optionsBuilder);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Modules>().Property(t => t.Createdtime).ValueGeneratedOnAdd();
        //    modelBuilder.Entity<Modules>().Property(t => t.ModifiedTime).ValueGeneratedOnAdd();
        //}

        public DbSet<Modules> Modules { get; set; }

        public DbSet<Rights> Rights { get; set; }
    }
}
