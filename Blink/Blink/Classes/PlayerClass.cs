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
        private int JUMP = 30, TILEWIDTH = 32, MARGIN = 0;
        private float GRAVITY = 1.6f, TERMINAL_V = 30, SPEED = 1.2f, GROUNDSPEED = 1.2f, ICESPEED = 0.4f, ACC_CAP = 16;
        private float curFriction = 2.4f, airFriction = .2f, groundFriction = 2.4f, iceFriction = .2f;

        //debug variables
        private Boolean bounce = false;

        public Boolean active = true;

        Map arena;
        public Texture2D playerText, deadText;
        public Vector2 velocity, SCREENSIZE, oldPos;
        Boolean atRest = false, dead = false;
        private PlayerClass[] players;
        Rectangle playerRect = new Rectangle(0, 0, 64, 64);
        public String title;


        public void Initialize(Texture2D text, Vector2 playerPos, Vector2 ScreenSize, Map m, PlayerClass[] p)
        {
            oldPos = playerPos;
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
            //debug stuff goes here
            if ((input.IsKeyDown(Keys.LeftShift)))
                this.bounce = !this.bounce;

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
                float fric = curFriction;
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
            if ((input.IsKeyDown(Keys.Up) || padState.IsButtonDown(Buttons.A) || bounce) && atRest && !dead)
            {
                velocity.Y -= JUMP;
                atRest = false;
            }

            //Velocity applications
            int footing = arena.checkFooting(new Vector2(playerRect.X, playerRect.Y));

            if (atRest && footing < 10)
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
            int x, y;

            for (x = 0; x < 3; x++)
            {
                for (y = 0; y < 3; y++)
                {
                    blocks[x * 3 + y] = arena.blockInfo(new Vector2(playerRect.X + x * 16 - 1, playerRect.Y + y * 16 - 1));
                }
            }

            foreach (int block in blocks)
            {
                if (block == 5)
                {
                    dead = true;
                }
            }
        }

        public void applyMove(Boolean left, Boolean right)
        {
            float testX = playerRect.X + velocity.X;
            float testY = playerRect.Y + velocity.Y;

            int d = 0, r = 0;
            if (velocity.X > 0)
                r = 1;
            else if (velocity.X < 0)
                r = -1;
            if (velocity.Y > 0)
                d = 1;
            else if (velocity.Y < 0)
                d = -1;

            Boolean[] collisions = arena.collides(new Vector2(testX, testY), new Vector2(playerRect.X, playerRect.Y), d, r);

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

                if (horiDist > vertDist)
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

            if (testY < 0)
                testY += SCREENSIZE.Y;
            if (testX < 0)
                testX += SCREENSIZE.X;

            oldPos.X = playerRect.X;
            oldPos.Y = playerRect.Y;

            playerRect.Y = (int)testY;
            playerRect.X = (int)testX;


            if (!dead && (velocity.X != 0 || velocity.Y != 0))
                playerCollision();
        }


        /*
        //Loops through the other players to check for collisions. There are Four major cases:
        //      (Horizontal) Players are moving in opposite directions  -> both players have their horizontal movement ceased.
        //      (Horizontal) Players are moving in the same direction   -> The faster player has their horizontal velocity matched to the other
        //      (Vertical) Players are moving in opposite directions  -> One player gets a boost, the other dies
        //      (Vertical) Players are moving in the same direction   -> Slight difference in the positioning
        */
        public void playerCollision()
        {
            PlayerClass[] checkedPlayers = new PlayerClass[4];
            int counter = 0;
            foreach (PlayerClass p in players)
            {
                //Make sure we're not checking a collision that wasn't already checked
                Boolean alreadyChecked = false;
                foreach (PlayerClass checkedPlayer in checkedPlayers)
                {
                    if (checkedPlayer == p)
                        alreadyChecked = true;
                }
                checkedPlayers[counter] = this;
                counter += 1;
                //Make sure we're not checking self collision, or dead players
                Boolean collided;
                if (p != this && !p.dead && !alreadyChecked)
                {
                    collided = doCollisions(playerRect, oldPos, p.playerRect, p.oldPos, p);

                    if (!collided)
                    {
                        Rectangle loopRectA = new Rectangle(playerRect.X, playerRect.Y, playerRect.Width, playerRect.Height);
                        loopRectA = loopRectangle(loopRectA);
                        Vector2 loopOldRectA = new Vector2(oldPos.X, oldPos.Y);
                        loopOldRectA = loopVector(loopOldRectA, playerRect.Width, playerRect.Height);
                        Rectangle loopRectB = new Rectangle(p.playerRect.X, p.playerRect.Y, p.playerRect.Width, p.playerRect.Height);
                        loopRectB = loopRectangle(loopRectB);
                        Vector2 loopOldRectB = new Vector2(p.oldPos.X, p.oldPos.Y);
                        loopOldRectB = loopVector(loopOldRectB, p.playerRect.Width, p.playerRect.Height);

                        collided = doCollisions(loopRectA, loopOldRectA, loopRectB, loopOldRectB, p);
                    }
                }
            }
        }

        public Rectangle loopRectangle(Rectangle rect)
        {
            if (rect.X > SCREENSIZE.X - rect.Width)
                rect.X -= (int)SCREENSIZE.X;
            if (rect.Y > SCREENSIZE.Y - rect.Height)
                rect.Y -= (int)SCREENSIZE.Y;

            return rect;
        }

        public Vector2 loopVector(Vector2 vect, int width, int height)
        {
            if (vect.X > SCREENSIZE.X - width)
                vect.X -= (int)SCREENSIZE.X;
            if (vect.Y > SCREENSIZE.Y - height)
                vect.Y -= (int)SCREENSIZE.Y;

            return vect;
        }

        public Boolean doCollisions(Rectangle rectA, Vector2 oldRectA, Rectangle rectB, Vector2 oldRectB, PlayerClass p)
        {
            Rectangle inter = Rectangle.Intersect(rectB, rectA);
            if (inter.Width > 0 && inter.Height > 0)
            {
                Boolean xCollision = true, yCollision = true;
                //Keeps track of the total relative distance traveled
                Vector2 distTraveled = new Vector2(Math.Abs(p.velocity.X - this.velocity.X), Math.Abs(p.velocity.Y - this.velocity.Y));

                //This deals with face collisions, where the objects already 'collided' on the X or Y plane
                if ((oldRectA.X >= oldRectB.X && oldRectA.X <= oldRectB.X + rectB.Width - 1) || (oldRectA.X + rectA.Width - 1 >= oldRectB.X && oldRectA.X + rectA.Width - 1 <= oldRectB.X + rectB.Width - 1))
                {
                    xCollision = false;
                }
                else if ((oldRectA.Y >= oldRectB.Y && oldRectA.Y <= oldRectB.Y + rectB.Height - 1) || (oldRectA.Y + rectA.Height - 1 >= oldRectB.Y && oldRectA.Y + rectA.Height - 1 <= oldRectB.Y + rectB.Height - 1))
                {
                    yCollision = false;
                }

                //If it's not a face collision, it's a dreaded diagonal...
                if (xCollision && yCollision)
                {
                    //Whichever direction the collision occurs at a higher velocity is assumed to be the 'first' collision.
                    //If they are even we go with a horizontal collision
                    //This might have to be refined if the collisions feel sloppy
                    if (distTraveled.X < distTraveled.Y)
                        yCollision = false;
                    else
                        xCollision = false;
                }

                if (xCollision)
                {
                    //Case 1, the directions are different. 
                    if (((this.velocity.X < 0) ? (p.velocity.X >= 0) : (p.velocity.X <= 0)))
                    {
                        int thisDist, otherDist;
                        if (this.velocity.X == 0)
                        {
                            otherDist = Math.Abs((int)Math.Floor(inter.Width * (p.velocity.X / distTraveled.X)));
                            thisDist = inter.Width - otherDist;
                        }
                        else
                        {
                            thisDist = Math.Abs((int)Math.Floor(inter.Width * (this.velocity.X / distTraveled.X)));
                            otherDist = inter.Width - thisDist;
                        }


                        if (this.velocity.X < 0)
                        {
                            playerRect.X += thisDist;
                            p.playerRect.X -= otherDist;
                            this.velocity.X = 0;
                            p.velocity.X = 0;
                        }
                        else
                        {
                            playerRect.X -= thisDist;
                            p.playerRect.X += otherDist;
                            this.velocity.X = 0;
                            p.velocity.X = 0;
                        }
                    }
                    //Case 2, the directions are the same.
                    else
                    {

                        if (Math.Abs(this.velocity.X) > Math.Abs(p.velocity.X))
                        {
                            if (this.velocity.X > 0)
                            {
                                playerRect.X = p.playerRect.X - playerRect.Width;
                            }
                            else if (this.velocity.X < 0)
                                playerRect.X = p.playerRect.X + p.playerRect.Width;
                        }
                    }
                }
                else if (yCollision)
                {
                    //Case 3, collision is vertical and opposite directions.
                    //if (((this.velocity.Y < 0) ? (p.velocity.Y >= 0) : (p.velocity.Y <= 0)))
                    //{
                    int thisDist = Math.Abs((int)Math.Floor(inter.Height * (this.velocity.Y / distTraveled.Y)));
                    int otherDist = inter.Height - thisDist;

                    if (this.velocity.Y < 0)
                    {
                        playerRect.Y += thisDist;
                        p.playerRect.Y -= otherDist;
                        this.velocity.Y = 20;
                        p.velocity.Y = -20;
                        this.dead = true;
                        //this.rectA.Height = rectA.Height / 2;
                        //this.rectA.Y += rectA.Height;
                    }
                    else
                    {
                        playerRect.Y -= thisDist;
                        p.playerRect.Y += otherDist;
                        this.velocity.Y = -20;
                        p.velocity.Y = 20;
                        p.dead = true;
                        //rectB.Height = rectB.Height / 2;
                        //rectB.Y += rectB.Height;
                    }
                    //}
                    //Case 4, the collision is vertical and in the same direction
                    //else
                    //{

                    //}
                }
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch sB)
        {
            Texture2D drawnText = playerText;
            if (dead)
                drawnText = deadText;
            sB.Draw(drawnText, new Vector2(playerRect.X, playerRect.Y + MARGIN), Color.White);

            //Drawing when the player is looping over
            if (playerRect.X < playerRect.Width)
                sB.Draw(drawnText, new Vector2(playerRect.X + SCREENSIZE.X, playerRect.Y + MARGIN), Color.White);
            else if (playerRect.X + playerRect.Width > SCREENSIZE.X)
                sB.Draw(drawnText, new Vector2(playerRect.X - (SCREENSIZE.X), playerRect.Y + MARGIN), Color.White);

            if (playerRect.Y < playerRect.Height)
                sB.Draw(drawnText, new Vector2(playerRect.X, playerRect.Y + SCREENSIZE.Y + MARGIN), Color.White);
            else if (playerRect.Y + playerRect.Height > SCREENSIZE.Y)
                sB.Draw(drawnText, new Vector2(playerRect.X, playerRect.Y - (SCREENSIZE.Y) + MARGIN), Color.White);
        }
    }
}