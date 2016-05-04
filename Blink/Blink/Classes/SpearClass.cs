using System;
using System.IO;
using Blink.GUI;
using Blink;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Blink.Classes;
using Blink.Utilities;

namespace Blink.Classes
{
     class SpearClass
    {
	    
        public Texture2D spearText, indicatorText;
        public Rectangle spear;
        public Vector2 /*pos,*/ velocity, SCREENSIZE;
        public float gravityEffect;
        public int spearOrientation, Width , Height;
        private float orientation;
        private Vector2 spearVector;
        private float GRAVITY = 1.6f, TERMINAL_V = 30, THROW_V = 30f;
        PlayerClass thrownBy = null;
        PlayerClass[] players;
        KeyboardState oldState;
        //  Spear orientation values
        //      1       2        3
        //      0     Player     4
        //      7       6        5
        public PlayerClass spearOwner; 
        public Map m;
        public readonly Keys THROW_KEY = Keys.Z, ATTACK_KEY = Keys.X;
        public readonly Buttons THROW_BUTTON = Buttons.X, ATTACK_BUTTON = Buttons.B;
        public Boolean throwDown = false;

		public int inUseTimer = 10, coolDown = 10;
        //Attacking or being thrown sets this to true
        public Boolean isInUse = false, atRest = true, dropped = false; // isInUse = if the spear is being used via throwing or attacking. atRest = spear not being used or on ground.
        public Boolean attachedToPlayer = true, throwing = false, attackDown = false; //attachedToPlayer to player, being thrown, or attacking Downward.

        //Sound effects
        public SoundEffectInstance Throw_Sound;
        public SoundEffectInstance Hit_Player_Sound;
        public SoundEffectInstance Hit_Wall_Sound;
        public SoundEffectInstance Stab_Sound;

        //Constructor for new spear
        //Takes inputs (Player, ScreenSize, Map)
        public SpearClass(PlayerClass spearOwner, Texture2D spearText,Texture2D indicatorText, Vector2 ScreenSize, /*necesary?*/ Map m, PlayerClass[] players)
        {
            this.spearText = spearText;
            this.indicatorText = indicatorText; 
            this.players = players;
            spear.Width = spearText.Width-20;
            spear.Height = spearText.Height;
            Width = spearText.Width;
            Height = spearText.Height;
            velocity.X = 0;
            velocity.Y = 0;
            this.spearOwner = spearOwner;
            spearOrientation = 0;
            this.SCREENSIZE = ScreenSize;
            this.m = m;
            spearOwner.setSpear(this);
        }

