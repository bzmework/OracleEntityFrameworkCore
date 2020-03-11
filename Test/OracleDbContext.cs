using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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

            // oracle 11g 数据库
            optionsBuilder.UseOracle("DATA SOURCE=127.0.0.1:1521/orcl;PASSWORD=gold;PERSIST SECURITY INFO=True;USER ID=gadata0001", b => b.UseOracleSQLCompatibility("11"));
            //optionsBuilder.UseOracle("DATA SOURCE=139.9.149.38:1521/ORCL;USER ID=CEDATA0001;PASSWORD=ce123456;PERSIST SECURITY INFO=True", b => b.UseOracleSQLCompatibility("11"));

            // oracle 12c 数据库
            //optionsBuilder.UseOracle("DATA SOURCE=127.0.0.1:1521/orcl;PASSWORD=gold;PERSIST SECURITY INFO=True;USER ID=gadata0001");

            base.OnConfiguring(optionsBuilder);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Modules>().Property(t => t.Createdtime).ValueGeneratedOnAdd();
        //    modelBuilder.Entity<Modules>().Property(t => t.ModifiedTime).ValueGeneratedOnAdd();
        //}

        public DbSet<Modules> Modules { get; set; }

        public DbSet<Rights> Rights { get; set; }

        public DbSet<DepartmentType> DepartmentTypes { get; set; }
    }
}
