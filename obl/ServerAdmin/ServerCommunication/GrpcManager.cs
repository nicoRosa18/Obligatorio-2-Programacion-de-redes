using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace ServerAdmin.ServerCommunication
{
    public class GrpcManager: IGrpcManager
    {
        private Greeter.GreeterClient _client;
        public GrpcManager()
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            // The port number(5001) must match the port of the gRPC server.
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:5001");
            _client = new Greeter.GreeterClient(channel);
        }

        public async Task<int> TestMethodAsync()
        {
            NumberReply replyNumero = await _client.GiveMeANumberAsync(new NumberRequest
                { Name = "Pepe" }
            );
            return NumberReply.NumberFieldNumber;
        }
        
    }
}
