using System;

namespace CommonLogs
{
    public class Log
    {
        public string User {get; set;}
        public string Game {get; set;}
        public string EventType {get; set;}
        public DateTime Time {get; set;}
        public string Status {get; set;}

        public Log(){}

        public Log(string user, string game, string eventType, string status)
        {
            this.User = user;
            this.Game = game;
            this.EventType = eventType;
            this.Time = DateTime.Now;
            this.Status = status;
        }
        
        
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                Log p = (Log) obj;
                return (this.User == p.User) && (this.Game == p.Game)
                    &&(this.Status==p.Status)&&(this.Time==p.Time)
                    &&(this.EventType==p.EventType);
            }
        }
    }
}
