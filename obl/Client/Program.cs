using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            ClientManager manager = new ClientManager();
            await manager.StartAsync();
        }
    }
}
