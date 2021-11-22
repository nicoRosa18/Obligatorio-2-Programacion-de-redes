using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommonLogs;

namespace ServerLogs.Container
{
    public class LogContainer : ILogContainer
    {
        private readonly object _gameDictionarylock = new object();
        private readonly object _userDictionarylock = new object();
        private IDictionary<string, List<Log>> _gameLogs {get; set;}
        private IDictionary<string, List<Log>> _userLogs {get; set;}

        public LogContainer()
        {
            _gameLogs = new Dictionary<string, List<Log>>();
            _userLogs = new Dictionary<string, List<Log>>();
        }

        public async Task AddLogAsync(Log log)
        {
            lock(_userLogs)
            {
                try
                {
                    _userLogs[log.User].Add(DeepCopyLog(log));
                }
                catch(System.Collections.Generic.KeyNotFoundException)
                {
                    List<Log> newCollectionToKey = new List<Log>();
                    newCollectionToKey.Add(DeepCopyLog(log));
                    _userLogs.Add(log.User, newCollectionToKey);
                }
            }
            if(!log.Game.Equals(string.Empty))
            {
                lock(_gameLogs)
                {
                    try
                    {
                        _gameLogs[log.Game].Add(DeepCopyLog(log));
                    }
                    catch(System.Collections.Generic.KeyNotFoundException)
                    {
                        List<Log> newCollectionToKey = new List<Log>();
                        newCollectionToKey.Add(DeepCopyLog(log));
                        _gameLogs.Add(log.Game, newCollectionToKey);
                    }
                }
            }
        }

        public async Task<ICollection<Log>> FilterLogsAsync(string user, string game, string date)
        {
            List<Log> filteredByUser = new List<Log>();
            List<Log> filteredByGame = new List<Log>();
            lock(_userLogs)
            {
                try
                {
                    filteredByUser = DeepCopyLogList(FilterByUserName(user));
                }
                catch(System.Collections.Generic.KeyNotFoundException)
                {
                    return filteredByUser;
                }
            }
            lock(_gameLogs)
            {
                try
                {
                    filteredByGame = DeepCopyLogList(FilterByGameName(game));
                }
                catch(System.Collections.Generic.KeyNotFoundException)
                {
                    return filteredByGame;
                }
            }
            List<Log> filtered  = new List<Log>();
            if(filteredByUser.Count == 0)
            {
                filtered = filteredByGame;
            }
            else if(filteredByGame.Count == 0)
            {
                filtered = filteredByUser;
            }
            else
            {
                filtered = Intersect(filteredByUser, filteredByGame);
            }
            return FilterByDate(filtered, date);
        }

        private List<Log> Intersect(List<Log> filteredByUser, List<Log> filteredByGame)
        {
            List<Log> toReturn = new List<Log>();
            foreach (Log log in filteredByUser)
            {
                if (filteredByGame.Contains(log)) toReturn.Add(log);
            }
            return toReturn;
        }

        private List<Log> FilterByUserName(string userName)
        {
            List<Log> listFiltered = new List<Log>();
            if(userName.Equals(string.Empty))
            {
                listFiltered = CollectionOfListsToSingleListConverter(_userLogs.Values);
            }
            else
            {
                listFiltered = _userLogs[userName];
            }
            return listFiltered; 
        }

        private List<Log> FilterByGameName(string gameName)
        {
            List<Log> listFiltered = new List<Log>();
            if(gameName.Equals(string.Empty))
            {
                listFiltered = CollectionOfListsToSingleListConverter(_gameLogs.Values);
            }
            else
            {
                listFiltered = _gameLogs[gameName];
            }
            return listFiltered;
        }

        private List<Log> FilterByDate(List<Log> logs, string date)
        {
            List<Log> listFiltered = new List<Log>();
            if(!date.Equals(string.Empty))
            {
                listFiltered = logs.FindAll(l => l.Time.Equals(DateTime.Parse(date)));
            }
            else
            {
                listFiltered = logs;
            }
            return listFiltered;
        }

        private List<Log> CollectionOfListsToSingleListConverter(ICollection<List<Log>> toConvert)
        {
            List<Log> toReturn = new List<Log>();
            foreach(List<Log> list in toConvert)
            {
                toReturn.AddRange(list);
            }
            return toReturn;
        }

        private List<Log> DeepCopyLogList(List<Log> logs)
        {
            List<Log> copy = new List<Log>();
            foreach(Log log in logs)
            {
                Log logCopy = DeepCopyLog(log);
                copy.Add(logCopy);
            }
            return copy;
        }

        private Log DeepCopyLog(Log logToCopy)
        {
            Log logCopy = new Log();
            logCopy.EventType = logToCopy.EventType;
            logCopy.User = logToCopy.User;
            logCopy.Game = logToCopy.Game;
            logCopy.Time = logToCopy.Time;
            logCopy.Status = logToCopy.Status;
            return logCopy;
        }
    }
}
