using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using xTile;

namespace AlwaysShowBarValues
{
    public class PlayerStat
    {
        /// <summary>Preset colors for the text</summary>
        private readonly Dictionary<string, Color> textColors = new()
        {
            {"Black", Color.Black },
            {"Yellow", new Color(255, 190, 0) },
            {"Green", new Color(0, 190, 0) },
            {"Red", new Color(190, 0, 0) },
            {"Blue", new Color(0, 130, 255) }
        };
        /// <summary>The stat's name</summary>
        public string StatName { get; set; }
        /// <summary>The texture the stat's icon is in</summary>
        public Texture2D IconSourceTexture = Game1.mouseCursors;
        /// <summary>The icon's position in Cursors</summary>
        public Rectangle IconSourceRectangle;
        /// <summary>The icon might need to be resized</summary>
        public float IconScale;
        /// <summary>The stat's current value</summary>
        private float currentValue;
        public float CurrentValue { get { return this.currentValue;  } set { 
                this.currentValue = value;
                this.StatusString = value + "/" + this.maxValue;
            } }
        /// <summary>The stat's maximum value</summary>
        private float maxValue;
        public float MaxValue
        {
            get { return this.maxValue; }
            set
            {
                this.maxValue = value;
                this.StatusString = this.currentValue + "/" + value;
                this.StringSize = Game1.smallFont.MeasureString(this.StatusString ?? "");
            }
        }
        /// <summary>The string that'll show up in the box</summary>
        public string StatusString = "";
        /// <summary>That string's size</summary>
        public Vector2 StringSize;

        /// <summary>Preset color mode chosen by the user</summary>
        public string ColorMode { get; set; } = "Green/Yellow/Red";

        /// <summary>Hex color codes input by the user</summary>
        public Dictionary<string, string> ColorCodes = new()
        {
            { "max", "#000000" },
            { "middle", "#000000" },
            { "min", "#000000" }
        };

        /// <summary>Current colors according to user-chosen color mode and hex codes</summary>
        public Dictionary<string, Color> Colors = new()
        {
            { "max", Color.Black },
            { "middle", Color.Black },
            { "min", Color.Black }
        };

        public PlayerStat(string StatName, Rectangle texturePosition) { 
            this.StatName = StatName;
            this.IconSourceRectangle = texturePosition;
            this.IconScale = 16f / texturePosition.Width;
            this.UpdateColors();
        }

        public void OnHUDUpdate(float currentValue, float maxValue)
        {
            this.StatusString = new(currentValue + "/" + maxValue);
        }

        private void UpdateColors()
        {
            string[] configColors = this.ColorMode.Split('/');
            if (configColors.Length < 3)
            {
                if (configColors.Length == 1 && configColors[0] == "Custom")
                {
                    Colors["max"] = GetColorFromHex(ColorCodes["max"]);
                    Colors["middle"] = GetColorFromHex(ColorCodes["middle"]);
                    Colors["min"] = GetColorFromHex(ColorCodes["min"]);
                    return;
                }
                configColors = new string[3] { "Black", "Black", "Black" };
            }
            Colors["max"] = textColors.ContainsKey(configColors[0]) ? textColors[configColors[0]] : Color.Black;
            Colors["middle"] = textColors.ContainsKey(configColors[1]) ? textColors[configColors[1]] : Color.Black;
            Colors["min"] = textColors.ContainsKey(configColors[2]) ? textColors[configColors[2]] : Color.Black;
        }

        public void OnColorModeUpdate() { 
            this.UpdateColors();
        }

        public void OnColorCodeUpdate()
        {
            if (this.ColorMode != "Custom") return;
            this.UpdateColors();
        }

        private static Color GetColorFromHex(string hex)
        {
            if (hex.Length < 6 || hex.Length > 7)
            {
                return Color.Black;
            }
            try
            {
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(hex.StartsWith("#") ? hex : "#" + hex);
                if (color != System.Drawing.Color.Empty) return new Color(color.R, color.G, color.B);
            }
            catch (ArgumentException)
            {
               return Color.Black;
            }
            return Color.Black;
        }

        private static Color CalculateCurrentColor(float ratio, Color lowestColor, Color highestColor)
        {
            int red = (int)(lowestColor.R + ((highestColor.R - lowestColor.R) * ratio));
            int green = (int)(lowestColor.G + ((highestColor.G - lowestColor.G) * ratio));
            int blue = (int)(lowestColor.B + ((highestColor.B - lowestColor.B) * ratio));
            return new Color(red, green, blue);
        }

        public Color GetTextColor()
        {
            float barFullness = this.CurrentValue / this.MaxValue;
            if (barFullness > 0.5)
            {
                float ratio = (2 * barFullness) - 1;
                return CalculateCurrentColor(ratio, this.Colors["middle"], this.Colors["max"]);
            }
            else
            {
                float ratio = 2 * barFullness;
                return CalculateCurrentColor(ratio, this.Colors["min"], this.Colors["middle"]);
            }
        }

    }
}
