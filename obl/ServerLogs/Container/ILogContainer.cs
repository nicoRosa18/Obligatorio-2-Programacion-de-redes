using System;
using System.Collections.Generic;
using CommonLogs;

namespace ServerLogs.Container
{
    public interface ILogContainer
    {
        void AddLog(Log log);
        ICollection<Log> ShowLogs();
    }
}
