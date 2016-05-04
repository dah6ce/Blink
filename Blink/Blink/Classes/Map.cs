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
        Color[] map = new Color[50*30];
        int[,] collisionMap = new int[50,30];
        Vector2 charSize = new Vector2(32, 64);
        int tileSize;
        Texture2D mapTexture;
        Vector2 mapSize;
        PlayerClass[] players = new PlayerClass[4];
        Vector2[] playerStarts = new Vector2[4];
        List<Powerup> powerupList = new List<Powerup>();
        Texture2D powerup;
        Rectangle bomb = new Rectangle(0, 0, 32, 32);
        Rectangle spearcatch = new Rectangle(32, 0, 32, 32);
        Rectangle backupspear = new Rectangle(64, 0, 32, 32);
        Rectangle shield = new Rectangle(96, 0, 32, 32);
        Rectangle unblinker = new Rectangle(128, 0, 32, 32);
        Boolean justSpawned = true;

        int MARGIN = 0;

        public void Initialize(Texture2D mText, String cMap, int tS, int mX, int mY, PlayerClass[] p, Texture2D power)
        {
            mapTexture = mText;
            mapSize = new Vector2(mX, mY);
            tileSize = tS;
            mapCollisions(cMap);
            players = p;
            powerup = power;
            
            for(int player = 0; player < 4; player++)
            {
                if(playerStarts[player] != null && players[player] != null && players[player].active)
                    players[player].setPos(playerStarts[player]);
            }
        }

        public void reset()
        {
            for (int player = 0; player < 4; player++)
            {
                if (playerStarts[player] != null && players[player] != null && players[player].active)
                    players[player].setPos(playerStarts[player]);
            }
            justSpawned = true;
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
                    //This should eventually change to only cover stuff that causes replacements (e.g. a 1 might denote player 1, and should actually be a 0 in
                    //the map.) Everything else will simply be parsed as an int.
                    //Air
                    /*if (blocks[p] == "0")
                    {
                        collisionMap[x, y] = 0;
                    }
                    //Kill zone
                    else if (blocks[p] == "5")
                    {
                        collisionMap[x, y] = 5;
                    }
                    //Solid block
                    else if (blocks[p] == "10")
                    {
                        collisionMap[x, y] = 10;
                    }
                    //Ice block
                    else if (blocks[p] == "11")
                    {
                        collisionMap[x, y] = 11;
                    }*/
                    if (blocks[p] == "1" || blocks[p] == "2" || blocks[p] == "3" || blocks[p] == "4")
                    {
                        playerStarts[int.Parse(blocks[p]) - 1] = new Vector2(x * 32, y * 32);
                    }
                    if (blocks[p] == "30")
                    {   
                        Powerup item = new Powerup(new Rectangle(x * 32, y * 32, 32, 32));
                        powerupList.Add(item);
                    }
                    else
                        collisionMap[x, y] = int.Parse(blocks[p]);
                    y += 1;
                    p += 1;
                }
                x += 1;
            }
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(mapTexture, new Vector2(0, MARGIN), Color.White);
            foreach(Powerup p in powerupList) {
                if (p.visible)
                {
                    if (p.type == PowerupEnum.bombSpear)
                    {
                        sB.Draw(powerup, p.hitbox, bomb, Color.White);
        }
                    if (p.type == PowerupEnum.spearCatch)
                    {
                        sB.Draw(powerup, p.hitbox, spearcatch, Color.White);
                    }
                    if (p.type == PowerupEnum.backupSpear)
                    {
                        sB.Draw(powerup, p.hitbox, backupspear, Color.White);
                    }
                    if (p.type == PowerupEnum.shield)
                    {
                        sB.Draw(powerup, p.hitbox, shield, Color.White);
                    }
                    if (p.type == PowerupEnum.unblinker)
                    {
                        sB.Draw(powerup, p.hitbox, unblinker, Color.White);
                    }
                }
            }
        }

        private int collidePowerup(Rectangle p)
        {
            //check if player rectangle p collides with powerup
            for(int i = 0; i < powerupList.Count; i++)
            {
                if(powerupList[i].visible && powerupList[i].hitbox.Intersects(p))
                {
                    return i;
                }
            }
            // Didn't collide with any power ups
            return -1;
        }
        public void updatePowerup(GameTime gt)
        {
            if (justSpawned)
            {
                for (int i = 0; i < powerupList.Count; i++)
                {
                    powerupList[i].spawnTime = (float)gt.TotalGameTime.TotalSeconds;
                }
                justSpawned = false;
            }
            for (int i = 0; i < powerupList.Count; i++)
            {
                if ((float)gt.TotalGameTime.TotalSeconds - powerupList[i].spawnTime > powerupList[i].timer)
                {
                    powerupList[i].visible = true;
                }
            }
        }
        public PowerupEnum checkPowerup(Rectangle r)
        {
            int c = collidePowerup(r);
            if (c != -1)
            {
                Powerup p = powerupList[c];
                powerupList.RemoveAt(c);
                return p.type;
            }
            return PowerupEnum.none;
        }

        //There are still a couple of issues with collisions, but they're hard to reproduce. They should eventually get ironed out.
        public Boolean[] collides(Vector2 pos, Vector2 oPos, int down, int right, Vector2 charSize, Boolean blinked, float hitReduce)
        {
            this.charSize = charSize;
            Vector2 newPos = new Vector2();
            Vector2 oldPos = new Vector2();
            Boolean[] collisions = new bool[3];
            //convert positions to respective tile locations, taking into account movement direction
            if (right == 1)
            {
                newPos.X = (float)Math.Floor((pos.X + charSize.X - hitReduce) / tileSize);
            }
            else
                newPos.X = (float)Math.Floor((pos.X + hitReduce) / tileSize);
            if(down == 1)
                newPos.Y = (float)Math.Floor((pos.Y + charSize.Y - hitReduce) / tileSize);
            else
                newPos.Y = (float)Math.Floor((pos.Y + hitReduce) / tileSize);

            newPos.X = loopCorrection(newPos.X, mapSize.X);
            newPos.Y = loopCorrection(newPos.Y, mapSize.Y);
            
            oldPos.X = (float)Math.Floor((oPos.X+hitReduce) / tileSize);
            oldPos.Y = (float)Math.Floor((oPos.Y+hitReduce) / tileSize);

            oldPos.X = loopCorrection(oldPos.X, mapSize.X);
            oldPos.Y = loopCorrection(oldPos.Y, mapSize.Y);
            
            float oldRight = (float)Math.Floor((oPos.X + (charSize.X-1-hitReduce)) / tileSize);
            oldRight = loopCorrection(oldRight, mapSize.X);
            float oldBot = (float)Math.Floor((oPos.Y + (charSize.Y-1-hitReduce)) / tileSize);
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

            //All values under 10 are blocks that can be passed through
            if (collisionMap[(int)newPos.X, (int)oldPos.Y] >= 10 || collisionMap[(int)newPos.X, (int)oldBot] >= 10 || collisionMap[(int)newPos.X, (int)oldVMid] >= 10)
                if(!blinked)
                   collisions[0] = true;
                else if ((collisionMap[(int)newPos.X, (int)oldPos.Y] < 20 && collisionMap[(int)newPos.X, (int)oldPos.Y] >= 10) || 
                    (collisionMap[(int)newPos.X, (int)oldBot] < 20 && collisionMap[(int)newPos.X, (int)oldBot] >= 10) || 
                    (collisionMap[(int)newPos.X, (int)oldVMid] < 20 && collisionMap[(int)newPos.X, (int)oldVMid] >= 10))
                    collisions[0] = true;
            if (collisionMap[(int)oldPos.X, (int)newPos.Y] >= 10 || collisionMap[(int)oldRight, (int)newPos.Y] >= 10 || collisionMap[(int)oldHMid, (int)newPos.Y] >= 10)
                if(!blinked)
                    collisions[1] = true;
                else if ((collisionMap[(int)oldPos.X, (int)newPos.Y] < 20 && collisionMap[(int)oldPos.X, (int)newPos.Y] >= 10) || 
                    (collisionMap[(int)oldRight, (int)newPos.Y] < 20 && collisionMap[(int)oldRight, (int)newPos.Y] >= 10) || 
                    (collisionMap[(int)oldHMid, (int)newPos.Y] < 20 && collisionMap[(int)oldHMid, (int)newPos.Y] >= 10))
                    collisions[1] = true;
            if (!collisions[0] && !collisions[1] && collisionMap[(int)newPos.X, (int)newPos.Y] >= 10)
                if(!blinked)
                    collisions[2] = true;
                else if (!collisions[0] && !collisions[1] && collisionMap[(int)newPos.X, (int)newPos.Y] < 20)
                    collisions[2] = true;



            return (collisions);

        }

        //Returns block data at a certain point on the map, using pixel coordinates
        public int blockInfo(Vector2 pos)
        {
            float xTile = (float)(Math.Floor(pos.X / tileSize) % mapSize.X);
            xTile = loopCorrection(xTile, mapSize.X);
            float yTile = (float)(Math.Floor(pos.Y / tileSize) % mapSize.Y);
            yTile = loopCorrection(yTile, mapSize.Y);
            return (collisionMap[(int)xTile, (int) yTile]);
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

        public int checkFooting(Vector2 pos)
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

            int left = collisionMap[(int)newPos.X, (int)newPos.Y];
            int right = collisionMap[(int)newRight, (int)newPos.Y];
            int middle = collisionMap[(int)newHMid, (int)newPos.Y];
            int footing = left < right ? right : left;
            footing = footing < middle ? middle : footing;

            return footing;
        }
    }
}
