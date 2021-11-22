using System;
using System.Threading.Tasks;
using ServerAdmin.DTOs;
using ServerAdmin.Exceptions;
using ServerAdmin.ServerCommunication;

namespace ServerAdmin.AdminLogic
{
    public class Logic : ILogic
    {
        private IGrpcManager _communication;

        public Logic()
        {
            _communication = new GrpcManager();
        }
        
        public async Task AddUserAsync(string userName)
        {
            Reply possibleError = await _communication.AddUserAsync(userName);
            if (possibleError.Error)
            {
                throw new UserException(possibleError.ErrorDescription);
            }
        }

        public async Task ModifyUserAsync(string oldName, string newName)
        {
            Reply possibleError = await _communication.ModifyUserAsync(oldName, newName);
            if (possibleError.Error)
            {
                throw new UserException(possibleError.ErrorDescription);
            }
        }

        public async Task RemoveUserAsync(string userName)
        {
            Reply possibleError = await _communication.RemoveUserAsync(userName);
            if (possibleError.Error)
            {
                throw new UserException(possibleError.ErrorDescription);
            }
        }

        public async Task AddGameAsync(GameDTO game)
        {
            Reply possibleError = await _communication.AddGameAsync(game);
            if (possibleError.Error)
            {
                throw new GameException(possibleError.ErrorDescription);
            }
        }

        public async Task ModifyGameAsync(string oldGameTitle, GameDTO game)
        {
            Reply possibleError = await _communication.ModifyGameAsync(oldGameTitle, game);
            if (possibleError.Error)
            {
                throw new GameException(possibleError.ErrorDescription);
            }
        }

        public async Task RemoveGameAsync(string gameTitle)
        {
            Reply possibleError = await _communication.RemoveGameAsync(gameTitle);
            if (possibleError.Error)
            {
                throw new GameException(possibleError.ErrorDescription);
            }
        }

        public async Task AssociateGameAsync(string gameTitle, string userName)
        {
            Reply possibleError = await _communication.AsociateGameAsync(gameTitle, userName);
            if (possibleError.Error)
            {
                throw new UserException(possibleError.ErrorDescription);
            }
        }

        public async Task DisassociateGameAsync(string gameTitle, string userName)
        {
            Reply possibleError = await _communication.DisassociateGameAsync(gameTitle, userName);
            if (possibleError.Error)
            {
                throw new UserException(possibleError.ErrorDescription);
            }
        }
    }
}