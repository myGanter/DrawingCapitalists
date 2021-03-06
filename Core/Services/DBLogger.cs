﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Models.DB;
using Core.Models;
using Core.Expansions;

namespace Core.Services
{
    public class DBLogger : ILogger
    {
        private object ScopeState { get; set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            ScopeState = state;

            var scope = new DBLoggerScope();
            scope.OnDispose += () => ScopeState = null;

            return scope;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = new Log()
            {
                Type = logLevel,
                Time = DateTime.Now,
                Message = formatter(state, exception)
            };

            if (ScopeState.IsNotNull())
                log.Context = ScopeState.ToString();

            if (state is UserObjectContainer us)
            {
                log.User = us.User;
                log.RequestId = us.RequestId;
            }          

            if (exception.IsNotNull())
            {
                log.StackTrace = exception.StackTrace;
                log.ExceptionMessage = $"{exception.Message} \nInnerException:[{exception.InnerException?.Message}]";
            }

            DBLoggerQueue.Add(log);
        }
    }
}
