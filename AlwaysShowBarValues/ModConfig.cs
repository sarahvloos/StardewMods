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
        private readonly Dictionary<string, PlayerStat> PlayerStats = new() {
            { "Health", new PlayerStat("Health", new Microsoft.Xna.Framework.Rectangle(0, 411, 16, 16)) },
            { "Stamina", new PlayerStat("Stamina", new Microsoft.Xna.Framework.Rectangle(16, 411, 16, 16)) },
            { "SurvivalisticHunger", new PlayerStat("Hunger", new Microsoft.Xna.Framework.Rectangle(10, 428, 10, 10)) },
            { "SurvivalisticThirst", new PlayerStat("Thirst", new Microsoft.Xna.Framework.Rectangle(372, 362, 9, 9)) }
        };
        public string BoxStyle { get; set; } = "Round";
        public string Position { get; set; } = "Bottom Right";
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string HealthColorMode
        {
            get { return this.PlayerStats["Health"].ColorMode; }
            set
            {
                this.PlayerStats["Health"].ColorMode = value;
                this.PlayerStats["Health"].OnColorModeUpdate();
            }
        }
        public string StaminaColorMode
        {
            get { return this.PlayerStats["Stamina"].ColorMode; }
            set
            {
                this.PlayerStats["Stamina"].ColorMode = value;
                this.PlayerStats["Stamina"].OnColorModeUpdate();
            }
        }
        public bool Above { get; set; } = true;
        public bool TextShadow { get; set; } = true;
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("H");

        public string MaxHealthHex
        {
            get { return this.PlayerStats["Health"].ColorCodes["max"]; }
            set
            {
                this.PlayerStats["Health"].ColorCodes["max"] = value;
                this.PlayerStats["Health"].OnColorCodeUpdate();
            }
        }
        public string MiddleHealthHex
        {
            get { return this.PlayerStats["Health"].ColorCodes["middle"]; }
            set
            {
                this.PlayerStats["Health"].ColorCodes["middle"] = value;
                this.PlayerStats["Health"].OnColorCodeUpdate();
            }
        }
        public string MinHealthHex
        {
            get { return this.PlayerStats["Health"].ColorCodes["min"]; }
            set
            {
                this.PlayerStats["Health"].ColorCodes["min"] = value;
                this.PlayerStats["Health"].OnColorCodeUpdate();
            }
        }
        public string MaxStaminaHex
        {
            get { return this.PlayerStats["Stamina"].ColorCodes["max"]; }
            set
            {
                this.PlayerStats["Stamina"].ColorCodes["max"] = value;
                this.PlayerStats["Stamina"].OnColorCodeUpdate();
            }
        }
        public string MiddleStaminaHex
        {
            get { return this.PlayerStats["Stamina"].ColorCodes["middle"]; }
            set
            {
                this.PlayerStats["Stamina"].ColorCodes["middle"] = value;
                this.PlayerStats["Stamina"].OnColorCodeUpdate();
            }
        }
        public string MinStaminaHex
        {
            get { return this.PlayerStats["Stamina"].ColorCodes["min"]; }
            set
            {
                this.PlayerStats["Stamina"].ColorCodes["min"] = value;
                this.PlayerStats["Stamina"].OnColorCodeUpdate();
            }
        }

        public PlayerStat GetPlayerStat(string statName)
        {
            return this.PlayerStats[statName];
        }
    }
}
