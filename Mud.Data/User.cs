using Mud.Client;

namespace Mud.Data
{
    public class User
    {
        public MudClient Connection { get; set; }
        public Character.Character Character { get; set; }
    }
}
