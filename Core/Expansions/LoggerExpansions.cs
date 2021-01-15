using Core.Models;
using Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Expansions
{
    public static class LoggerExpansions
    {
        public static ILoggerFactory AddDBLogger(this ILoggerFactory factory)
        {
            factory.AddProvider(new DBLoggerProvider());
            return factory;
        }

        public static void WriteLog<TState>(this ILogger logger, LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter, object scopeState)
        {
            using (logger.BeginScope(scopeState))
                logger.Log(logLevel, new EventId(), state, exception, formatter);
        }

        public static void WriteLog<TState>(this ILogger logger, LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logger.Log(logLevel, new EventId(), state, exception, formatter);
        }
    }
}
