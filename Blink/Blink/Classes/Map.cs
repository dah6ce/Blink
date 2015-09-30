using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blink.Classes
{
    public class Map
    {
        Color[] map = new Color[50*30];
        Vector2 charSize = new Vector2(64, 64);
        public int tileSize;
        Texture2D mapTexture;
        public Vector2 mapSize;

        int MARGIN = 0;

        public List<Rectangle> rectangles;

        public void Initialize(Texture2D mText, String cMap, int tS, int mX, int mY)
        {
            mapTexture = mText;
            mapSize = new Vector2(mX, mY);
            tileSize = tS;
            rectangles = new List<Rectangle>();
            mapCollisions(cMap);
        }



        //Read in collision map data
        public void mapCollisions(String cMap)
        {
            String[] blocks = cMap.Split(',');
            int p = 0,x = 0,y = 0;

            while (x < mapSize.X)
            {
                y = 0;
                while (y < mapSize.Y)
                {
                    if(blocks[p] == "10")
                    {
                        rectangles.Add(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                    y += 1;
                    p += 1;
                }
                x += 1;
            }
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(mapTexture, new Vector2(0, MARGIN), Color.White);
        }

        //When points start to go off the screen, this will correct them by looping the point back using the scale
        public float loopCorrection(float input, float scale)
        {
            input %= scale;
            while (input < 0)
            {
                input += scale;
            }
            return input;
        }
    }
}
