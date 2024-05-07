using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;

namespace AlwaysShowBarValues
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ModConfig Config;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>Chosen colors for the text</summary>
        private readonly Dictionary<string, Color> textColors = new()
        {
            {"Black", Color.Black },
            {"Yellow", new Color(255, 190, 0) },
            {"Green", new Color(0, 190, 0) },
            {"Red", new Color(190, 0, 0) },
            {"Blue", new Color(0, 130, 255) }
        };
        /// <summary>Whether the box with values should be drawn on screen</summary>
        private bool ShouldDraw = true;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            string position = this.Config.Position;
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
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
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Visibility"
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show above HUD",
                tooltip: () => "Check this to show the box above all HUD elements, leave unchecked to show it below everything.",
                getValue: () => this.Config.Above,
                setValue: value => this.Config.Above = value
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle key",
                tooltip: () => "Press the toggle key to show or hide the box with values. The box will not show if the rest of the HUD is hidden.",
                getValue: () => this.Config.ToggleKey,
                setValue: value => this.Config.ToggleKey = value
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Position"
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
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Background"
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Box Style",
                getValue: () => this.Config.BoxStyle,
                setValue: value => this.Config.BoxStyle = value,
                allowedValues: new string[] { "Round", "Toolbar", "None" }
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Text"
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Add Shadow to Text",
                tooltip: () => "Whether you want the text to have a shadow underneath.",
                getValue: () => this.Config.TextShadow,
                setValue: value => this.Config.TextShadow = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Health Text Color Scheme",
                getValue: () => this.Config.HealthColorMode,
                setValue: value => this.Config.HealthColorMode = value,
                allowedValues: new string[] { "Black", "Green/Yellow/Red", "Blue/Yellow/Red", "Blue/Black/Red", "Custom" }
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Stamina Text Color Scheme",
                getValue: () => this.Config.StaminaColorMode,
                setValue: value => this.Config.StaminaColorMode = value,
                allowedValues: new string[] { "Black", "Green/Yellow/Red", "Blue/Yellow/Red", "Blue/Black/Red", "Custom" }
            );
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Custom Text Colors"
            );
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "The following settings only work if you selected Custom as the color scheme for the text you're trying to edit. Colors must be a hex code (like 00ff00). Reshades will affect the color you choose."
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Full Health Color Override",
                tooltip: () => "What color should the health text be when you're at full health?",
                getValue: () => this.Config.MaxHealthHex,
                setValue: value => this.Config.MaxHealthHex = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Half Health Color Override",
                tooltip: () => "What color should the health text be when you're at half health?",
                getValue: () => this.Config.MiddleHealthHex,
                setValue: value => this.Config.MiddleHealthHex = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Zero Health Color Override",
                tooltip: () => "What color should the health text be when you're at zero health?",
                getValue: () => this.Config.MinHealthHex,
                setValue: value => this.Config.MinHealthHex = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Full Stamina Color Override",
                tooltip: () => "What color should the stamina text be when you're at full stamina?",
                getValue: () => this.Config.MaxStaminaHex,
                setValue: value => this.Config.MaxStaminaHex = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Half Stamina Color Override",
                tooltip: () => "What color should the stamina text be when you're at half stamina?",
                getValue: () => this.Config.MiddleStaminaHex,
                setValue: value => this.Config.MiddleStaminaHex = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Zero Stamina Color Override",
                tooltip: () => "What color should the stamina text be when you're at zero stamina?",
                getValue: () => this.Config.MinStaminaHex,
                setValue: value => this.Config.MinStaminaHex = value
            );
        }

        /// <summary>Whenever the HUD is rendering, render the health/stamina values as well.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            if (Config.Above) return;
            this.Draw(e.SpriteBatch, Game1.player.Stamina, Game1.player.MaxStamina, Game1.player.health, Game1.player.maxHealth);
        }

        /// <summary>Whenever the HUD is rendered, render the health/stamina values as well.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (!Config.Above) return;
            this.Draw(e.SpriteBatch, Game1.player.Stamina, Game1.player.MaxStamina, Game1.player.health, Game1.player.maxHealth);
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (this.Config.ToggleKey.JustPressed()) ShouldDraw = !ShouldDraw;
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

        public static Vector2 GetPositionFromPreset(string preset, float messageWidth)
        {
            Rectangle tsarea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            var res = preset switch
            {
                "Bottom Left" => new Vector2(tsarea.Left + 16, tsarea.Bottom - 120),
                "Center Left" => new Vector2(tsarea.Left + 16, (tsarea.Height / 2) - 66),
                "Top Left" => new Vector2(tsarea.Left + 16, tsarea.Top + 16),
                "Top Center" => new Vector2((tsarea.Width / 2) - 96, tsarea.Top + 116),
                "Center Right" => new Vector2(tsarea.Right - (2 * messageWidth), (tsarea.Height / 2) - 56),
                "Bottom Right" => new Vector2(tsarea.Right - (2 * messageWidth) - 72, tsarea.Bottom - 120),
                _ => new Vector2(tsarea.Left + 16, tsarea.Bottom - 120),
            };
            if (Game1.uiViewport.Width < 1400)
            {
                res.Y -= 48f;
            }
            return res;
        }

        public void Draw(SpriteBatch b, float currentStamina, int maxStamina, int currentHealth, int maxHealth)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;
            // ignore if the HUD or box is hidden
            if (Game1.game1.takingMapScreenshot || !Game1.displayHUD || !ShouldDraw) return;

            // create the text that'll show up in the box
            string staminaValue = new((int)Math.Max(0f, currentStamina) + "/" + maxStamina);
            string healthValue = new((int)Math.Max(0f, currentHealth) + "/" + maxHealth);
            // calculate text dimensions for later
            float messageWidth = 24f + Math.Max(Game1.smallFont.MeasureString(staminaValue ?? "").X, Game1.smallFont.MeasureString(healthValue ?? "").X);
            float messageHeight = Game1.smallFont.MeasureString(staminaValue ?? "").Y + Game1.smallFont.MeasureString(healthValue ?? "").Y - 8f;
            // get the chosen position by the player
            Vector2 itemBoxPosition = GetPositionFromConfig(messageWidth);

            // draw background
            if (Config == null || Config.BoxStyle == "Round") DrawRoundBackground(b, messageWidth, itemBoxPosition);
            else if (Config.BoxStyle == "Toolbar") DrawToolbarBackground(b, messageWidth, messageHeight, itemBoxPosition);

            // adjust position to draw health
            itemBoxPosition.Y += 28f;
            itemBoxPosition.X += 48f;

            // health icon
            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(-12f, 16f), new Rectangle(16, 411, 16, 16), Color.White * 1f, 0f, new Vector2(8f, 8f), 1f, SpriteEffects.None, 1f);
            // get health color names from the player's config
            List<Color> healthColors = Config == null ? GetTextColors("Black", "", "", "") : GetTextColors(Config.HealthColorMode, Config.MaxHealthHex, Config.MiddleHealthHex, Config.MinHealthHex);
            // get health color values ready
            ColorValues healthColorValues = new(healthColors[0], healthColors[1], healthColors[2]);
            Color healthColor = healthColorValues.GetTextColor(currentHealth / maxHealth);
            // health value
            if (Config == null || Config.TextShadow)
                Utility.drawTextWithShadow(b, healthValue ?? "", Game1.smallFont, itemBoxPosition, healthColor, 1f, 1f, -1, -1, 1f);
            else
                b.DrawString(Game1.smallFont, healthValue ?? "", itemBoxPosition, healthColor);

            // adjust position to draw stamina
            itemBoxPosition.Y += Game1.smallFont.MeasureString(healthValue ?? "").Y - 8f;

            // stamina icon
            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(-12f, 16f), new Rectangle(0, 411, 16, 16), Color.White * 1f, 0f, new Vector2(8f, 8f), 1f, SpriteEffects.None, 1f);
            // get color names from the player's config
            List<Color> staminaColors = Config == null ? GetTextColors("Black", "", "", "") : GetTextColors(Config.StaminaColorMode, Config.MaxStaminaHex, Config.MiddleStaminaHex, Config.MinStaminaHex);
            // get stamina color values ready
            ColorValues staminaColorValues = new(staminaColors[0], staminaColors[1], staminaColors[2]);
            Color staminaColor = staminaColorValues.GetTextColor(currentStamina / maxStamina);
            // stamina value
            if (Config == null || Config.TextShadow)
                Utility.drawTextWithShadow(b, staminaValue ?? "", Game1.smallFont, itemBoxPosition, staminaColor, 1f, 1f, -1, -1, 1f);
            else
                b.DrawString(Game1.smallFont, staminaValue ?? "", itemBoxPosition, staminaColor);
        }

        public static void DrawRoundBackground(SpriteBatch b, float messageWidth, Vector2 itemBoxPosition)
        {
            // left rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.FlipHorizontally, 1f);
            // middle rectangle
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f, itemBoxPosition.Y), new Rectangle(319, 360, 1, 24), Color.White * 1f, 0f, Vector2.Zero, new Vector2(messageWidth, 4.5f), SpriteEffects.None, 1f);
            // right rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f + messageWidth, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.None, 1f);
        }

        public static void DrawToolbarBackground(SpriteBatch b, float messageWidth, float messageHeight, Vector2 itemBoxPosition)
        {
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)itemBoxPosition.X + 10, (int)itemBoxPosition.Y + 6, (int)messageWidth + 32, (int)messageHeight + 36, Color.White, 1f, drawShadow: false);
        }

        public List<Color> GetTextColors(string colorMode, string maxHex, string middleHex, string minHex)
        {
            string[] configColors = colorMode.Split('/');
            if (configColors.Length < 3) { 
                if (configColors.Length == 1 && configColors[0] == "Custom")
                {
                    return new List<Color> { GetColorFromHex(maxHex), GetColorFromHex(middleHex), GetColorFromHex(minHex) };
                }
                configColors = new string[3] { "Black", "Black", "Black" }; 
            }
            Color max = textColors.ContainsKey(configColors[0]) ? textColors[configColors[0]] : Color.Black;
            Color middle = textColors.ContainsKey(configColors[1]) ? textColors[configColors[1]] : Color.Black;
            Color min = textColors.ContainsKey(configColors[2]) ? textColors[configColors[2]] : Color.Black;
            return new List<Color> { max, middle, min };
        }

        public Color GetColorFromHex(string hex)
        {
            if(hex.Length < 6 || hex.Length > 7)
            {
                this.Monitor.Log($"Invalid custom color for AlwaysShowBarValues: {hex}", LogLevel.Warn);
                return Color.Black;
            }
            try
            {
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(hex.StartsWith("#") ? hex : "#"+hex);
                if (color != System.Drawing.Color.Empty) return new Color(color.R, color.G, color.B);
            }
            catch (ArgumentException e)
            {
                this.Monitor.Log("Error choosing custom color for AlwaysShowBarValues: "+e.Message, LogLevel.Warn);
                return Color.Black;
            }
            return Color.Black;
        }
    }
}