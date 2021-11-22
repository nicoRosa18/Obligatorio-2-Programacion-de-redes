using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonLogs;

namespace ServerLogs.Container
{
    public interface ILogContainer
    {
        Task AddLogAsync(Log log);
        Task<ICollection<Log>> FilterLogsAsync(string user, string game, string date);
    }
}
