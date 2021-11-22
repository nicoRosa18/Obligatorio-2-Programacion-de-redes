using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using ServerAdmin.DTOs;

namespace ServerAdmin.ServerCommunication
{
    public class GrpcManager : IGrpcManager
    {
        private AdminCommunication.AdminCommunicationClient _client;

        public GrpcManager()
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:31700");
            _client = new AdminCommunication.AdminCommunicationClient(channel);
        }

        public async Task<Reply> AddUserAsync(string userName)
        {
            Reply reply = await _client.AddUserAsync(new UserRequestAddAndRemove()
                {UserName = userName}
            );
            return reply;
        }

        public async Task<Reply> ModifyUserAsync(string oldName, string newName)
        {
            Reply reply = await _client.ModifyUserAsync(new UserRequestModify()
                {
                    OldUserName = oldName,
                    NewUserName = newName
                }
            );
            return reply;
        }

        public async Task<Reply> RemoveUserAsync(string userName)
        {
            Reply reply = await _client.RemoveUserAsync(new UserRequestAddAndRemove()
                {UserName = userName}
            );
            return reply;
        }

        public async Task<Reply> AddGameAsync(GameDTO game)
        {
            Reply reply = await _client.AddGameAsync(new GameRequestAdd()
                {
                    Genre = game.Genre,
                    Title = game.Title,
                    AgeRating = game.AgeRating,
                    Cover = game.Cover,
                    Synopsis = game.Synopsis
                }
            );
            return reply;
        }

        public async Task<Reply> AsociateGameAsync(string gameTitle, string userName)
        {
            Reply reply = await _client.AssociateGameAsync(new VinculationRequest()
                {
                    UserName = userName,
                    GameTitle = gameTitle
                }
            );
            return reply;
        }

        public async Task<Reply> ModifyGameAsync(string oldGameTitle, GameDTO game)
        {
            Reply reply = await _client.ModifyGameAsync(new GameRequestModify()
                {
                    NewTitle = game.Title,
                    NewAgeRating = game.AgeRating,
                    NewCover = game.Cover,
                    NewSynopsis = game.Synopsis,
                    NewGenre = game.Genre,
                    OldTilte = oldGameTitle
                }
            );
            return reply;
        }

        public async Task<Reply> RemoveGameAsync(string gameTitle)
        {
            Reply reply = await _client.RemoveGameAsync(new GameRequestRemove()
                {
                    Title = gameTitle
                }
            );
            return reply;
        }

        public async Task<Reply> DisassociateGameAsync(string gameTitle, string userName)
        {
            Reply reply = await _client.DisassociateGameAsync(new VinculationRequest()
                {
                    UserName = userName,
                    GameTitle = gameTitle
                }
            );
            return reply;
        }
    }
}