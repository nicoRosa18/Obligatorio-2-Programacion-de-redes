using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommonLogs;

namespace ServerLogs.Container
{
    public class LogContainer : ILogContainer
    {
        private readonly object padlock = new object();
        public ICollection<Log> Logs {get; set;}

        public LogContainer()
        {
            Logs = new Collection<Log>();
        }

        public void AddLog(Log log)
        {
            lock(padlock)
            {
                Console.WriteLine(log.EventType);    
                Logs.Add(log);
            }
        }

        public ICollection<Log> ShowLogs()
        {
            ICollection<Log> copy = new Collection<Log>();
            lock(padlock)
            {
                copy = Copy(Logs);
            }
            return copy;
        }

        private ICollection<Log> Copy(ICollection<Log> logs)
        {
            ICollection<Log> copy = new Collection<Log>();
            foreach(Log log in logs)
            {
                Log logCopy = new Log();
                logCopy.User = log.User;
                logCopy.Game = log.Game;
                logCopy.Time = log.Time;
                logCopy.Status = log.Status;
                copy.Add(logCopy);
            }
            return copy;
        }
    }
}
