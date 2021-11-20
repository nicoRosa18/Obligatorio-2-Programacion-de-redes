using System;

namespace ServerAdmin.DTOs
{
    public class UserAndGameDTO
    {
        public string User {get; set;}
        public string Game {get; set;}

        UserAndGameDTO(string user, string game)
        {
            this.User = user;
            this.Game = game;
        }
    }
}
