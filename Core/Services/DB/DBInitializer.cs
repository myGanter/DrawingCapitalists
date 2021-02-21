using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Core.Models.DB;

namespace Core.Services.DB
{
    public class DBInitializer : IDisposable
    {
        private readonly AppDBContext Context;

        public DBInitializer(AppDBContext context)
        {
            Context = context;
        }

        public void Initialize()
        {
            //todo
            Context.Logs.Add(new Log() 
            {
                Time = DateTime.Now,
                Type = Microsoft.Extensions.Logging.LogLevel.Information,
                Context = "Start app",
                Message = "Hello DB!"               
            });

            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
