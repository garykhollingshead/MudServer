using System;
using System.Collections.Generic;
using System.Text;
using Mud.Data.Interfaces;
using Mud.Data.Persistance;

namespace Mud.Data.Character
{
    public class Race : ModelWithIdentity, IRace
    {
        public string Name { get; set; }
        public List<string> Description { get; set; }
        public List<IStat> StartingStatsModifiers { get; set; }
    }
}
