using Core.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.DB
{
    public abstract class AppDBContext : DbContext, IAppDBContext
    {
        public DbSet<Log> Logs { get; set; }

        public DbSet<UserState> UserStates { get; set; }

        public DbSet<UserConfigure> UserConfigures { get; set; }

        public DbSet<WordCollection> WordCollections { get; set; }

        public DbSet<Word> Words { get; set; }

        public AppDBContext(DbContextOptions options) : base(options)
        { }
    }
}
