using System;
using System.Collections.Generic;
using System.Text;
using Mud.Data.Interfaces;
using Mud.Data.Persistance;

namespace Mud.Data.Character
{
    public class Skill : ModelWithIdentity, ISkill
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public short CurrentLevel { get; set; }
        public string Group { get; set; }
    }
}
