using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysShowBarValues
{
    public sealed class ModConfig
    {
        public string Position { get; set; } = "Bottom Right";
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string HealthColorMode { get; set; } = "Green/Yellow/Red";
        public string StaminaColorMode { get; set; } = "Green/Yellow/Red";
    }
}
