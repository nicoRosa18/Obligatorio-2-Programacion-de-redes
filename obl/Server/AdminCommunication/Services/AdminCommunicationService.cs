using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Server.Domain;
using Server.Domain.ServerExceptions;

namespace Server.AdminCommunication
{
    public class AdminCommunicationService : AdminCommunication.AdminCommunicationBase
    {
        private readonly ILogger<AdminCommunicationService> _logger;
        private UsersAndCatalogueManager _usersAndCatalogueManager;
        private Message _messageLanguage;

        public AdminCommunicationService(ILogger<AdminCommunicationService> logger)
        {
            _logger = logger;
            _usersAndCatalogueManager = UsersAndCatalogueManager.Instance;
            _messageLanguage = new SpanishMessage();
        }

        public override Task<Reply> AddUser(UserRequestAddAndRemove request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            try
            {
                _usersAndCatalogueManager.ContainsUser(request.UserName);
                error = true;
                errorMessage = _messageLanguage.UserRepeated;
            }
            catch(UserNotFound)
            {
                _usersAndCatalogueManager.AddUser(request.UserName);
            }
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }

        public override Task<Reply> ModifyUser(UserRequestModify request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            try
            {
                _usersAndCatalogueManager.ModifyUserByAdmin(request.OldUserName, request.NewUserName);
            }
            catch(UserAlreadyExists)
            {
                error = true;
                errorMessage = _messageLanguage.UserRepeated;
            }
            catch(UserNotFound)
            {
                error = true;
                errorMessage = _messageLanguage.UserNotFound;
            }
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }

        public override Task<Reply> RemoveUser(UserRequestAddAndRemove request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            try
            {
                _usersAndCatalogueManager.RemoveUserByAdmin(request.UserName);
            }
            catch(UserNotFound)
            {
                error = true;
                errorMessage = _messageLanguage.UserNotFound;
            }
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }

        public override Task<Reply> AssociateGame(VinculationRequest request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            try
            {
                _usersAndCatalogueManager.AssociateGameToUserByAdmin(request.GameTitle, request.UserName);
            }
            catch(GameNotFound)
            {
                error = true;
                errorMessage = _messageLanguage.GameNotFound;
            }
            catch(UserNotFound)
            {
                error = true;
                errorMessage = _messageLanguage.UserNotFound;
            }
            catch(GameAlreadyPurchased)
            {
                error = true;
                errorMessage = _messageLanguage.GameAlreadyInLibrary;
            }
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }

        public override Task<Reply> DisassociateGame(VinculationRequest request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            try
            {
                _usersAndCatalogueManager.DesassociateGameToUserByAdmin(request.GameTitle, request.UserName);
            }
            catch(GameNotFound)
            {
                error = true;
                errorMessage = _messageLanguage.GameNotFound;
            }
            catch(UserNotFound)
            {
                error = true;
                errorMessage = _messageLanguage.UserNotFound;
            }
            catch(GameNotPurchased)
            {
                error = true;
                errorMessage = _messageLanguage.GameNotInLibrary;
            }
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }

        public override Task<Reply> AddGame(GameRequestAdd request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            User publisherDecoy = new User();
            Game newGame = new Game{
                Title = request.Title,
                Cover = request.Cover,
                Genre = request.Genre,
                Synopsis = request.Synopsis,
                AgeRating = request.AgeRating
            };
            newGame.CommunityQualifications= new Collection<Qualification>();
            try
            {
                _usersAndCatalogueManager.ExistsGame(newGame);
                _usersAndCatalogueManager.AddGame(publisherDecoy, newGame);   
            }
            catch(GameAlreadyExists) 
            {
                error = true;
                errorMessage = _messageLanguage.GameAlreadyExists;
            }
            
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }

        public override Task<Reply> ModifyGame(GameRequestModify request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            Game oldGameBeacon = new Game();
            oldGameBeacon.Title = request.OldTilte;
            Game newGame = new Game{
                Title = request.NewTitle,
                Cover = request.NewCover,
                Genre = request.NewGenre,
                Synopsis = request.NewSynopsis,
                AgeRating = request.NewAgeRating
            };
            try
            {
                _usersAndCatalogueManager.ExistsGame(oldGameBeacon);
                error = true;
                errorMessage = _messageLanguage.GameNotFound;
            }
            catch(GameAlreadyExists) 
            {
                try
                {
                    _usersAndCatalogueManager.ExistsGame(newGame);
                    _usersAndCatalogueManager.ModifyGameByAdmin(request.OldTilte, newGame);
                }
                catch(GameAlreadyExists)
                {
                    error = true;
                    errorMessage = _messageLanguage.GameAlreadyExists;
                }
            }
            
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }

        public override Task<Reply> RemoveGame(GameRequestRemove request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            Game gameBeacon = new Game();
            gameBeacon.Title = request.Title;
            try
            {
                _usersAndCatalogueManager.RemoveGameByAdmin(request.Title);
            }
            catch(GameNotFound) 
            {
                error = true;
                errorMessage = _messageLanguage.GameNotFound;
            }
            
            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }
    }
}