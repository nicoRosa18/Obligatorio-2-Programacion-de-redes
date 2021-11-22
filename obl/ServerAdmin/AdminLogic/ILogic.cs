using System;
using System.Threading.Tasks;
using ServerAdmin.DTOs;

namespace ServerAdmin.AdminLogic
{
    public interface ILogic
    {
        Task GetUser(string id);
        Task GetGame(string id);
        Task AddUserAsync(string userName);
        Task ModifyUserAsync(string oldName, string newName);
        Task RemoveUserAsync(string userName);
        Task AddGameAsync(GameDTO game);
        Task ModifyGameAsync(string oldGameTitle, GameDTO game);
        Task RemoveGameAsync(string gameTitle);
        Task AssociateGameAsync(string gameTitle, string userName);
        Task DisassociateGameAsync(string gameTitle, string userName);
    }
}
