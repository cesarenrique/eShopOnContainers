using Catalog.API.Controllers.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Controllers.Model
{
    public class CatalogContext : DbContext // it must inherit from DbContext
    {
        // For each Class from Model that we want to store in the database, we create a DbSet
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // we may call the fluent API to customize how we map classes to tables
            // https://docs.microsoft.com/es-es/ef/ef6/modeling/code-first/fluent/types-and-properties
            builder.Entity<CatalogItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(5,3)");

            builder.Entity<CatalogItem>()
                .HasAlternateKey(ci => ci.Name);
        }
    }
}