namespace Mud.Data.Interfaces
{
    public interface IItem
    {
        int Weight { get; set; }
        int BaseCost { get; set; }
        string BaseDescription { get; set; }
        string Name { get; set; }
    }
}
