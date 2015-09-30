using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Blink.Classes;
using System.Collections.Generic;
using System.Linq;

namespace Blink.Classes
{
    public class PlayerClass
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
        Rectangle myRect;
        List<Rectangle> collideWith;
        List<Rectangle> playersList;


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
            myRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, playerText.Width, playerText.Height);
            playersList = new List<Rectangle>();
        }

        public void updatePlayersList(List<PlayerClass> allPlayers)
        {
            // Go through the list of AllPlayers and add the players rectangles that aren't this into collideWidth
            // We need them in the collideWith so we are able to check collisions with them
            playersList.Clear();
            foreach (PlayerClass p in allPlayers)
            {
                if(p != this)
                {
                    playersList.Add(p.myRect);
                }
            }
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
            //Removed checkFooting function all together, not sure what it was doing?
            if (velocity.X != 0 && atRest)
                atRest = false;
            
            newMove();

            //Gravity
            if (!atRest && velocity.Y < TERMINAL_V)
                velocity.Y += GRAVITY;

        }

        public void newMove()
        {
            myRect.X = (int)pos.X;
            myRect.Y = (int)pos.Y;

            //if player's position needs to updated
            //update it one direction (x-axis and/or y-axis) at a time
            //No idea why diving by 10f is needed, but I got it from the old collision code
            if (velocity.X != 0)
            {
                newMoveOneAxis(new Vector2(velocity.X / 10f, 0));
            }
            if (velocity.Y != 0)
            {
                newMoveOneAxis(new Vector2(0, velocity.Y / 10f));
            }

            //After moving
            //We moved the myRect, but haven't update the Vector2D pos yet
            pos.X = myRect.X;
            pos.Y = myRect.Y;
        }
        public void newMoveOneAxis(Vector2 movement)
        {
            int collide = -1;
            //move, then check if collision occured
            myRect.Offset(movement);
            //Loop position, because myRect may have moved off screen
            myRect.X = (int)(arena.loopCorrection(myRect.X, arena.mapSize.X * arena.tileSize));
            myRect.Y = (int)(arena.loopCorrection(myRect.Y, arena.mapSize.Y * arena.tileSize));
            //Merge the map rectangles and other players rectangles so we can check collisions against both
            collideWith = arena.rectangles.Concat(playersList).ToList();
            //if a collision occured, find which rectangle it collided with
            for (int i = 0; i < collideWith.Count; i++)
            {
                if (myRect.Intersects(collideWith[i]))
                {
                    collide = i;
                }
            }
            //if a collision occured
            if (collide != -1)
            {
                if (movement.X > 0)
                {
                    //moving right, hit left side of wall
                    myRect.X = collideWith[collide].Left - myRect.Width;
                    velocity.X = 0;
                }
                if (movement.X < 0)
                {
                    //moving left, hit right side of wall
                    myRect.X = collideWith[collide].Right;
                    velocity.X = 0;
                }
                if (movement.Y > 0)
                {
                    //moving down, hit top of wall
                    myRect.Y = collideWith[collide].Top - myRect.Height;
                    velocity.Y = 0;
                    atRest = true;
                }
                if (movement.Y < 0)
                {
                    //moving up, hit bottom of wall
                    myRect.Y = collideWith[collide].Bottom;
                    velocity.Y = 0;
                }
            }
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
