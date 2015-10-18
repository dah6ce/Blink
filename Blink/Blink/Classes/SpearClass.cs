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
        public PlayerClass[] players;
        Rectangle spearRect = new Rectangle(0, 0, 83, 19);
        public Map m;
        public readonly Keys THROW_KEY = Keys.Q;
        public readonly Buttons THROW_BUTTON = Buttons.RightShoulder;
        public readonly Keys STAB_KEY = Keys.Space;
        public readonly Buttons STAB_BUTTON = Buttons.X;
        KeyboardState oldState;
        public Boolean attached = true;
        public Boolean isInUse = false;

        //Constructor for new spear
        //Takes inputs (Player, ScreenSize, Map)
        public SpearClass(PlayerClass spearOwner, Texture2D spear, Vector2 ScreenSize, /*necesary?*/ Map m, PlayerClass[] p)
        {
            this.spearText = spear;
            Height = spearOwner.oldPos.Y;
            Width = spearOwner.oldPos.X / 16;
            this.spearOwner = spearOwner;
            spearOrientation = 0;
            this.SCREENSIZE = ScreenSize;
            this.m = m;
            players = p;
            spearRect.X = (int)spearOwner.oldPos.X;
            spearRect.Y = (int)spearOwner.oldPos.Y;
        }

        //Manage inputs, check for a spear throw.
        public void Update(KeyboardState input, GamePadState padState)
        {
            KeyboardState newState = input;
            isInUse = false;
            if (input.IsKeyDown(THROW_KEY) || padState.IsButtonDown(THROW_BUTTON))
            {
                throwSpear();
                isInUse = true;
            }

            else if ((input.IsKeyDown(STAB_KEY) || padState.IsButtonDown(STAB_BUTTON)) && oldState != newState
                && !isInUse && attached && !spearOwner.dead)
            {
                isInUse = true;
            }
            playerCollision();
            oldState = newState;
        }

        //Handle throw physics
        private void throwSpear()
        {
            attached = false;
            spearOwner.hasSpear = false;
            //spearOwner = null;
            pos = new Vector2(96, 650);
            spearRect.X = (int)pos.X;
            spearRect.Y = (int)pos.Y;
        }

        private void playerCollision()
        {
            //Make sure we can actually collide with players/not already attached to a player
            if (!attached)
            {
                PlayerClass[] checkedPlayers = new PlayerClass[4];
                //Make sure we're not checking a collision that wasn't already checked
                foreach (PlayerClass p in players)
                {
                    Boolean alreadyChecked = false;
                    foreach (PlayerClass checkedPlayer in checkedPlayers)
                    {
                        if (checkedPlayer == p)
                            alreadyChecked = true;
                    }
                    if (!attached && !alreadyChecked)
                    {
                       // Console.WriteLine("Not Attached and Not Checked");
                        Rectangle inter = Rectangle.Intersect(p.playerRect, this.spearRect);
                        Console.WriteLine(p.hasSpear);
                        if (inter.Width > 0 && inter.Height > 0 && !p.hasSpear)
                        {
                            attached = true;
                            spearOwner = p;
                            p.hasSpear = true;
                        }
                    }
                }
            }
        }
        public void Draw(SpriteBatch sB)
        {
            if (isInUse && attached && !spearOwner.dead)
            {
                sB.Draw(spearText, spearOwner.oldPos, Color.White);
            }

            if (!attached)
            {
                sB.Draw(spearText, pos, Color.White);
            }
        }
    }
}
