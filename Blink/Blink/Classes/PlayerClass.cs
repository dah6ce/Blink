using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Blink.Classes;

namespace Blink.Classes
{
    class PlayerClass
    {
        //private int GRAVITY = 8, SPEED = 6, TERMINAL_V = 150, ACC_CAP = 80, JUMP = 150, TILEWIDTH = 16, MARGIN = 0;
        //private int curFriction = 12, airFriction = 1;
        private int JUMP = 300, TILEWIDTH = 32, MARGIN = 0;
        private int GRAVITY = 16, TERMINAL_V = 300, SPEED = 12, GROUNDSPEED = 12, ICESPEED = 4, ACC_CAP = 160;
        private int curFriction = 24, airFriction = 2, groundFriction = 24, iceFriction = 2;



        public Boolean active = true;
        
        Map arena;
        public Texture2D playerText;
        public Vector2 velocity, SCREENSIZE;
        Boolean atRest = false, dead = false;
        private PlayerClass[] players;
        Rectangle playerRect = new Rectangle(0,0,64,64);
        

        public void Initialize(Texture2D text, Vector2 playerPos, Vector2 ScreenSize, Map m, PlayerClass[] p)
        {
            playerRect.X = (int)playerPos.X;
            playerRect.Y = (int)playerPos.Y;
            players = p;
            playerText = text;
            velocity.X = 0;
            velocity.Y = 0;
            SCREENSIZE = ScreenSize;
            arena = m;
        }

        public void Update(KeyboardState input, GamePadState padState)
        {


            //Horizontal movement
            if ((input.IsKeyDown(Keys.Right) || padState.IsButtonDown(Buttons.LeftThumbstickRight)) && velocity.X < ACC_CAP && !dead)
            {
                velocity.X += SPEED;
                if (velocity.X < -SPEED)
                    velocity.X += SPEED / 2;
            }
            else if ((input.IsKeyDown(Keys.Left) || padState.IsButtonDown(Buttons.LeftThumbstickLeft)) && velocity.X > -ACC_CAP && !dead)
            {
                velocity.X -= SPEED;
                if (velocity.X > SPEED)
                    velocity.X -= SPEED / 2;
            }
            //Friction
            else if (velocity.X != 0)
            {
                int fric = curFriction;
                if (!atRest)
                    fric = airFriction;
                if (velocity.X < 0)
                {
                    if (velocity.X > -fric)
                        velocity.X = 0;
                    else
                        velocity.X += fric;
                }
                else
                {
                    if (velocity.X < fric)
                        velocity.X = 0;
                    else
                        velocity.X -= fric;
                }
            }

            //Jump
            if ((input.IsKeyDown(Keys.Up) || padState.IsButtonDown(Buttons.A)) && atRest)
            {
                velocity.Y -= JUMP;
                atRest = false;
            }

            //Velocity applications
            int footing = arena.checkFooting(new Vector2(playerRect.X,playerRect.Y));

            if (velocity.X != 0 && atRest && footing < 10)
                atRest = false;

            if (footing == 11)
            {
                curFriction = iceFriction;
                SPEED = ICESPEED;
            }
            else
            {
                curFriction = groundFriction;
                SPEED = GROUNDSPEED;
            }


            applyMove(velocity.X < 0, velocity.Y < 0);

            blockDataUpdate();
            
            
            //Gravity
            if (!atRest && velocity.Y < TERMINAL_V)
                velocity.Y += GRAVITY;

        }

        public void blockDataUpdate()
        {
            int[] blocks = new int[9];
            int x,y;

            for(x = 0; x < 3; x++)
            {
                for(y = 0; y < 3; y++)
                {
                    blocks[x * 3 + y] = arena.blockInfo(new Vector2(playerRect.X + x * 16 - 1, playerRect.Y + y * 16 - 1));
                }
            }

            foreach(int block in blocks)
            {
                if(block == 5)
                {
                    dead = true;
                }
            }
        }
        
