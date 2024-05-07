using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysShowBarValues
{
    public sealed class ModConfig
    {
        public string BoxStyle { get; set; } = "Round";
        public string Position { get; set; } = "Bottom Right";
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string HealthColorMode { get; set; } = "Green/Yellow/Red";
        public string StaminaColorMode { get; set; } = "Green/Yellow/Red";
        public bool Above { get; set; } = true;
        public bool TextShadow { get; set; } = true;
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("H");
        public string MaxHealthHex { get; set; } = "000000";
        public string MiddleHealthHex { get; set; } = "000000";
        public string MinHealthHex { get; set; } = "000000";
        public string MaxStaminaHex { get; set; } = "000000";
        public string MiddleStaminaHex { get; set; } = "000000";
        public string MinStaminaHex { get; set; } = "000000";
    }
}
