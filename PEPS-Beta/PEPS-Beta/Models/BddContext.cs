using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PEPS_Beta.Models
{
	public class BddContext : DbContext
	{
        public DbSet<Indice> Indices { get; set; }
        public DbSet<MultiMondeParam> Parametres { get; set; }
	}
}