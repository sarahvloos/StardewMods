using System;
using System.Runtime.CompilerServices;
using AlwaysShowBarValues;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;

namespace YourProjectName
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;
        /// <summary>Chosen colors for the text</summary>
        private readonly Dictionary<string, Color> textColors = new()
        {
            {"Black", Color.Black },
            {"Yellow", new Color(255, 190, 0) },
            {"Green", new Color(0, 190, 0) },
            {"Red", new Color(190, 0, 0) },
            {"Blue", new Color(0, 130, 255) }
        };

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            string position = this.Config.Position;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/

        /// <summary>Right after the game is launched, create a config menu for the player.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Bar Value Position",
                getValue: () => this.Config.Position,
                setValue: value => this.Config.Position = value,
                allowedValues: new string[] { "Bottom Left", "Center Left", "Top Left", "Top Center", "Bottom Right", "Center Right", "Custom" }
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Override X Position",
                tooltip: () => "Choose 'Custom' as your bar value position, then change the horizontal position here.",
                getValue: () => this.Config.X,
                setValue: value => this.Config.X = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Override Y Position",
                tooltip: () => "Choose 'Custom' as your bar value position, then change the horizontal position here.",
                getValue: () => this.Config.Y,
                setValue: value => this.Config.Y = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Health Text Color Scheme",
                getValue: () => this.Config.HealthColorMode,
                setValue: value => this.Config.HealthColorMode = value,
                allowedValues: new string[] { "Black", "Green/Yellow/Red", "Blue/Yellow/Red", "Blue/Black/Red" }
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Stamina Text Color Scheme",
                getValue: () => this.Config.StaminaColorMode,
                setValue: value => this.Config.StaminaColorMode = value,
                allowedValues: new string[] { "Black", "Green/Yellow/Red", "Blue/Yellow/Red", "Blue/Black/Red" }
            );
        }

        /// <summary>Whenever the HUD is rendered, render the health/stamina values as well.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            this.Draw(e.SpriteBatch, Game1.player.Stamina, Game1.player.MaxStamina, Game1.player.health, Game1.player.maxHealth);
        }


        /// <summary>Get the box's position according to the player's chosen preset, if any, or their overriden X and Y values.</summary>
        /// <param name="messageWidth">The width of the longest string between stamina and health values.</param>
        public Vector2 GetPositionFromConfig(float messageWidth)
        {
            // If the preset is invalid, use the Bottom Left preset
            if (Config == null || Config.Position == null) return GetPositionFromPreset("Bottom Left", messageWidth);
            // If the player selected a custom preset, get a position from their X and Y values
            if (Config.Position == "Custom") return new Vector2(Config.X, Config.Y);
            // Get a vector according to the player's selected preset
            return GetPositionFromPreset(Config.Position, messageWidth);
        }

        public Vector2 GetPositionFromPreset(string preset, float messageWidth)
        {
            Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            Vector2 res;
            switch (preset)
            {
                case "Bottom Left":
                    res = new Vector2(tsarea.Left + 16, tsarea.Bottom - 120);
                    break;
                case "Center Left":
                    res = new Vector2(tsarea.Left + 16, (tsarea.Height / 2) - 66);
                    break;
                case "Top Left":
                    res = new Vector2(tsarea.Left + 16, tsarea.Top + 16);
                    break;
                case "Top Center":
                    res = new Vector2((tsarea.Width / 2) - 96, tsarea.Top + 116);
                    break;
                case "Center Right":
                    res = new Vector2(tsarea.Right - (2 * messageWidth), (tsarea.Height / 2) - 56);
                    break;
                case "Bottom Right":
                    res = new Vector2(tsarea.Right - (2 * messageWidth) - 72, tsarea.Bottom - 120);
                    break;
                default:
                    res = new Vector2(tsarea.Left + 16, tsarea.Bottom - 120);
                    break;
            }
            if (Game1.uiViewport.Width < 1400)
            {
                res.Y -= 48f;
            }
            return res;
        }

        public void Draw(SpriteBatch b, float currentStamina, int maxStamina, int currentHealth, int maxHealth)
        {
            string staminaValue = new((int)Math.Max(0f, currentStamina) + "/" + maxStamina);
            string healthValue = new((int)Math.Max(0f, currentHealth) + "/" + maxHealth);
            float messageWidth = 24f + Math.Max(Game1.smallFont.MeasureString(staminaValue ?? "").X, Game1.smallFont.MeasureString(healthValue ?? "").X);
            Vector2 itemBoxPosition = GetPositionFromConfig(messageWidth);
            // left rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.FlipHorizontally, 1f);
            // middle rectangle
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f, itemBoxPosition.Y), new Rectangle(319, 360, 1, 24), Color.White * 1f, 0f, Vector2.Zero, new Vector2(messageWidth, 4.5f), SpriteEffects.None, 1f);
            // right rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f + messageWidth, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.None, 1f);

            itemBoxPosition.Y += 28f;
            itemBoxPosition.X += 48f;

            // health icon
            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(-12f, 16f), new Rectangle(16, 411, 16, 16), Color.White * 1f, 0f, new Vector2(8f, 8f), 1f, SpriteEffects.None, 1f);
            // get health color names from the player's config
            List<Color> healthColors = GetTextColors(Config.HealthColorMode);
            // get health color values ready
            ColorValues healthColorValues = new(healthColors[0], healthColors[1], healthColors[2]);
            Color healthColor = healthColorValues.GetTextColor(currentHealth / maxHealth);
            // health value
            Utility.drawTextWithShadow(b, healthValue ?? "", Game1.smallFont, itemBoxPosition, healthColor, 1f, 1f, -1, -1, 1f);

            itemBoxPosition.Y += Game1.smallFont.MeasureString(healthValue ?? "").Y - 8f;

            // stamina icon
            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(-12f, 16f), new Rectangle(0, 411, 16, 16), Color.White * 1f, 0f, new Vector2(8f, 8f), 1f, SpriteEffects.None, 1f);
            // get color names from the player's config
            List<Color> staminaColors = GetTextColors(Config.StaminaColorMode);
            // get stamina color values ready
            ColorValues staminaColorValues = new(staminaColors[0], staminaColors[1], staminaColors[2]);
            Color staminaColor = staminaColorValues.GetTextColor(currentStamina / maxStamina);
            // stamina value
            Utility.drawTextWithShadow(b, staminaValue ?? "", Game1.smallFont, itemBoxPosition, staminaColor, 1f, 1f, -1, -1, 1f);
        }

        public List<Color> GetTextColors(string colorMode)
        {
            string[] configColors = colorMode.Split('/');
            if (configColors.Length < 3) configColors = new string[3] { "Black", "Black", "Black" };
            Color max = textColors.ContainsKey(configColors[0]) ? textColors[configColors[0]] : Color.Black;
            Color middle = textColors.ContainsKey(configColors[1]) ? textColors[configColors[1]] : Color.Black;
            Color min = textColors.ContainsKey(configColors[2]) ? textColors[configColors[2]] : Color.Black;
            return new List<Color> { max, middle, min };
        }
    }
}