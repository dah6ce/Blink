using System;

using System.IO;
using Blink.GUI;
using Blink;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Blink.Classes;

namespace Blink.Classes
{
    class SpearClass
    {
	    
        public Texture2D spearText;
        public int Width, Height;
        public Vector2 pos, velocity, SCREENSIZE;
        public int spearOrientation;
        public PlayerClass spearOwner;
        public Map m;
        public readonly Keys THROW_KEY = Keys.Q;
        public readonly Buttons THROW_BUTTON = Buttons.RightShoulder;

        //Attacking or being thrown sets this to true
        public Boolean isInUse = false;

        //Constructor for new spear
        //Takes inputs (Player, ScreenSize, Map)
        public SpearClass(PlayerClass spearOwner, Texture2D spearText, Vector2 ScreenSize, /*necesary?*/ Map m)
        {
            this.spearText = spearText;
            Height = spearOwner.getPlayerRect().Height;
            Width = spearOwner.getPlayerRect().Width / 10;
            this.spearOwner = spearOwner;
            spearOrientation = 0;
            this.SCREENSIZE = ScreenSize;
            this.m = m;
        }

        //Manage inputs, check for a spear throw.
        public void Update(KeyboardState input, GamePadState padState)
        {
            if (input.IsKeyDown(THROW_KEY) || padState.IsButtonDown(THROW_BUTTON))
            {
                throwSpear();
            }
        }

        //Handle throw physics
        private void throwSpear()
        {

        }

        public void Draw(SpriteBatch sB)
        {
            Vector2 origin = new Vector2(0, 0);
            Vector2 screenPos = new Vector2(spearOwner.getPlayerRect().X, spearOwner.getPlayerRect().Y);
            float RotationAngle = 0;
            Texture2D drawnText = spearText;
            if (!isInUse && spearOwner!=null)
            {
                RotationAngle = (float)(MathHelper.Pi * 1.5);
                screenPos.Y += spearOwner.getPlayerRect().Height;
                if(spearOwner.getDirectionFacing() == 0)
                    screenPos.X += spearOwner.getPlayerRect().Width;
                else if (spearOwner.getDirectionFacing() == 1)
                    screenPos.X -= 3*Width;
                //Drawing when the player is looping over
                if (spearOwner.getDirectionFacing() == 0 && spearOwner.getPlayerRect().X + spearOwner.getPlayerRect().Width > SCREENSIZE.X)
                {
                    screenPos.X = (spearOwner.getPlayerRect().X + spearOwner.getPlayerRect().Width) - SCREENSIZE.X;
                    System.Diagnostics.Debug.WriteLine(screenPos.X);
                    sB.Draw(drawnText, screenPos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                }
                else
                    sB.Draw(drawnText, screenPos, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
            }
        }

        internal void setOwner(PlayerClass player)
        {
            spearOwner = player;
        }
    }
}
