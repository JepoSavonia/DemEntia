using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DemEntiaHRApplication.Models;

namespace DemEntiaHRApplication.Data
{
    public class DementiaContext : DbContext
    {
        public DementiaContext(DbContextOptions<DementiaContext> options) : base(options)
        {
        }

        public DbSet<ResetPass> ResetPass { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResetPass>().ToTable("ResetPass");

        }
    }

    

}
