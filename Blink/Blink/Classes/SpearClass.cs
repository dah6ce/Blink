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
            Width = spearOwner.getPlayerRect().Width / 16;
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
            Texture2D drawnText = spearText;
            if (!isInUse && spearOwner!=null)
            {
                sB.Draw(drawnText, new Vector2(spearOwner.getPlayerRect().X, spearOwner.getPlayerRect().Y), Color.White);
            }
        }

        internal void setOwner(PlayerClass player)
        {
            spearOwner = player;
        }
    }
}
