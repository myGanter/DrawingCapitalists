using Core.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.DB.MsSql
{
    public class MsSqlApplicationContext : AppDBContext
    {
        public MsSqlApplicationContext(DbContextOptions<MsSqlApplicationContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserState>().HasKey(x => new { x.Name, x.FingerPrint });
        }
    }
}
