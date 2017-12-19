using Marten.Schema;

namespace Mud.Data.Persistance
{
    public class ModelWithIdentity
    {
        [Identity]
        public string Id { get; set; }
    }
}
