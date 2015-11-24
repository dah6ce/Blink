using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace Blink.GUI
{

    public class StateCharacterSelect : GameState
    {
        const int THUMBROWSIZE = 8;

        Vector2 screenSize;
        int selected;
        IEnumerable<string> chars;

        String titleString;

        GameState nextState;
        GameState game;

        bool lastMoveLeft;
        bool lastMoveRight;
        bool lastAccept;
        bool prematureEnter;


        //Storage list for all our char names
        List<string> charNames = new List<string>();
        List<ImageButton> charThumbs = new List<ImageButton>();

        Texture2D selectedOverlay;

        public StateCharacterSelect(Vector2 screenSize, String title, GameState g)
        {
            this.game = g;
            this.screenSize = screenSize;
            this.titleString = title;
        }

        public void Initialize()
        {
            if (this.charThumbs.Count > 0)
                this.charThumbs[selected].unselect();
            this.selected = 0;
            this.nextState = null;
            AudioManager.TriggerCharacterSelect();
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (keyState.IsKeyDown(Keys.Enter))
            {
                prematureEnter = true;
            }
        }

        public void LoadContent(ContentManager Content)
        {
            if (charNames.Count > 0)
            {
                this.charThumbs[selected].select();
                return;
            }

            selectedOverlay = Content.Load<Texture2D>("select");

            //Gets a list of all the .char files in our chardata folder, Platform specific paths
            #if WINDOWS
            chars = Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Content\\CharData", "*.char");
            #elif LINUX
            chars = Directory.EnumerateFiles(Environment.CurrentDirectory + "/Content/CharData", "*.char");
            #endif

            //For each char file, slice off the path to store just the char's name.
            foreach (string path in chars)
            {
                string charName = path.Remove(0, Environment.CurrentDirectory.Length + "\\Content\\CharData\\".Length);
                charName = charName.Replace(".char", "");
                charNames.Add(charName);
                Texture2D charThumbtext = Content.Load<Texture2D>("CharData/"+charName + "Thumb");
                ImageButton thumb = new ImageButton(charThumbtext, new Vector2(), charName);
                thumb.selectionOverlay = selectedOverlay;
                charThumbs.Add(thumb);
            }

            positionThumbs(charThumbs);

            Vector2 pos = new Vector2(screenSize.X / 2, 50);
            pos.Y += 50;

            charThumbs[0].select();
        }

        public void UnloadContent()
        {

        }

        private void positionThumbs(List<ImageButton> thumbs)
        {
            for(int i = 0; i < thumbs.Count; i++)
            {
                ImageButton thumb = thumbs[i];
                thumb.setPosition(new Vector2(200 * (i % THUMBROWSIZE), (float)Math.Floor((i / 8f)) * 120 + 600));
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            bool moveLeft = false;
            bool moveRight = false;
            bool accept = false;

            if (keyState.IsKeyUp(Keys.Enter))
            {
                prematureEnter = false;
            }

            if (keyState.IsKeyDown(Keys.Left))
                moveLeft = true;
            if (keyState.IsKeyDown(Keys.Right))
                moveRight = true;
            if (keyState.IsKeyDown(Keys.Enter) && prematureEnter == false)
                accept = true;

            Vector2 thumbDir = padState.ThumbSticks.Left.ToPoint().ToVector2();
            if (thumbDir.Length() > .01)
            {
                if (thumbDir.X < 0)
                    moveLeft = true;
                if (thumbDir.X > 0)
                    moveRight = true;
            }
            if (padState.IsButtonDown(Buttons.A))
                accept = true;


            if (moveLeft && !lastMoveLeft)
            {
                charThumbs[selected].unselect();
                selected--;
                if (selected % THUMBROWSIZE == 7)
                    selected += THUMBROWSIZE;
                else if(selected < 0)
                {
                    if (charThumbs.Count > 8)
                        selected = 7;
                    else
                    {
                        selected = charThumbs.Count - 1;
                    }
                }
                charThumbs[selected].select();
            }
            if (moveRight && !lastMoveRight)
            {
                charThumbs[selected].unselect();
                selected++;
                if (selected % THUMBROWSIZE == 0)
                    selected -= THUMBROWSIZE;
                else if(selected >= charThumbs.Count)
                {
                    selected -= (selected % THUMBROWSIZE);
                }
                charThumbs[selected].select();
            }
            if (accept && !lastAccept)
            {
                nextState = game;
            }

            lastAccept = accept;
            lastMoveRight = moveRight;
            lastMoveLeft = moveLeft;

        }

        public void Draw(SpriteBatch sb)
        {
            //title.Draw(sb);
            foreach (ImageButton thumb in charThumbs)
                thumb.Draw(sb);
            //sb.Draw(selectedOverlay, new Vector2(200 * (selected % THUMBROWSIZE), (float)Math.Floor((selected / 8f)) * 120 + 600), Color.Gold);
        }

        public GameState GetTransition()
        {
            return nextState;
        }


    }
}

