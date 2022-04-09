using Core.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Core.Services.DB.MsSql
{
    public class MsSqlApplicationContext : AppDBContext
    {
        public MsSqlApplicationContext(DbContextOptions<MsSqlApplicationContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            if (Database.EnsureCreated())
            {
                //запись базовых слов в бд             
                var wordCollectionsInitSql = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/SqlScripts/MsSqlWordCollectionsInit.sql");
                Database.ExecuteSqlRaw(wordCollectionsInitSql);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserState>().HasKey(x => new { x.Name, x.FingerPrint });            
        }
    }
}
