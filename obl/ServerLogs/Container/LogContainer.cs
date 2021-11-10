using System;
using System.Collections.Generic;
using CommonLogs;

namespace ServerLogs.Container
{
    public class LogContainer
    {
        public List<Log> Logs {get; set;}

        LogContainer()
        {
            Logs = new List<Log>();
        }

        public void AddLog(Log log)
        {
            Console.WriteLine(log.EventType);    
            Logs.Add(log);
        }
    }
}
