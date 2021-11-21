using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace ServerAdmin.ServerCommunication
{
    public class GrpcManager: IGrpcManager
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
            Reply reply = await _client.AddUserAsync(new UserRequest
                { UserName = userName }
            );
            return reply;
        }
        
    }
}