        //Manage inputs, check for a spear throw.
        public void Update(KeyboardState input, GamePadState padState)
        {
            //Reset variables to defualt state
            KeyboardState newState = input;
            //spear.Width = Width;  //updates the rectangle of the spear
            //spear.Height = Height;
			if (isInUse)  //delay after spear being thrown
			{
				if(inUseTimer > 0)
				{
					inUseTimer -= 1;
				}
				else
				{
					isInUse = false;
					inUseTimer = 10;
                    coolDown = 20;
				}
			}
            if (!isInUse && coolDown > 0)
            {
                coolDown -= 1;
            }
            //Spear throw
            /*if ((input.IsKeyDown(THROW_KEY) || padState.IsButtonDown(THROW_BUTTON)) && attachedToPlayer && !spearOwner.dead && !isInUse && coolDown <= 0 && !throwDown)
            {
                throwDown = true;
                isInUse = true;
                if (spearOwner.getDirectionFacing() == 0)
                {
                    spearOrientation = 0;
                }
                else if (spearOwner.getDirectionFacing() == 1)
                {
                    spearOrientation = 4;
                }
                if (input.IsKeyDown(Keys.Up) || padState.IsButtonDown(Buttons.LeftThumbstickUp))
                {
                    spearOrientation = 2;
                }
                else if (input.IsKeyDown(Keys.Down) || padState.IsButtonDown(Buttons.LeftThumbstickDown))
                {
                    spearOrientation = 6;
                }
                thrownBy = spearOwner;
                throwSpear();
            }

            else if ((input.IsKeyUp(THROW_KEY) && padState.IsButtonUp(THROW_BUTTON)))
                throwDown = false;
                */


            //Updating aim from player
            if(spearOwner != null) { 
                Vector2 stickDir = spearOwner.spearVector;
                if (stickDir.Length() >= .85f)
            {
                    orientation = VectorMath.rotationFromVector(stickDir) + (float)(Math.PI * 0.5f); //90 degree adjustment is to account for spear's initial rotation
                    spearVector = stickDir;
                }
                }
            //Holding spear attacks
            if ((input.IsKeyDown(ATTACK_KEY) || padState.IsButtonDown(ATTACK_BUTTON) || padState.IsButtonDown(THROW_BUTTON)) && attachedToPlayer && !spearOwner.dead && !isInUse && coolDown <= 0)
                {
                attackDown = true;
                
                }
            else if ((input.IsKeyUp(ATTACK_KEY) && padState.IsButtonUp(ATTACK_BUTTON)) && attackDown)
                {
                attackDown = false;
                }
            
            if(attachedToPlayer && !isInUse) { 
                spear.Location = spearOwner.getPlayerRect().Center;

                spear.X -= 16;
                spear.Y -= 8;
            }

            if (!attachedToPlayer)
            {
            playerCollision();
            }
            if (!atRest)
            {
                mapCollision();
                //if (thrownBy != null)
                    throwUpdate();
                /*if (Math.Abs(velocity.Y) + gravityEffect < TERMINAL_V)
                    if(thrownBy != null)
                        gravityEffect += GRAVITY / 10;
                    else
                        gravityEffect += GRAVITY;
                spear.Y += (int)(velocity.Y + gravityEffect);*/
            }

            oldState = newState;
        }

        //Change spear hitbox - SHOULD NO LONGER BE NECESSARY
        private void correctHitBox()
        {
            /*if (spearOrientation == 0 || spearOrientation == 4)
            {
                spear.Width = Width;
                spear.Height = Height;
            }
            //When vertical, flip the hitbox
            else if(spearOrientation == 2 || spearOrientation == 6)
            {
                spear.Width = Height;
                spear.Height = Width;
            }*/
            }

        public void meleeCheck(int frame)
        {
            Vector2 move = new Vector2(spearVector.X, spearVector.Y);
            move.Normalize();
            move = move * (48/(frame));
            spear.Location = spearOwner.getPlayerRect().Center;

            spear.X -= 16;
            spear.Y -= 8;
            this.spear.X += (int)move.X;
            this.spear.Y -= (int)move.Y;
            
            playerCollision();
        }

