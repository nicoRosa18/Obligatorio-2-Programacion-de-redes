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

        public override Task<Reply> AddUser(UserRequest request, ServerCallContext context)
        {
            bool error = false;
            string errorMessage = "";
            try
            {
                _usersAndCatalogueManager.ContainsUser(request.UserName);
                _usersAndCatalogueManager.AddUser(request.UserName);
            }
            catch(UserNotFound)
            {
                error = true;
                errorMessage = _messageLanguage.UserRepeated;
            }

            return Task.FromResult(new Reply
            {
                Error = error,
                ErrorDescription = errorMessage
            });
        }
    }
}