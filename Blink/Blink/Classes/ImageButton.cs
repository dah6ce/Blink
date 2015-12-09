using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blink.GUI
{
 
    /// <summary>
    /// A button with text
    /// </summary>
    public class ImageButton
    {
        const float FONT_SIZE = 12;
        const float MARGIN = 10;
        Vector2 THUMB_SIZE = new Vector2(200, 120);

        bool selected = false;
        bool usesMultiSelect = false;
        bool[] multiSelect = { false, false, false, false };
        bool[] locks = { false, false, false, false };
        Rectangle position;

        String name;
        Texture2D thumb;
        Texture2D[] selects;
        public Texture2D selectionOverlay;

        public ImageButton(Texture2D thumbnail, Vector2 pos, String mapName)
        {
            this.thumb = thumbnail;
            this.position = new Rectangle((int)pos.X, (int)pos.Y, (int)THUMB_SIZE.X, (int)THUMB_SIZE.Y);
            this.name = mapName;
        }

        public ImageButton(Texture2D thumbnail, Vector2 pos, String mapName, Texture2D[] selectOverlays)
        {
            this.thumb = thumbnail;
            this.position = new Rectangle((int)pos.X, (int)pos.Y, (int)THUMB_SIZE.X, (int)THUMB_SIZE.Y);
            this.name = mapName;
            this.selects = selectOverlays;
        }

        public void useMultiSelect()
        {
            usesMultiSelect = true;
        }

        public void setPosition(Vector2 pos)
        {
            this.position.X = (int)pos.X;
            this.position.Y = (int)pos.Y;
        }

        public void setSize(Vector2 size)
        {
            THUMB_SIZE = size;
            this.position.Width = (int)THUMB_SIZE.X;
            this.position.Height = (int)THUMB_SIZE.Y;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(thumb, position, Color.White);
            if (!usesMultiSelect)
            {
                if (selected)
                    sb.Draw(selectionOverlay, position, Color.Gold);
                else
                    sb.Draw(selectionOverlay, position, Color.Silver);
            }
            else
            {
                for(int i = 0; i < 4; i++)
                {
                    if (multiSelect[i]) { 
                        if(!locks[i])
                            sb.Draw(selects[i], position, Color.Silver);
                        else
                        {
                            sb.Draw(selects[i], position, Color.Gold);
                        }
                    }
                }
            }

        }

        public bool isSelected()
        {
            return selected;
        }

        public void unselect()
        {
            selected = false;
            if (usesMultiSelect)
            {
                for (int i = 0; i < 4; i++)
                {
                    multiSelect[i] = false;
                    unhover(i);
                    locks[i] = false;
                }
            }
            
        }

        public void unlock()
        {
            // if (usesMultiSelect)
            // {
            selected = false;
                for (int i = 0; i < 4; i++)
                {
                    //multiSelect[i] = false;
                    //unhover(i);
                    locks[i] = false;
                }
            //}
        }
        
        public void select()
        {
            selected = true;
            if (usesMultiSelect)
            {
                for(int i = 0; i < 4; i++)
                {
                    multiSelect[i] = true;
                }
            }
        }

        public void unselect(int p)
        {
            selected = false;
            locks[p] = false;
        }

        public void select(int p)
        {
            selected = true;
            locks[p] = true;
        }

        public void hover(int p)
        {
            multiSelect[p] = true;
        }

        public void unhover(int p)
        {
            multiSelect[p] = false;
        }
        
    }
}