        public void applyMove(Boolean left, Boolean right)
        {
            float testX = playerRect.X + velocity.X / 10f;
            float testY = playerRect.Y + velocity.Y / 10f;
            
            int d = 0, r = 0;
            if (velocity.X > 0)
                r = 1;
            else if (velocity.X < 0)
                r = -1;
            if (velocity.Y > 0)
                d = 1;
            else if (velocity.Y < 0)
                d = -1;

            Boolean[] collisions = arena.collides(new Vector2(testX, testY), new Vector2(playerRect.X,playerRect.Y), d, r);

            //This is kinda messy, I should eventually clean it up

            //Diagonal collisions
            if (collisions[2] && !collisions[0] && !collisions[1])
            {
                float horiDist, vertDist;
                if (velocity.X > 0)
                {
                    horiDist = testX % TILEWIDTH;
                }
                else
                    horiDist = TILEWIDTH - (testX % TILEWIDTH);

                if (velocity.Y > 0)
                {
                    vertDist = testY % TILEWIDTH;
                }
                else
                    vertDist = TILEWIDTH - (testY % TILEWIDTH);

                if(horiDist > vertDist)
                {
                    //If player is traveling left
                    if (velocity.X < 0)
                    {
                        testX = TILEWIDTH * (float)Math.Floor((double)playerRect.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                    else if (velocity.X > 0)
                    {
                        testX = TILEWIDTH * (float)Math.Floor((double)playerRect.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                }
                else
                {
                    //If player is traveling upwards
                    if (velocity.Y < 0)
                    {
                        testY = TILEWIDTH * (float)Math.Floor((double)playerRect.Y / TILEWIDTH);
                        velocity.Y = 0;
                    }
                    else if (velocity.Y > 0)
                    {
                        testY = TILEWIDTH * (float)Math.Ceiling((double)playerRect.Y / TILEWIDTH);
                        velocity.Y = 0;
                        atRest = true;
                    }
                }
            }
            else
            {
                if (collisions[0])
                {
                    //If player is traveling left
                    if (velocity.X < 0)
                    {
                        testX = TILEWIDTH * (float)Math.Floor((double)playerRect.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                    else if (velocity.X > 0)
                    {
                        testX = TILEWIDTH * (float)Math.Ceiling((double)playerRect.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                }
                if (collisions[1])
                {
                    //If player is traveling upwards
                    if (velocity.Y < 0)
                    {
                        testY = TILEWIDTH * (float)Math.Floor((double)playerRect.Y / TILEWIDTH);
                        velocity.Y = 0;
                    }
                    else if (velocity.Y > 0)
                    {
                        testY = TILEWIDTH * (float)Math.Ceiling((double)playerRect.Y / TILEWIDTH);
                        velocity.Y = 0;
                        atRest = true;
                    }
                }
            }
            //Looping stuff
            testY %= SCREENSIZE.Y;
            testX %= SCREENSIZE.X;

            if(testY < 0)
                testY += SCREENSIZE.Y;
            if (testX < 0)
                testX += SCREENSIZE.X;

            playerRect.Y = (int)testY;
            playerRect.X = (int)testX;
        }


        public void Draw(SpriteBatch sB)
        {
            sB.Draw(playerText, new Vector2(playerRect.X,playerRect.Y + MARGIN), Color.White);

            //Drawing when the player is looping over
            if (playerRect.X < playerRect.Width)
                sB.Draw(playerText, new Vector2(playerRect.X + SCREENSIZE.X,playerRect.Y + MARGIN), Color.White);
            else if(playerRect.X + playerRect.Width > SCREENSIZE.X)
                sB.Draw(playerText, new Vector2(playerRect.X - (SCREENSIZE.X), playerRect.Y + MARGIN), Color.White);

            if (playerRect.Y < playerRect.Height)
                sB.Draw(playerText, new Vector2(playerRect.X,playerRect.Y + SCREENSIZE.Y + MARGIN), Color.White);
            else if (playerRect.Y + playerRect.Height > SCREENSIZE.Y)
                sB.Draw(playerText, new Vector2(playerRect.X, playerRect.Y - (SCREENSIZE.Y) + MARGIN), Color.White);
        }
    }
}
