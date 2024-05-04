using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AlwaysShowBarValues
{
    public class ColorValues
    {
        private Color Max;
        private Color Middle;
        private Color Min;

        public ColorValues(Color max, Color middle, Color min) {
            this.Max = max;
            this.Middle = middle;
            this.Min = min;
        }

        private Color GetHigherColor(float ratio)
        {
            float proportion = ( 2 * ratio ) - 1;
            int red = (int)(Middle.R + ((Max.R - Middle.R)*proportion));
            int green = (int)(Middle.G + ((Max.G - Middle.G)*proportion));
            int blue = (int)(Middle.B + ((Max.B - Middle.B)*proportion));
            return new Color(red, green, blue);
        }
        private Color GetLowerColor(float ratio)
        {
            float proportion = 2 * ratio;
            int red = (int)(Min.R + ((Middle.R - Min.R)*proportion));
            int green = (int)(Min.G + ((Middle.G - Min.G)*proportion));
            int blue = (int)(Min.B + ((Middle.B - Min.B) * proportion));
            return new Color(red, green, blue);
        }

        public Color GetTextColor(float ratio)
        {
            return ratio > 0.5 ? this.GetHigherColor(ratio) : this.GetLowerColor(ratio);
        }
    }
}
