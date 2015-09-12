using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blink.Classes
{
    class Map
    {
        Color[] map = new Color[48*28];
        int[,] collisionMap = new int[48,28];
        Vector2 charSize = new Vector2(32, 32);
        int tileSize;
        Texture2D mapTexture;
        Vector2 mapSize;

        int MARGIN = 0;

        public void Initialize(Texture2D mText, Texture2D cMap, int tS)
        {
            mapTexture = mText;
            mapSize = new Vector2(cMap.Width, cMap.Height);
            tileSize = tS;
            mapCollisions(cMap);
        }

        //Read in collision map data
        public void mapCollisions(Texture2D cMap)
        {
            cMap.GetData<Color>(map);
            int p = 0,x = 0,y = 0;
            while(y < mapSize.Y)
            {
                x = 0;
                while(x < mapSize.X)
                {
                    
                    
                    if(map[p] == Color.White)
                    {
                        collisionMap[x,y] = 0;
                    }
                    else if(map[p] == Color.Black)
                    {
                        collisionMap[x,y] = 1;
                    }
                    x += 1;
                    p += 1;
                }
                y += 1;
            }
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(mapTexture, new Vector2(0, MARGIN), Color.White);
        }


        //There are still a couple of issues with collisions, but they're hard to reproduce. They should eventually get ironed out.
        public Boolean[] collides(Vector2 pos, Vector2 oPos, int down, int right)
        {
            Vector2 newPos = new Vector2();
            Vector2 oldPos = new Vector2();
            Boolean[] collisions = new bool[3];
            //convert positions to respective tile locations, taking into account movement direction
            if(right == 1)
                newPos.X = (float)Math.Floor((pos.X+charSize.X) / tileSize);
            else
                newPos.X = (float)Math.Floor(pos.X / tileSize);
            if(down == 1)
                newPos.Y = (float)Math.Floor((pos.Y+charSize.Y) / tileSize);
            else
                newPos.Y = (float)Math.Floor(pos.Y / tileSize);

            newPos.X = loopCorrection(newPos.X, mapSize.X);
            newPos.Y = loopCorrection(newPos.Y, mapSize.Y);
            
            oldPos.X = (float)Math.Floor(oPos.X / tileSize);
            oldPos.Y = (float)Math.Floor(oPos.Y / tileSize);

            oldPos.X = loopCorrection(oldPos.X, mapSize.X);
            oldPos.Y = loopCorrection(oldPos.Y, mapSize.Y);
            
            float oldRight = (float)Math.Floor((oPos.X + (charSize.X-1)) / tileSize);
            oldRight = loopCorrection(oldRight, mapSize.X);
            float oldBot = (float)Math.Floor((oPos.Y + (charSize.Y-1)) / tileSize);
            oldBot = loopCorrection(oldBot, mapSize.Y);

            //Middle...ish of hitbox in the horizontal direction
            float oldHMid = (float)Math.Floor((oPos.X + (charSize.X / 2)) / tileSize);
            oldHMid = loopCorrection(oldHMid, mapSize.X);
            //middle of the hitbox in the vertical direction
            float oldVMid = (float)Math.Floor((oPos.Y + (charSize.Y / 2)) / tileSize);
            oldVMid = loopCorrection(oldVMid, mapSize.Y);

            collisions[0] = false;
            collisions[1] = false;
            collisions[2] = false;
            if (collisionMap[(int)newPos.X, (int)oldPos.Y] != 0 || collisionMap[(int)newPos.X, (int)oldBot] != 0 || collisionMap[(int)newPos.X, (int)oldVMid] != 0)
                collisions[0] = true;
            if (collisionMap[(int)oldPos.X, (int)newPos.Y] != 0 || collisionMap[(int)oldRight, (int)newPos.Y] != 0 || collisionMap[(int)oldHMid, (int)newPos.Y] != 0)
                collisions[1] = true;
            if (!collisions[0] && !collisions[1] && collisionMap[(int)newPos.X, (int)newPos.Y] != 0)
                collisions[2] = true;
            return (collisions);

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

        public Boolean checkFooting(Vector2 pos)
        {
            Vector2 newPos = new Vector2();
            newPos.X = (float)Math.Floor(pos.X / tileSize); 
            newPos.Y = (float)Math.Floor((pos.Y + charSize.Y) / tileSize);

            newPos.X = loopCorrection(newPos.X, mapSize.X);
            newPos.Y = loopCorrection(newPos.Y, mapSize.Y);

            float newRight = (float)Math.Floor((pos.X + (charSize.X - 1)) / tileSize);
            newRight = loopCorrection(newRight, mapSize.X);

            float newHMid = (float)Math.Floor((pos.X + (charSize.X / 2)) / tileSize);
            newHMid = loopCorrection(newHMid, mapSize.X);

            if (collisionMap[(int)newPos.X, (int)newPos.Y] != 0 || collisionMap[(int)newRight, (int)newPos.Y] != 0 || collisionMap[(int)newHMid, (int)newPos.Y] != 0)
            {
                return true;
            }
            return false;
        }
    }
}
