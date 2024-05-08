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
        /// <summary>Whether the box with values should be drawn on screen</summary>
        private bool ShouldDraw = true;
        private Drawer? Drawer;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Drawer = new Drawer(this.Config);
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
            Drawer.DrawHealthStamina(e.SpriteBatch);
        }

        /// <summary>Whenever the HUD is rendered, render the health/stamina values as well.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (!Config.Above) return;
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;
            // ignore if the HUD or box is hidden
            if (Game1.game1.takingMapScreenshot || !Game1.displayHUD || !ShouldDraw) return;
            Drawer.DrawHealthStamina(e.SpriteBatch);
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (this.Config.ToggleKey.JustPressed()) ShouldDraw = !ShouldDraw;
        }
    }
}