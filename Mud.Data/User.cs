using System.Net.Sockets;

namespace Mud.Data
{
    public class User
    {
        public TcpClient Connection { get; set; }
        public Character Character { get; set; }
    }
}
