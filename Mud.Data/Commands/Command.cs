using System;
using System.Collections.Generic;
using System.Text;

namespace Mud.Data.Commands
{
    public interface ICommand
    {
        string CommandString { get; set; }
        Commands CommandType { get; set; }
    }
}
