using System;

namespace CommonLogs
{
    public class Log
    {
        public string User {get; set;}
        public string Game {get; set;}
        public string EventType {get; set;}
        public DateTime Time {get; set;}
        public string EventDescription {get; set;}

        public Log(string user, string game, string eventType, string eventDescription)
        {
            this.User = user;
            this.Game = game;
            this.EventType = eventType;
            this.Time = DateTime.Now;
            this.EventDescription = eventDescription;
        }
    }
}
