using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Blink.GUI
{
    /// <summary>
    /// An empty button for a menu, only draws a button background, used as a base for more complex buttons.
    /// </summary>
    public class Button
    {
        Texture2D up;
        Texture2D down;

        public Vector2 pos
        {
            get;
            set;
        }
        public Vector2 size
        {
            get;
            set;
        }
        public Vector2 center
        {
            get;
            set;
        }

        protected bool selected;

        public Button(Vector2 pos, Vector2 size, Texture2D up, Texture2D down, Vector2 center = default(Vector2)) 
        {
            this.up = up;
            this.down = down;
            this.pos = pos;
            this.size = size;
            this.selected = false;
            this.center = center;
        }

        public void Select()
        {
            selected = true;
        }

        public void UnSelect()
        {
            selected = false;
        }

        public void Toggle()
        {
            selected = !selected;
        }



        public virtual void Draw(SpriteBatch sb) 
        {
            if (selected)
                sb.Draw(down, new Rectangle((pos-(center*size)).ToPoint(), size.ToPoint()), Color.White);
            else
                sb.Draw(up, new Rectangle((pos-(center*size)).ToPoint(), size.ToPoint()), Color.White);
        }
    }
}

