using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

using Core.Models.DB;

namespace Core.Services.DB
{    
    public interface IAppDBContext : IDisposable
    {
        DbSet<Log> Logs { get; set; }

        DbSet<UserState> UserStates { get; set; }

        DbSet<UserConfigure> UserConfigures { get; set; }
    }
}
