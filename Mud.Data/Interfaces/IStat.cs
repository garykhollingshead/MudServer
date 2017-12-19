namespace Mud.Data.Interfaces
{
    public interface IStat
    {
        short CurrentValue { get; set; }
        string Description { get; set; }
        string Name { get; set; }
    }
}