        //Check to see if the spear is colliding with a player
        private void playerCollision()
        {
            if (isInUse && attachedToPlayer)
            {
                PlayerClass[] players = spearOwner.getPlayers();
                foreach (PlayerClass player in players)
                {
                    if (player != null && !player.Equals(spearOwner) && !player.dead && player.blinked == spearOwner.blinked)
                    {
                        Rectangle adjustedRect = new Rectangle(spear.Location, spear.Size);
                        adjustedRect.X -= 27;

                        bool hit = VectorMath.rectCollision(adjustedRect, orientation, player.getPlayerRect(), 0, new Point(16, 8));
                        
                        if (hit)//player.getPlayerRect().Intersects(this.spear))
                        {
                            player.setDead(true, this.spearOwner, "SPEAR");
                            Hit_Player_Sound.Play();
                        }

                        //Wrap-around collisions
                        if(this.spear.X <= spear.Width)
                        {
                            Rectangle tempRect = new Rectangle((int)(spear.X + SCREENSIZE.X), (int)spear.Y, spear.Width, spear.Height);
                            if (VectorMath.rectCollision(tempRect, orientation, player.getPlayerRect(), 0, new Point(16, 8)))//player.getPlayerRect().Intersects(tempRect))
                            {
                                player.setDead(true, this.spearOwner, "SPEAR");
                                Hit_Player_Sound.Play();
                            }
                        }
                        else if(this.spear.X >= SCREENSIZE.X - this.spear.Width)
                        {
                            Rectangle tempRect = new Rectangle((int)(spear.X - SCREENSIZE.X - 32), (int)spear.Y, spear.Width, spear.Height);
                            if (VectorMath.rectCollision(tempRect, orientation, player.getPlayerRect(), 0, new Point(16, 8)))//player.getPlayerRect().Intersects(tempRect))
                            {
                                player.setDead(true, this.spearOwner, "SPEAR");
                                Hit_Player_Sound.Play();
                            }
                        }
                    }
                }
            }
            if (!attachedToPlayer)
            {
                foreach (PlayerClass p in players)
                {
                    if(p != null) { 
                        if (!attachedToPlayer && !throwing)
                        {
                            //Rectangle inter = Rectangle.Intersect(p.getPlayerRect(), spear);//new Rectangle((int)spear.X,(int)spear.Y, spear.Width, spear.Height));
                            Rectangle adjustedRect = new Rectangle(spear.Location, spear.Size);
                            adjustedRect.X -= 27;
                            bool hit = VectorMath.rectCollision(adjustedRect, orientation+(float)(Math.PI/2), p.getPlayerRect(), 0, new Point(16, 8));

                            if (!hit && spear.X > SCREENSIZE.X - spear.Width / 2)
                            {
                                Rectangle dupePos = new Rectangle((int)(adjustedRect.X - SCREENSIZE.X), adjustedRect.Y, adjustedRect.Width, adjustedRect.Height);
                                hit = VectorMath.rectCollision(dupePos, orientation - (float)(Math.PI / 2), p.getPlayerRect(), 0, new Point(16, 8));

                            }

                            if (!hit && spear.Y > SCREENSIZE.Y - spear.Width / 2)
                            {
                                Rectangle dupePos = new Rectangle((int)(adjustedRect.X), (int)(adjustedRect.Y - SCREENSIZE.Y), adjustedRect.Width, adjustedRect.Height);
                                hit = VectorMath.rectCollision(dupePos, orientation, p.getPlayerRect(), 0, new Point(16, 8));

                            }

                            if (hit && !p.hasSpear && !throwing && !p.dead)
                            {
                                gravityEffect = 0;
                                attachedToPlayer = true;
                                setOwner(p);
                                spearOwner.setSpear(this);
                                isInUse = false;
                                throwing = false;
                                p.hasSpear = true;
                                dropped = false;
                            }
                        }
                        if (!atRest && thrownBy != null && p.blinked == thrownBy.blinked)
                        {
                            Rectangle adjustedRect = new Rectangle(spear.Location, spear.Size);
                            adjustedRect.X -= 27;
                            bool hit = VectorMath.rectCollision(adjustedRect, orientation, p.getPlayerRect(), 0, new Point(16, 8));

                            if (!hit && spear.X > SCREENSIZE.X - spear.Width / 2)
                            {
                                Rectangle dupePos = new Rectangle((int)(adjustedRect.X - SCREENSIZE.X), adjustedRect.Y, adjustedRect.Width, adjustedRect.Height);
                                hit = VectorMath.rectCollision(dupePos, orientation - (float)(Math.PI / 2), p.getPlayerRect(), 0, new Point(16, 8));

                            }

                            if (!hit && spear.Y > SCREENSIZE.Y - spear.Width / 2)
                            {
                                Rectangle dupePos = new Rectangle((int)(adjustedRect.X), (int)(adjustedRect.Y - SCREENSIZE.Y), adjustedRect.Width, adjustedRect.Height);
                                hit = VectorMath.rectCollision(dupePos, orientation, p.getPlayerRect(), 0, new Point(16, 8));

                            }

                            if (hit && spearOwner != p && !p.dead && !dropped)
                            {
                                p.setDead(true, thrownBy, "SPEAR");
                                Hit_Player_Sound.Play();
                            }

                            //When the spear first leaves the one who threw its hitbox, make it live.
                            if(!hit && spearOwner != null && spearOwner == p)
                            {
                                isInUse = false;
                                attachedToPlayer = false;
                                if(spearOwner.spear == this)
                                {
                                    spearOwner.hasSpear = false;
                                    spearOwner.setSpear(null);
                                }
                                    
                                setOwner(null);
                        }
                    }
                }
            }
            }

        }

