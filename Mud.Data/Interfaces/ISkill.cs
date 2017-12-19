namespace Mud.Data.Interfaces
{
    public interface ISkill
    {
        string Description { get; set; }
        string Name { get; set; }
        short CurrentLevel { get; set; }
        string Group { get; set; }
    }
}
