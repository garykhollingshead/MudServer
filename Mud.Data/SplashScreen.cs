using System;
using System.Collections.Generic;
using System.Text;
using Mud.Data.Persistance;

namespace Mud.Data
{
    public class SplashScreen : ModelWithIdentity
    {
        public string[] Screen { get; set; }
    }
}