        private void mapCollision()
        {
            Rectangle correctedBox = new Rectangle(spear.Location, spear.Size);

            correctedBox.X -= 27;

            Vector2[] verts = VectorMath.rectVerts(correctedBox, orientation);

            HashSet<int> xVals = new HashSet<int>();
            HashSet<int> yVals = new HashSet<int>();
            //gather values the spear might be passing through
            foreach(Vector2 vertex in verts)
            {
                xVals.Add((int)Math.Floor(vertex.X));
                yVals.Add((int)Math.Floor(vertex.Y));

            }

            //Using the dot product, find collidable rectangles and test.
            bool colliding = false;
            foreach(int x in xVals)
            {
                foreach(int y in yVals)
                {
                    int squareData = m.blockInfo(new Vector2(x, y));
                    if(thrownBy == null && squareData >= 10)
                    {
                        Rectangle r = new Rectangle(x, y, 32, 32);
                        if (VectorMath.rectCollision(correctedBox, orientation, r, 0))
                        {
                            velocity.X = 0;
                            velocity.Y = 0;
                            atRest = true;
                            throwing = false;
                            Hit_Wall_Sound.Play();
                            colliding = true;
                            break;
                        }
                    }//This is messy, I know.
                    else if((squareData >= 10 && !thrownBy.blinked) || (squareData >= 10 && squareData < 20 && thrownBy.blinked))
                    {
                        Rectangle r = new Rectangle(x, y, 32, 32);
                        if (VectorMath.rectCollision(correctedBox, orientation, r, 0))
                        {
                            velocity.X = 0;
                            velocity.Y = 0;
                            atRest = true;
                            throwing = false;
                            Hit_Wall_Sound.Play();
                            colliding = true;
                            break;
                        }
                    }
                }
                if (colliding)
                    break;
            }
            
        }

        //Handle throw physics
        public void throwSpear()
        {
            Throw_Sound.Play();
            this.attachedToPlayer = false;
            this.throwing = true;
            this.isInUse = false;
            this.spearOwner.hasSpear = false;
            //velocity.X = spearOwner.velocity.X;

            this.spear.X = spearOwner.getPlayerRect().X;
            this.atRest = false;

            Vector2 move = new Vector2(spearVector.X, spearVector.Y);
            move *= THROW_V;
            this.velocity.X = move.X;
            this.velocity.Y = -move.Y;

            spear.X += (int)(velocity.X * thrownBy.getMulti());
            spear.Y += (int)(velocity.Y * thrownBy.getMulti());

            /*switch (spearOrientation)
            {
                case 0:
                    velocity.X = -20;
                    spear.X += (int)velocity.X;
                    spear.Y = spearOwner.getPlayerRect().Y + spearOwner.getPlayerRect().Height / 3;
                    break;
                case 2:
                    velocity.Y = -20;
                    spear.Y += (int)velocity.Y;
                    spear.Y = spearOwner.getPlayerRect().Y;
                    break;
                case 4:
                    velocity.X = 20;
                    spear.X = spearOwner.getPlayerRect().Width + spearOwner.getPlayerRect().X - spear.Width;
                    spear.X += (int)velocity.X;
                    spear.Y = spearOwner.getPlayerRect().Y + spearOwner.getPlayerRect().Height / 3;
                    break;
                case 6:
                    velocity.Y = 20;
                    spear.Y += (int)velocity.Y;
                    spear.Y = spearOwner.getPlayerRect().Y + spearOwner.getPlayerRect().Height / 2;
                    break;
            }*/

            //correctHitBox();
        }

