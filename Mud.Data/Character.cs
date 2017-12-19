using System.Collections.Generic;
using Mud.Data.Interfaces;
using Mud.Data.Persistance;

namespace Mud.Data
{
    public class Character : ModelWithIdentity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public Gender Gender { get; set; }
        public IRace Race { get; set; }
        public int CombatExperience { get; set; }
        public int QuestExperience { get; set; }
        public List<IItem> Inventory { get; set; }
        public List<ISkill> Skills { get; set; }
        public List<IStat> Stats { get; set; }
    }
}
