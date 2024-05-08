using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Menus;


namespace AlwaysShowBarValues
{
    public class Drawer
    {
        private readonly ModConfig Config;

        public Drawer(ModConfig config)
        {
            Config = config;
        }

        public void DrawHealthStamina(SpriteBatch b)
        {
            PlayerStat health = Config.GetPlayerStat("Health");
            PlayerStat stamina = Config.GetPlayerStat("Stamina");
            health.CurrentValue = (float)Game1.player.health;
            health.MaxValue = (float)Game1.player.maxHealth;
            stamina.CurrentValue = (float)Game1.player.Stamina;
            stamina.MaxValue = (float)Game1.player.MaxStamina;
            this.Draw(b, health, stamina);
        }

        /// <summary>Get the box's position according to the player's chosen preset, if any, or their overriden X and Y values.</summary>
        /// <param name="messageWidth">The width of the longest string between stamina and health values.</param>
        private Vector2 GetPositionFromConfig(float messageWidth)
        {
            // If the preset is invalid, use the Bottom Left preset
            if (Config == null || Config.Position == null) return GetPositionFromPreset("Bottom Left", messageWidth);
            // If the player selected a custom preset, get a position from their X and Y values
            if (Config.Position == "Custom") return new Vector2(Config.X, Config.Y);
            // Get a vector according to the player's selected preset
            return GetPositionFromPreset(Config.Position, messageWidth);
        }

        private static Vector2 GetPositionFromPreset(string preset, float messageWidth)
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
        private void Draw(SpriteBatch b, PlayerStat topStat, PlayerStat bottomStat)
        {
            // calculate text dimensions for later
            float messageWidth = 24f + Math.Max(topStat.StringSize.X, bottomStat.StringSize.X);
            float messageHeight = topStat.StringSize.Y + bottomStat.StringSize.Y - 8f;
            // get the chosen position by the player
            Vector2 itemBoxPosition = GetPositionFromConfig(messageWidth);

            // draw background
            if (Config == null || Config.BoxStyle == "Round") DrawRoundBackground(b, messageWidth, itemBoxPosition);
            else if (Config.BoxStyle == "Toolbar") DrawToolbarBackground(b, messageWidth, messageHeight, itemBoxPosition);

            // adjust position to draw the top stat
            itemBoxPosition.Y += 28f;
            itemBoxPosition.X += 48f;

            this.DrawStat(b, topStat, itemBoxPosition);
            itemBoxPosition.Y += topStat.StringSize.Y - 8f;
            this.DrawStat(b, bottomStat, itemBoxPosition);
        }

        private void DrawStat(SpriteBatch b, PlayerStat stat, Vector2 itemBoxPosition)
        {
            // icon
            b.Draw(Game1.mouseCursors, itemBoxPosition + new Vector2(-12f, 16f), stat.IconSourceRectangle, Color.White * 1f, 0f, new Vector2(8f, 8f), stat.IconScale, SpriteEffects.None, 1f);
            // draw bottom string
            if (Config == null || Config.TextShadow)
                Utility.drawTextWithShadow(b, stat.StatusString, Game1.smallFont, itemBoxPosition, stat.GetTextColor(), 1f, 1f, -1, -1, 1f);
            else
                b.DrawString(Game1.smallFont, stat.StatusString, itemBoxPosition, stat.GetTextColor());
        }

        private static void DrawRoundBackground(SpriteBatch b, float messageWidth, Vector2 itemBoxPosition)
        {
            // left rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.FlipHorizontally, 1f);
            // middle rectangle
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f, itemBoxPosition.Y), new Rectangle(319, 360, 1, 24), Color.White * 1f, 0f, Vector2.Zero, new Vector2(messageWidth, 4.5f), SpriteEffects.None, 1f);
            // right rounded corners
            b.Draw(Game1.mouseCursors, new Vector2(itemBoxPosition.X + 24f + messageWidth, itemBoxPosition.Y), new Rectangle(323, 360, 6, 24), Color.White * 1f, 0f, Vector2.Zero, 4.5f, SpriteEffects.None, 1f);
        }

        private static void DrawToolbarBackground(SpriteBatch b, float messageWidth, float messageHeight, Vector2 itemBoxPosition)
        {
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)itemBoxPosition.X + 10, (int)itemBoxPosition.Y + 6, (int)messageWidth + 32, (int)messageHeight + 36, Color.White, 1f, drawShadow: false);
        }
    }
}
