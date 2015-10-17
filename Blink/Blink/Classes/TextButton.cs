using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blink.GUI
{
 
    /// <summary>
    /// A button with text
    /// </summary>
    public class TextButton : Button
    {
        const float FONT_SIZE = 12;
        const float MARGIN = 10;

        String text;
        SpriteFont font;

        public TextButton(String text, SpriteFont font, Vector2 pos, Texture2D up, Texture2D down, Vector2 center = default(Vector2)) : base(pos, new Vector2(0,0), up, down, center)
        {
            this.text = text;
            this.font = font;
            this.size = new Vector2(FONT_SIZE * text.Length + 2*MARGIN, 1.6f * FONT_SIZE + 2*MARGIN); 
        }

        public void SetText(String text)
        {
            this.text = text;
            this.size = new Vector2(FONT_SIZE * text.Length + 2*MARGIN, this.size.Y);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(font, text, (this.pos-(this.center * this.size)) + new Vector2(MARGIN, MARGIN), Color.Black);
        }
    }
}

