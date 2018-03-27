using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEPS_Beta.Models
{
	public class BddContext : DbContext
	{
        public DbSet<Indice> Indices { get; set; }
        public DbSet<IndexesAtDate> IndexesValue { get; set; }
        public DbSet<MultiMondeParam> Parametres { get; set; }
        public DbSet<TauxDeChange> GetTaux { get; set; }
        public DbSet<PortefeuilleCouverture> GetPort { get; set; }
        public DbSet<PortefeuilleIdeal> GetOptim { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<BddContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}