using System.Collections.Generic;

namespace Mud.Data.Interfaces
{
    public interface IRace
    {
        string Name { get; set; }
        string Description { get; set; }
        List<IStat> StartingStatsModifiers { get; set; }
    }
}
