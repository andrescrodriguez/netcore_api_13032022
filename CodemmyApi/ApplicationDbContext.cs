using CodemmyApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImagenXArticulo>()
                .HasKey(pc => new { pc.ImagenId, pc.ArticuloId });

            modelBuilder.Entity<ImagenXArticulo>()
                .HasOne(pc => pc.Imagen)
                .WithMany(p => p.ImagenesXArticulos)
                .HasForeignKey(pc => pc.ImagenId);

            modelBuilder.Entity<ImagenXArticulo>()
               .HasOne(pc => pc.Articulo)
               .WithMany(p => p.ImagenesXArticulos)
               .HasForeignKey(pc => pc.ArticuloId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Imagen> Imagen { get; set; }
        public DbSet<ImagenXArticulo> ImagenXArticulo { get; set; }
    }
}
