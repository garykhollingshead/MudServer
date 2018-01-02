using System.Collections.Generic;
using Mud.Data.Enums;
using Mud.Data.Interfaces;
using Mud.Data.Persistance;

namespace Mud.Data.Character
{
    public class Character : ModelWithIdentity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public Gender Gender { get; set; }
        public string Race { get; set; }
        public Height Height { get; set; }
        public int Weight { get; set; }
        public int CombatExperience { get; set; }
        public int QuestExperience { get; set; }
        public List<Item> Inventory { get; set; }
        public List<Skill> Skills { get; set; }
        public List<IStat> Stats { get; set; }
        public List<Commands.Commands> CommandsAvailiable = new List<Commands.Commands>{Commands.Commands.Login};
        public CharacterState CurrentState { get; set; }

    }
}
