using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Server.AdminCommunication
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
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
                Number = 25
            });
        }
    }
}