using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Server.Domain;

namespace Server.AdminCommunication
{
    public class AdminCommunicationService : Greeter.GreeterBase
    {
        private readonly ILogger<AdminCommunicationService> _logger;
        private UsersAndCatalogueManager _usersAndCatalogueManager;

        public AdminCommunicationService(ILogger<AdminCommunicationService> logger)
        {
            _logger = logger;
            _usersAndCatalogueManager = UsersAndCatalogueManager.Instance;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hola cliente: " + request.Name
            });
        }

        public override Task<NumberReply> GiveMeANumber(NumberRequest request, ServerCallContext context)
        {            
            return Task.FromResult(new NumberReply()
            {

            });
        }
    }
}