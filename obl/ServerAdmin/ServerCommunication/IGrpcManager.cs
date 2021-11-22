using System;
using System.Threading.Tasks;
using ServerAdmin.DTOs;

namespace ServerAdmin.ServerCommunication
{
    public interface IGrpcManager
    {
        Task<Reply> AddUserAsync(string userName);
        Task<Reply> ModifyUserAsync(string oldName, string newName);
        Task<Reply> RemoveUserAsync(string userName);
        Task<Reply> AddGameAsync(GameDTO game);
        Task<Reply> AsociateGameAsync(string gameTitle, string userName);
        Task<Reply> ModifyGameAsync(string oldGameTitle, GameDTO game);
        Task<Reply> RemoveGameAsync(string gameTitle);
        Task<Reply> DisassociateGameAsync(string gameTitle, string userName);
    }
}
