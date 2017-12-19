using System;
using System.Collections.Generic;
using Marten.Schema.Identity;
using Marten.Storage;

namespace Mud.DataAccess.IdGenerators
{
    public class CustomdIdGeneration : IIdGeneration
    {
        public IEnumerable<Type> KeyTypes { get; } = new Type[] { typeof(string) };

        public IIdGenerator<T> Build<T>()
        {
            return (IIdGenerator<T>)new CustomIdGenerator();
        }

        public bool RequiresSequences { get; } = false;

        public class CustomIdGenerator : IIdGenerator<string>
        {
            public string Assign(ITenant tenant, string existing, out bool assigned)
            {
                assigned = true;
                return existing ?? SnowflakeIdGenerator.NewIdAsString;
            }
        }
    }
}
