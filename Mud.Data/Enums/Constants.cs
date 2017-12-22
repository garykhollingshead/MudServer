using System.Collections.Generic;

namespace Mud.Data.Enums
{
    public static class StringConstants
    {
        public static readonly List<string> AffirmativeReply = new List<string>
        {
            "yes", "yeah", "yup", "yea", "yay", "y"
        };

        public static readonly List<string> NegativeReply = new List<string>
        {
            "no", "nope", "nay", "nah", "n"
        };

        public static readonly List<string> NorthMovement = new List<string>
        {
            "north", "n"
        };

    }
}
