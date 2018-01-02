using System;
using System.Collections.Generic;
using System.Text;

namespace Mud.Server.Helpers
{
    public static class HeightHelper
    {
        public static string HeightToEnglishUnits(int height)
        {
            int feet = height / 12;
            int inches = height % 12;
            return $"{feet}' {inches}\"";
        }
    }
}
