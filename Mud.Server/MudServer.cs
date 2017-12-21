using Mud.Client;
using Mud.Data;
using Mud.Data.Character;
using Mud.Data.Commands;
using Mud.Data.Configuration;
using Mud.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mud.Server
{
    public class MudServer
    {
        private CharacterRepo CharacterRepo;

        private TcpListener Server;
        private CancellationTokenSource ServerCancellationTokenSource;
        private Dictionary<MudClient, User> Users;

        public MudServer(AppSettings appSettings)
        {
            CommandProccessor.Secret = appSettings.AuthenticationSettings.SecretKey;
            CharacterRepo = new CharacterRepo(appSettings);
            Users = new Dictionary<MudClient, User>();

            Server = new TcpListener(IPAddress.Parse(appSettings.UrlSettings.ServerUrl), appSettings.UrlSettings.ServerPort);
            Server.Start(10);
            ServerCancellationTokenSource = new CancellationTokenSource();
            var token = ServerCancellationTokenSource.Token;
            Task.Run(() => StartListener(token), token);
        }

        private async Task StartListener(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await Server.AcceptTcpClientAsync();
                    var mudClient = new MudClient(client);
                    mudClient.ReceivedData = ReceivedData;
                    var user = new User { Character = new Character(), Connection = mudClient };
                    Users[mudClient] = user;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Shutdown()
        {
            ServerCancellationTokenSource.Cancel();
        }

        private void ReceivedData(object sender, List<string> data)
        {
            var client = sender as MudClient;
            var user = Users[client];
            ProcessCommand(user, data);
        }

        private void ProcessCommand(User user, List<string> data)
        {
            if (user.Character.CommandsAvailiable.First() == Commands.Login)
            {
                var name = data[0];
                if (!CommandProccessor.CheckUserLogin(user, name))
                {
                    return;
                }

                name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
                var character = CharacterRepo.GetCharacterByName(name);

                CommandProccessor.HandleUserLogin(user, character, name);
                return;
            }
            if (user.Character.CommandsAvailiable.Contains(Commands.EnterPassword))
            {
                var password = data[0];
                CommandProccessor.HandleUserPassword(user, password);
                return;
            }
        }

    }
}
