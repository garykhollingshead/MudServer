using System;
using System.Collections.Generic;
using System.Text;
using Mud.Data.Interfaces;
using Mud.Data.Persistance;

namespace Mud.Data.Character
{
    public class Item : ModelWithIdentity, IItem
    {
        public int Weight { get; set; }
        public int BaseCost { get; set; }
        public string BaseDescription { get; set; }
        public string Name { get; set; }
    }
}
