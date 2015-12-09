using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;

namespace Blink.Classes
{
    public class mapSet
    {
        List<string> mapNames;
        string setName;
        Random picker;
        Texture2D background;
        Texture2D[] thumbnails = new Texture2D[5];
        int column;
        bool selected = false;

        public mapSet(string name)
        {
            setName = name;
            mapNames = new List<string>();
            picker = new Random();
        }
        
        public void setBackground(Texture2D b)
        {
            background = b;
        }

        public void setColumn(int c)
        {
            column = c;
        }

        public int getColumn()
        {
            return column;
        }

        public void select()
        {
            selected = true;
        }

        public void unselect()
        {
            selected = false;
        }

        public bool isSelected()
        {
            return selected;
        }

        public List<string> Maps()
        {
            return mapNames;
        }

        public Texture2D[] Thumbs()
        {
            return thumbnails;
        }

        //Random functionality TBD
        public void getRandomThumbs(ContentManager Content)
        {
            for(int i = 0; i < 5; i++)
            {
                //Texture2D mapThumbtext = Content.Load<Texture2D>("MapData/" + pickMap() + "Thumb");
                Texture2D mapThumbtext = Content.Load<Texture2D>("MapData/" + mapNames[i] + "Color");
                thumbnails[i] = mapThumbtext;
            }
            
        }

        public Texture2D getBackground()
        {
            return background;
        }

        public void addMap(string mapName)
        {
            mapNames.Add(mapName);
        }

        public string pickMap()
        {
            int mapNum = (int)Math.Floor((double)picker.Next(mapNames.Count));
            return mapNames.ElementAt(mapNum);
        }
    }
}
