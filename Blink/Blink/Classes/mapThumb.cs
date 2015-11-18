using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blink.GUI
{
 
    /// <summary>
    /// A button with text
    /// </summary>
    public class mapThumb
    {
        const float FONT_SIZE = 12;
        const float MARGIN = 10;
        Vector2 THUMB_SIZE = new Vector2(200, 120);

        bool selected = false;
        Rectangle position;

        String name;
        Texture2D thumb;
        public Texture2D selectionOverlay;

        public mapThumb(Texture2D thumbnail, Vector2 pos, String mapName)
        {
            this.thumb = thumbnail;
            this.position = new Rectangle((int)pos.X, (int)pos.Y, (int)THUMB_SIZE.X, (int)THUMB_SIZE.Y);
            this.name = mapName;
        }

        public void setPosition(Vector2 pos)
        {
            this.position.X = (int)pos.X;
            this.position.Y = (int)pos.Y;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(thumb, position, Color.White);
            if(selected)
                sb.Draw(selectionOverlay, position, Color.Gold);
            else
                sb.Draw(selectionOverlay, position, Color.Silver);

        }

        public void unselect()
        {
            selected = false;
        }
        
        public void select()
        {
            selected = true;
        }
    }
}

