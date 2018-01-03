﻿using System;
using System.Collections.Generic;
using System.Text;
using Mud.Data.Interfaces;

namespace Mud.Data.Character.Stats
{
    public class Constitution : IStat
    {
        public short CurrentValue { get; set; }
        public string Description { get; set; }
            = "";
        public string Name { get; set; } = "Constitution";
    }
}