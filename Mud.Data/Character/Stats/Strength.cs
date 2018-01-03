using Mud.Data.Interfaces;

namespace Mud.Data.Character.Stats
{
    public class Strength : IStat
    {
        public short CurrentValue { get; set; }
        public string Description { get; set; } 
            = "The physical power of your character. Used in carrying capacity, lifting, climbing, and damage.";
        public string Name { get; set; } = "Strength";
    }
}
