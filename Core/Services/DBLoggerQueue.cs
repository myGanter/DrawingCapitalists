using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Models.DB;

namespace Core.Services
{
    internal static class DBLoggerQueue
    {
        private static readonly Queue<Log> LogQueue;

        private static readonly object Locker;

        static DBLoggerQueue()
        {
            LogQueue = new Queue<Log>();
            Locker = new object();
        }

        public static void Add(Log obj)
        {
            lock (Locker)
                LogQueue.Enqueue(obj);
        }

        public static List<Log> GetAll()
        {
            var res = new List<Log>();

            lock (Locker)
            {
                while (LogQueue.Count > 0)
                    res.Add(LogQueue.Dequeue());
            }

            return res;
        }

        public static int Count 
        { 
            get
            {
                lock (Locker)
                    return LogQueue.Count;
            } 
        }
    }
}
