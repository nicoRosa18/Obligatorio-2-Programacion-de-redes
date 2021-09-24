using System;

namespace Server.Domain.ServerExceptions
{
    public class GameAlreadyPurchased : Exception
    {
        public GameAlreadyPurchased()
        {
        }

        public override string Message => "Game already purchased";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}