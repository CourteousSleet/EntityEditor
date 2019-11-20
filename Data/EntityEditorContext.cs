using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EntityEditor.Models;

namespace EntityEditor.Data
{
    public class EntityEditorContext : DbContext
    {
        public EntityEditorContext (DbContextOptions<EntityEditorContext> options)
            : base(options)
        {
        }

        public DbSet<EntityEditor.Models.Client> Clients { get; set; }
        public DbSet<EntityEditor.Models.Founder> Founders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Founder>().ToTable("Founder");
        }
    }
}
