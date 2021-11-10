using System;
using CommonLogs;

namespace ServerLogs.Container
{
    public interface ILogContainer
    {
        void AddLog(Log log);
    }
}
