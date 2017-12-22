using System.Collections.Generic;

namespace Mud.Data.Interfaces
{
    public interface IRace
    {
        string Name { get; set; }
        List<string> Description { get; set; }
        List<IStat> StartingStatsModifiers { get; set; }
    }
}
