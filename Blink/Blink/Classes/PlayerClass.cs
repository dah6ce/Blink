using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Blink.Classes;
using Blink.Utilities;

namespace Blink.Classes
{
       class PlayerClass
    {
        //private int GRAVITY = 8, SPEED = 6, TERMINAL_V = 150, ACC_CAP = 80, JUMP = 150, TILEWIDTH = 16, MARGIN = 0;
        //private int curFriction = 12, airFriction = 1;
        private int JUMP = 22, TILEWIDTH = 32, MARGIN = 0;
        private float GRAVITY = 1.6f, TERMINAL_V = 30, SPEED = 0.75f, GROUNDSPEED = 0.75f, ICESPEED = 0.4f, ACC_CAP = 15;
        private float STUNTIME = 3f, BLINKCOOL = .5f, MAXBLINKJUICE = 6f, DEATHTIMER = 5f, BLINKMULTI = 1.5f, TRAILTIMER = 0.075f;
        private float curFriction = 2.4f, airFriction = .2f, groundFriction = 2.4f, iceFriction = .2f;

        //debug variables
        private Boolean bounce = false;

        public Boolean active = true;

        Map arena;
        public Texture2D playerText, deadText, blinkRect;
        public Vector2 velocity, SCREENSIZE, oldPos;
        public Boolean atRest = false, dead = false, victory = false;
        private PlayerClass[] players;
        Rectangle playerRect = new Rectangle(0, 0, 32, 64);
        public String title;
        public int winAssign = 0;
        public SpearClass spear;
        private int directionFacing = 0; //0 for left, 1 for right
        public Vector2 spearVector = new Vector2(-1,0);
        public Boolean hasSpear = true;
        public int attackAnimationFrames = 2, throwAnimationFrames = 5, attackFrameWait = 0, attackType = 0; //0 for melee, 1 for ranged.
        public Boolean blinked = false, blinkKeyDown = false, attackKeyDown = false, throwKeyDown = false;
        private float blinkJuice, blinkCoolDown, stunTimer, deathTimer, curMultiplier, dustTimer;

        public SoundEffectInstance Death_Sound;
        public SoundEffectInstance Jump_Sound;
        public SoundEffectInstance Blink_Sound;
        public SoundEffectInstance Unblink_Sound;

        private Vector2 offset;

        public Texture2D dustEffect, dustPoof;
        public List<Animation> aniList;

        public delegate void PlayerKilledHandler(object sender, DeathEventArgs e);
        public event PlayerKilledHandler onPlayerKilled;

		public int score = 0;

        public void Initialize(Texture2D text, Vector2 playerPos, Vector2 ScreenSize, Map m, PlayerClass[] p, Vector2 off, Texture2D bar)
        {
            offset = off;
            oldPos = playerPos;
            playerRect.X = (int)playerPos.X;
            playerRect.Y = (int)playerPos.Y;
            players = p;
            playerText = text;
            velocity.X = 0;
            velocity.Y = 0;
            SCREENSIZE = ScreenSize;
            arena = m;

            curMultiplier = 1f;

            blinkRect = bar;
            blinkJuice = MAXBLINKJUICE;
            blinkCoolDown = 0;
            stunTimer = 0;
            deathTimer = -2;
        }

        public void setTexture(Texture2D newSprite)
        {
            playerText = newSprite;
        }

        public void setSpear(SpearClass spr)
        {
            spear = spr;
            if (spear != null)
            {
                spear.setOwner(this); 
            }
        }