        private void throwUpdate()
        {
            

            if (spear.X < spear.Width)
            {
                spear.X += (int)SCREENSIZE.X;
            }


            if (spear.Y < spear.Width)
            {
                spear.Y += (int)SCREENSIZE.Y;
            }
            
            velocity.Y += GRAVITY / 10f;

            if(thrownBy != null) { 
            spear.X += (int)(velocity.X * thrownBy.getMulti());
            spear.Y += (int)(velocity.Y * thrownBy.getMulti());
            }
            else
            {
                spear.X += (int)(velocity.X);
                spear.Y += (int)(velocity.Y);
            }

            Vector2 dirVect = new Vector2(velocity.X, -velocity.Y);
            if (!(dirVect.X == 0) && !(dirVect.Y == 0))
            {
                dirVect.Normalize();
                orientation = VectorMath.rotationFromVector(dirVect)+ (float)(Math.PI / 2);
            }

            /*if (Math.Abs(velocity.X) >= Math.Abs(velocity.Y))
            {
                if (velocity.X > 0)
                    spearOrientation = 4;
                else
                    spearOrientation = 0;
            }
            else
            {
                if (velocity.Y > 0)
                    spearOrientation = 6;
                else
                    spearOrientation = 2;
            }*/

            /*if (spearOwner != null)
            {
                Rectangle inter = Rectangle.Intersect(spearOwner.getPlayerRect(), new Rectangle((int)spear.X, (int)spear.Y, spear.Width, spear.Height));
                if (inter.Width == 0 && inter.Height == 0)
                {
                    //throwing = false;
                    isInUse = false;
                    attachedToPlayer = false;
                    spearOwner.hasSpear = false;
                    spearOwner.setSpear(null);
                    setOwner(null);
                }
            }*/
        }

