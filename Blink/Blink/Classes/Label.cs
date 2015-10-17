using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Blink.GUI
{
    /// <summary>
    /// A plain text element for a menu.
    /// </summary>
    public class Label
    {
        public Vector2 pos
        {
            get;
            set;
        }

        /**
         * Center of the label as a proportion of the size, 
         * e.g. (0.5, 0.5) places pos at the center of the label, (0,0) puts pos at the top left
         */
        public Vector2 center
        {
            get;
            set;
        }

        public String text
        {
            get;
            set;
        }
        SpriteFont font;
        Color color;

        public Label(String text, SpriteFont font, Vector2 pos, Vector2 center = default(Vector2), Color? color = null)
        {
            this.pos = pos;
            this.center = center;
            this.text = text;
            this.font = font;
            this.color = color ?? Color.White;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            Vector2 size = new Vector2(12 * text.Length, 16);
            sb.DrawString(font, text, pos - (size*center), color);
        }
    }
}

