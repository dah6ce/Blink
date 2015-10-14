using System;
using System.IO;
using Blink.GUI;
using Blink;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Blink.Classes;

/// <summary>
/// Spear object class
/// </summary>
namespace Blink.Classes
{
    class SpearClass
    {

        public Texture2D spearText;
        public float Width, Height;
        public Vector2 pos, velocity, SCREENSIZE;
        public int spearOrientation;
        public PlayerClass spearOwner;
        public Map m;
        public readonly Keys THROW_KEY = Keys.Q;
        public readonly Buttons THROW_BUTTON = Buttons.RightShoulder;
        public readonly Keys STAB_KEY = Keys.Space;
        public readonly Buttons STAB_BUTTON = Buttons.X;
        public Boolean attached = true;
        public Boolean isInUse = false;

        //Constructor for new spear
        //Takes inputs (Player, ScreenSize, Map)
        public SpearClass(PlayerClass spearOwner,Texture2D spear, Vector2 ScreenSize, /*necesary?*/ Map m)
        {
            this.spearText = spear;
            Height = spearOwner.oldPos.Y;
            Width = spearOwner.oldPos.X / 16;
            this.spearOwner = spearOwner;
            spearOrientation = 0;
            this.SCREENSIZE = ScreenSize;
            this.m = m;
        }

        //Manage inputs, check for a spear throw.
        public void Update(KeyboardState input, GamePadState padState)
        {
            isInUse = false;
            if (input.IsKeyDown(THROW_KEY) || padState.IsButtonDown(THROW_BUTTON))
            {
                throwSpear();
                isInUse = true;
            }

            else if ((input.IsKeyDown(STAB_KEY) || padState.IsButtonDown(STAB_BUTTON)) && !isInUse && attached)
            {
                isInUse = true;
            }
        }

        //Handle throw physics
        private void throwSpear()
        {
            attached = false;
        }

        public void Draw(SpriteBatch sB)
        {
            if (isInUse && attached)
            {
                sB.Draw(spearText, spearOwner.oldPos, Color.White);
            }
        }
    }
}
