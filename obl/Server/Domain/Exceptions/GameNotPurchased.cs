using System;

namespace Server.Domain.ServerExceptions
{
    public class GameNotPurchased : Exception
    {
        public GameNotPurchased()
        {
        }

        public override string Message => "Game not in library";

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