        public void Draw(SpriteBatch sB)
        {
            Vector2 origin = new Vector2(0, 8);
            float RotationAngle = 0;
            Texture2D drawnText = spearText;

            if (spearOwner == null)
            {
                if (!atRest && thrownBy != null && thrownBy.blinked)
                    return;
                origin.X = 32;
                Vector2 screenPos = new Vector2(spear.X, spear.Y);
                Vector2 dirVect = new Vector2(velocity.X, -velocity.Y);
                dirVect.Normalize();
                RotationAngle = VectorMath.rotationFromVector(dirVect) + (float)(Math.PI/2);
                sB.Draw(drawnText, screenPos, null ,  Color.White , orientation, origin, 1.0f, SpriteEffects.None, 0f);
                Vector2 dupePos;
                if(spear.X > SCREENSIZE.X - spear.Width / 2) {
                    dupePos = new Vector2(screenPos.X-SCREENSIZE.X, screenPos.Y);
                    sB.Draw(drawnText, dupePos, null, Color.White, orientation, origin, 1.0f, SpriteEffects.None, 0f);
                }
                if (spear.Y > SCREENSIZE.Y - spear.Width / 2)
                {
                    dupePos = new Vector2(screenPos.X , screenPos.Y - SCREENSIZE.Y);
                    sB.Draw(drawnText, dupePos, null, Color.White, orientation, origin, 1.0f, SpriteEffects.None, 0f);
                }
            }

            else if (!isInUse && spearOwner!=null && !attackDown)
            {
                if (spearOwner.blinked)
                    return;
                Vector2 screenPos = new Vector2(spearOwner.getPlayerRect().X, spearOwner.getPlayerRect().Y);
                RotationAngle = (float)(MathHelper.Pi * .5);
                //screenPos.Y += spearOwner.getPlayerRect().Height;
                if(spearOwner.getDirectionFacing() == 1)
                    screenPos.X += spearOwner.getPlayerRect().Width+spear.Width/13;
                else if (spearOwner.getDirectionFacing() == 0)
                    screenPos.X -= spear.Width/16;
                //Drawing when the player is looping over
                if (spearOwner.getDirectionFacing() == 1 && spearOwner.getPlayerRect().X + spearOwner.getPlayerRect().Width > SCREENSIZE.X)
                {
                    screenPos.X = (spearOwner.getPlayerRect().X + spearOwner.getPlayerRect().Width) - SCREENSIZE.X;
                    //sB.Draw(drawnText, screenPos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                }
                //else
                    //sB.Draw(drawnText, screenPos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
            }
            else if ((attackDown || isInUse) && spearOwner != null)
            {
                /*Vector2 screenPos = new Vector2(spearOwner.getPlayerRect().X, spearOwner.getPlayerRect().Y);
                switch (spearOrientation) { 
                    case 0:
                        RotationAngle = 0;
                        screenPos.X -= spearOwner.getPlayerRect().Width;
                        screenPos.Y += spearOwner.getPlayerRect().Height/2;
                        break;
                    case 2:
                        RotationAngle = (float)(MathHelper.Pi/2);
                        screenPos.X += spearOwner.getPlayerRect().Width / 2;
                        screenPos.Y -= 3*spearOwner.getPlayerRect().Height / 4;
                        break;
                    case 4:
                        RotationAngle = (float)(MathHelper.Pi);
                        screenPos.X += spearOwner.getPlayerRect().Width*2;
                        screenPos.Y += spearOwner.getPlayerRect().Height/2;
                        break;
                    case 6:
                        RotationAngle = (float)(3*MathHelper.Pi/2);
                        screenPos.X += spearOwner.getPlayerRect().Width/2;
                        screenPos.Y += 7*spearOwner.getPlayerRect().Height/4;
                        break;
                }
                sB.Draw(drawnText, screenPos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                */
                
                //if (attackDown)
                //{
                RotationAngle = orientation + (float)Math.PI;
                Vector2 normDir = spearVector;
                normDir.X *= 10;
                normDir.Y *= 10;
                normDir.Normalize();
                Vector2 screenPos = new Vector2(spear.X+16, spear.Y+8);//new Vector2(spear.Location, spearOwner.getPlayerRect().Center.Y+(normDir.Y*32));
                //}
                origin = new Vector2(16, 8);
                if (attackDown)
                {
                    drawnText = indicatorText;
                }
                sB.Draw(drawnText, screenPos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
            }
        }

        public void reset(PlayerClass p, Map newMap)
        {
            if (p == null)
                return;
            gravityEffect = 0;
            setOwner(p);
            //Width = spear.Width;
            //Height = spear.Height;
            velocity.X = 0;
            velocity.Y = 0;
            spearOrientation = 0;
            isInUse = false;
            throwing = false;
            attachedToPlayer = true;
            atRest = true;
            spear.X = spearOwner.getPlayerRect().X;
            spear.Y = spearOwner.getPlayerRect().Y;
            spearOwner.setSpear(this);
            this.m = newMap;
        }

        internal void dropSpear()
        {
            spear.X = spearOwner.getPlayerRect().X+spearOwner.getPlayerRect().Width/2;
            spear.Y = spearOwner.getPlayerRect().Y;
            spearOwner.setSpear(null);
            spearOwner = null;
            attachedToPlayer = false;
            isInUse = false;
            throwing = false;
            dropped = true;
            atRest = false;
        }

        internal void setOwner(PlayerClass player) //set new Spear owner to player
        {
            spearOwner = player;
            if (spearOwner != null)
            {
                spearOwner.hasSpear = true;
            }
        }

        public void setThrownBy(PlayerClass owner)
        {
            thrownBy = owner;
        }
    }
}