        public void Update(KeyboardState input, GamePadState padState, GameTime gameTime)
        {
            if (!active)
                return;
            //debug stuff goes here
            if ((input.IsKeyDown(Keys.Q)))
                this.bounce = !this.bounce;

            if (blinked)
            {
                if (velocity.X != 0 && atRest)
                {
                    dustTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (dustTimer < 0)
                    {
                        Animation poof = new Animation(dustEffect, new Vector2(playerRect.X - 30, playerRect.Y + playerRect.Height - 16), 20f, 6, aniList, directionFacing);
                        dustTimer = TRAILTIMER;
                    }
                }
                if(deathTimer < -1) { 
                    blinkJuice -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(blinkJuice <= 0)
                    {
                        blinkJuice = 0;
                        Boolean blocked = inPlayer();
                        if (!blocked)
                            blocked = inWall();
                        if (!blocked)
                        {
                            blinked = false;
                            curMultiplier = 1f;
                            Unblink_Sound.Play();
                            blinkCoolDown = BLINKCOOL;
                            stunTimer = STUNTIME;
                            deathTimer = -2;
                        }
                        else
                        {
                            deathTimer = DEATHTIMER;
                        }
                    }
                }
                else
                {
                    deathTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if(deathTimer <= 0)
                    {
                        setDead(true, null, "EXPIRE");
                        blinked = false;
                        Unblink_Sound.Play();
                        curMultiplier = 1f;
                    }
                }
            }
            else
            {
                if(blinkJuice < MAXBLINKJUICE)
                {
                    blinkJuice += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (blinkJuice > MAXBLINKJUICE)
                        blinkJuice = MAXBLINKJUICE;
                }
            }

            if(blinkCoolDown > 0)
            {
                blinkCoolDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if(stunTimer > 0)
            {
                stunTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            //Blink
            if ((input.IsKeyDown(Keys.LeftAlt) || padState.IsButtonDown(Buttons.LeftShoulder)) && !blinkKeyDown)
            {
                blinkKeyDown = true;

                Boolean blocked = inPlayer();
                if (!blocked)
                    blocked = inWall();

                if (!blocked && blinkCoolDown <= 0) {
                    if (!blinked && blinkJuice > 1 && !dead)
                    {
                        blinked = true;
                        Blink_Sound.Play();
                        curMultiplier = BLINKMULTI;
                        blinkJuice -= 1;
                        //blinkCoolDown = BLINKCOOL / 2f;
                    }
                    else if(blinked)
                    {
                        blinked = false;
                        Unblink_Sound.Play();
                        curMultiplier = 1f;
                        blinkCoolDown = BLINKCOOL;
                    }
                }
                
            }
            else if (blinkKeyDown && (input.IsKeyUp(Keys.LeftAlt) && padState.IsButtonUp(Buttons.LeftShoulder)))
            {
                blinkKeyDown = false;
            }


            //Implicit Aim
            if (padState.ThumbSticks.Left.Length() > 0.85f)
                spearVector = padState.ThumbSticks.Left;
            
            //Are we aiming?
            if ((!(padState.IsButtonDown(Buttons.B)) && !(padState.IsButtonDown(Buttons.X))) || !hasSpear) {
            //Horizontal movement
                if ((input.IsKeyDown(Keys.Right) || padState.IsButtonDown(Buttons.LeftThumbstickRight)) && velocity.X < ACC_CAP * curMultiplier && !dead && !victory && stunTimer <= 0)
            {
                    velocity.X += SPEED * curMultiplier;
                if (velocity.X < -SPEED)
                    velocity.X += SPEED * curMultiplier / 2;
            }
                else if ((input.IsKeyDown(Keys.Left) || padState.IsButtonDown(Buttons.LeftThumbstickLeft)) && velocity.X > -ACC_CAP * curMultiplier && !dead && !victory && stunTimer <= 0)
            {
                velocity.X -= SPEED * curMultiplier;
                if (velocity.X > SPEED)
                    velocity.X -= SPEED * curMultiplier / 2;
            }

                //Initiating a melee attack!
                if (attackKeyDown && spear != null)
                {
                    spear.isInUse = true;
                    //animate attack
                    attackFrameWait = attackAnimationFrames;
                    attackType = 0;
                    attackKeyDown = false;
                }
                //Initiating a thrown attack!
                else if (throwKeyDown && spear != null)
                {
                    throwKeyDown = false;
                    spear.setThrownBy(this);
                    spear.throwSpear();
                }
            }
            else
            {
                if(padState.IsButtonDown(Buttons.B))
                    attackKeyDown = true;
                if (padState.IsButtonDown(Buttons.X))
                    throwKeyDown = true;
                if(padState.ThumbSticks.Left.Length() > 0.85f)
                {
                    directionFacing = VectorMath.rotationFromVector(spearVector) > Math.PI || VectorMath.rotationFromVector(spearVector) < 0 ? 0 : 1;
                }
            }


            //Is there an attack animation in progress?
            if(attackFrameWait > 0)
            {
                attackFrameWait--;
                //Melee?
                if(attackType == 0)
                {
                    spear.meleeCheck();
                }
            }



            //Friction
            if (velocity.X != 0 && !padState.IsButtonDown(Buttons.LeftThumbstickRight) && !padState.IsButtonDown(Buttons.LeftThumbstickLeft))
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

            if (victory)
            {
                if (atRest && !dead)
                {
                    velocity.Y -= (JUMP / 2);
                    atRest = false;
                }
            }
            else
            {
                //Jump
                if ((input.IsKeyDown(Keys.LeftControl) || padState.IsButtonDown(Buttons.A) || bounce) && atRest && !dead && stunTimer <= 0)
                {
                    velocity.Y -= JUMP * curMultiplier;
                    atRest = false;
                    Jump_Sound.Play();
                }
            }

            //Velocity applications
            int footing = arena.checkFooting(new Vector2(playerRect.X, playerRect.Y));

            if ((atRest && footing < 10) || (atRest && footing >= 20 && blinked))
                atRest = false;

            if (footing == 11 || footing == 21)
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

            //Check whether player is moving left/right
            if (velocity.X > 0)
                directionFacing = 1;
            else if (velocity.X < 0)
                directionFacing = 0;

        }

        private Boolean inPlayer()
        {
            Boolean inAPlayer = false;
            foreach (PlayerClass p in players)
            {
                if (p != null && p != this && p.playerRect.Intersects(this.playerRect) && p.blinked != blinked)
                    inAPlayer = true;
            }

            return inAPlayer;
        }

        private Boolean inWall()
        {
            Boolean inAWall = false;
            Boolean[] collisions = arena.collides(new Vector2(playerRect.X, playerRect.Y), new Vector2(playerRect.X, playerRect.Y), 1, 0, new Vector2(playerRect.Width, playerRect.Height), false, 1f);
            foreach(bool b in collisions)
            {
                if (b)
                {
                    inAWall = true;
                    break;
                }
            }

            return inAWall;
        }

        public void blockDataUpdate()
        {
            int[] blocks = new int[9];
            int x, y;

            for (x = 0; x < 3; x++)
            {
                for (y = 0; y < 3; y++)
                {
                    blocks[x * 3 + y] = arena.blockInfo(new Vector2(playerRect.X + x * (int)(playerRect.Width/3) - 1, playerRect.Y + y * (int)(playerRect.Height/3) - 1));
                }
            }

            foreach (int block in blocks)
            {
                if (block == 5 && !dead)
                {
                    setDead(true, null, "MAP");
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

            Boolean[] collisions = arena.collides(new Vector2(testX, testY), new Vector2(playerRect.X, playerRect.Y), d, r, new Vector2(playerRect.Width, playerRect.Height), blinked, 0f);

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
                        if ((blinked || velocity.Y > (TERMINAL_V / 3 * 2)) && !atRest)
                        {
                            Animation poof = new Animation(dustPoof, new Vector2(testX - 16, testY + playerRect.Height - 60), 20f, 10, aniList, directionFacing);
                        }
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
                        if (((blinked && velocity.Y > (TERMINAL_V / 4) || velocity.Y > (TERMINAL_V / 3 * 2))))
                        {
                            Animation poof = new Animation(dustPoof, new Vector2(testX - 16, testY + playerRect.Height - 60), 20f, 10, aniList, directionFacing);
                        }
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
                if (p != null && p != this && !p.dead && !alreadyChecked && blinked == p.blinked)
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
                        this.setDead(true, p, "STOMP");
                        //this.rectA.Height = rectA.Height / 2;
                        //this.rectA.Y += rectA.Height;
                    }
                    else
                    {
                        playerRect.Y -= thisDist;
                        p.playerRect.Y += otherDist;
                        this.velocity.Y = -20;
                        p.velocity.Y = 20;
                        p.setDead(true, this, "STOMP");
                        //rectB.Height = rectB.Height / 2;
                        //rectB.Y += rectB.Height;
                    }
                }
                return true;
            }
            return false;
        }

        //Getter for player dimensions for spear class
        public Rectangle getPlayerRect()
        {
            return playerRect;
                    }
        
        internal int getDirectionFacing()
        {
            return directionFacing;
                }

        public PlayerClass[] getPlayers()
        {
            return players;
            }

        internal void setDead(Boolean deathState, PlayerClass killer, String method)
        {
            dead = deathState;
            blinkJuice = MAXBLINKJUICE;
            if(spear != null)
            {
                spear.dropSpear();
            }
            blinked = false;
            throwKilled(this, killer, method);
        }

        private Rectangle getFrame(Rectangle rect)
        {
            if (directionFacing == 0)
                rect.X = 0;
            else
                rect.X = 36;
            return rect;
        }

        public void Draw(SpriteBatch sB)
        {
            if (!active)
                return;
			if(blinked && velocity.X == 0 && velocity.Y == 0)
			{
                return;
			}
			Color colorDrawn;
			if (blinked)
				colorDrawn = new Color(100, 100, 100, 10);
			else
				colorDrawn = Color.White;

            Texture2D drawnText = playerText;

            
            
            //THIS HEIGHT AND WIDTH IS HARDCODED. CHANGE THIS COLIN YOU LAZY ASS
            Rectangle frame = new Rectangle(0,0,36,68);

            int offX = (directionFacing == 0 ? (int)offset.X : (int)-offset.X);
            int offY = (int)offset.Y;

            Rectangle barFrame = new Rectangle(0, 0, 30, 6);
            barFrame.Width = (int)(30 * (blinkJuice / MAXBLINKJUICE));

            if (blinkJuice < MAXBLINKJUICE && !blinked)
            {
                sB.Draw(blinkRect, new Vector2(playerRect.X , playerRect.Y + offY - 12), barFrame, Color.Green);
            }

            frame = getFrame(frame);

            if (dead)
                drawnText = deadText;

            /*if (attackKeyDown)
            {
                SpriteEffects flip = SpriteEffects.None;
                if(VectorMath.rotationFromVector(spearVector) > Math.PI)
                    flip = SpriteEffects.FlipHorizontally;
                sB.Draw(drawnText, new Vector2(playerRect.X + offX, playerRect.Y + MARGIN + offY), frame, colorDrawn, 0f, new Vector2(0, 0), 0f, flip, 0f);
                

            }
            else*/
            sB.Draw(drawnText, new Vector2(playerRect.X + offX, playerRect.Y + MARGIN + offY), frame, colorDrawn);

            //Drawing when the player is looping over
            if (playerRect.X < playerRect.Width)
                sB.Draw(drawnText, new Vector2(playerRect.X + SCREENSIZE.X + offX, playerRect.Y + MARGIN + offY), frame, colorDrawn);
            else if (playerRect.X + playerRect.Width > SCREENSIZE.X)
                sB.Draw(drawnText, new Vector2(playerRect.X - (SCREENSIZE.X) + offX, playerRect.Y + MARGIN + offY), frame, colorDrawn);

            if (playerRect.Y < playerRect.Height)
                sB.Draw(drawnText, new Vector2(playerRect.X + offX, playerRect.Y + SCREENSIZE.Y + MARGIN + offY), frame, colorDrawn);
            else if (playerRect.Y + playerRect.Height > SCREENSIZE.Y)
                sB.Draw(drawnText, new Vector2(playerRect.X + offX, playerRect.Y - (SCREENSIZE.Y) + MARGIN + offY), frame, colorDrawn);
        }



        public void throwKilled(PlayerClass killed, PlayerClass killer, string method)
        {
            if (onPlayerKilled == null) return;
            Death_Sound.Play();
            if (killer == killed)
            {
                killer.score -= 1;
            }
            else if (killer != null)
                killer.score += 1;
            else
                killed.score -= 1;
            DeathEventArgs args = new DeathEventArgs(killed, killer, method);
            onPlayerKilled(this, args);
        }

        public void winner()
        {
            victory = true;
        }

        public Boolean isDead()
        {
            return dead;
        }

        //Getters/setters for stuff

        public void setPos(Vector2 pos)
        {
            playerRect.X = (int)pos.X;
            playerRect.Y = (int)pos.Y;
        }

        public float getMulti()
        {
            return curMultiplier;
        }

        public void reset(Map map)
        {
            dead = false;
            victory = false;
            velocity.X = 0;
            velocity.Y = 0;
            blinkCoolDown = 0;
            deathTimer = -2;
            stunTimer = 0;
            blinked = false;
            curMultiplier = 1f;
            blinkJuice = MAXBLINKJUICE;
            arena = map;
        }

        internal void jumpForJoy()
        {
            velocity.Y -= JUMP * curMultiplier;
            atRest = false;
        }
    }
}
