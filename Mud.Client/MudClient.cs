using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mud.Client
{
    public class MudClient
    {
        private TcpClient Client;

        public EventHandler<List<string>> ReceivedData;

        public MudClient(TcpClient client)
        {
            Client = client;
            var clientCancelToken = new CancellationToken();
            Task.Run(() => StartReader(clientCancelToken), clientCancelToken);
        }

        async Task StartReader(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    while (Client.Connected)
                    {
                        var bytes = new byte[Client.Available];
                        var stream = Client.GetStream();
                        await stream.ReadAsync(bytes, 0, bytes.Length);
                        var data = Encoding.Default.GetString(bytes);
                        if (!string.IsNullOrEmpty(data))
                        {
                            ReceivedData?.Invoke(this, data.Trim().Split(' ').ToList());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public void SendData(string data)
        {
            try
            {
                data += "\n";
                var bytes = Encoding.UTF8.GetBytes(data);
                var stream = Client.GetStream();
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
