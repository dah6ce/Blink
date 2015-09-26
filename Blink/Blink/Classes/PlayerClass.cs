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
        private int GRAVITY = 16, SPEED = 12, TERMINAL_V = 300, ACC_CAP = 160, JUMP = 300, TILEWIDTH = 32, MARGIN = 0;
        private int curFriction = 24, airFriction = 2;



        public Boolean active = true;
        
        Map arena;
        public Texture2D playerText;
        public int Width, Height;
        public Vector2 pos,velocity, SCREENSIZE;
        Boolean atRest = false;

        public void Initialize(Texture2D text, Vector2 playerPos, Vector2 ScreenSize, Map m)
        {
            playerText = text;
            pos = playerPos;
            Width = playerText.Width;
            Height = playerText.Height;
            velocity.X = 0;
            velocity.Y = 0;
            SCREENSIZE = ScreenSize;
            arena = m;
        }

        public void Update(KeyboardState input, GamePadState padState)
        {
            

            //Horizontal movement
            if ((input.IsKeyDown(Keys.Right) || padState.IsButtonDown(Buttons.DPadRight)) && velocity.X < ACC_CAP)
            {
                velocity.X += SPEED;
                if (velocity.X < -SPEED)
                    velocity.X += SPEED / 2;
            }
            else if ((input.IsKeyDown(Keys.Left) || padState.IsButtonDown(Buttons.DPadLeft)) && velocity.X > -ACC_CAP)
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
            if ((input.IsKeyDown(Keys.Up) || padState.IsButtonDown(Buttons.DPadUp)) && atRest)
            {
                velocity.Y -= JUMP;
                atRest = false;
            }

            //Velocity applications
            if (velocity.X != 0 && atRest && !arena.checkFooting(pos))
                atRest = false;

            applyMove(velocity.X < 0, velocity.Y < 0);
            
            
            //Gravity
            if (!atRest && velocity.Y < TERMINAL_V)
                velocity.Y += GRAVITY;

        }
        
        public void applyMove(Boolean left, Boolean right)
        {
            float testX = pos.X + velocity.X / 10f;
            float testY = pos.Y + velocity.Y / 10f;
            
            int d = 0, r = 0;
            if (velocity.X > 0)
                r = 1;
            else if (velocity.X < 0)
                r = -1;
            if (velocity.Y > 0)
                d = 1;
            else if (velocity.Y < 0)
                d = -1;

            Boolean[] collisions = arena.collides(new Vector2(testX, testY), pos, d, r);

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
                        testX = TILEWIDTH * (float)Math.Floor(pos.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                    else if (velocity.X > 0)
                    {
                        testX = TILEWIDTH * (float)Math.Floor(pos.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                }
                else
                {
                    //If player is traveling upwards
                    if (velocity.Y < 0)
                    {
                        testY = TILEWIDTH * (float)Math.Floor(pos.Y / TILEWIDTH);
                        velocity.Y = 0;
                    }
                    else if (velocity.Y > 0)
                    {
                        testY = TILEWIDTH * (float)Math.Ceiling(pos.Y / TILEWIDTH);
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
                        testX = TILEWIDTH * (float)Math.Floor(pos.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                    else if (velocity.X > 0)
                    {
                        testX = TILEWIDTH * (float)Math.Ceiling(pos.X / TILEWIDTH);
                        velocity.X = 0;
                    }
                }
                if (collisions[1])
                {
                    //If player is traveling upwards
                    if (velocity.Y < 0)
                    {
                        testY = TILEWIDTH * (float)Math.Floor(pos.Y / TILEWIDTH);
                        velocity.Y = 0;
                    }
                    else if (velocity.Y > 0)
                    {
                        testY = TILEWIDTH * (float)Math.Ceiling(pos.Y / TILEWIDTH);
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

            pos.Y = testY;
            pos.X = testX;
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(playerText, new Vector2(pos.X,pos.Y + MARGIN), Color.White);

            //Drawing when the player is looping over
            if (pos.X < Width)
                sB.Draw(playerText, new Vector2(pos.X + SCREENSIZE.X,pos.Y + MARGIN), Color.White);
            else if(pos.X + Width > SCREENSIZE.X)
                sB.Draw(playerText, new Vector2(pos.X - (SCREENSIZE.X), pos.Y + MARGIN), Color.White);

            if (pos.Y < Height)
                sB.Draw(playerText, new Vector2(pos.X,pos.Y + SCREENSIZE.Y + MARGIN), Color.White);
            else if (pos.Y + Height > SCREENSIZE.Y)
                sB.Draw(playerText, new Vector2(pos.X, pos.Y - (SCREENSIZE.Y) + MARGIN), Color.White);
        }
    }
}
