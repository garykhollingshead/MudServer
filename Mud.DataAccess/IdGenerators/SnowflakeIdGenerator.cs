using System;
using Flakey;

namespace Mud.DataAccess.IdGenerators
{
    public class SnowflakeIdGenerator
    {
        private static readonly IdGenerator IdGen = new IdGenerator(0, new DateTime(2013, 1, 1));

        public static string NewIdAsString => IdGen.CreateId().ToString();
    }
}
