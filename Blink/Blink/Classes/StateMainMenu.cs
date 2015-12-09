using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Blink.GUI;

namespace Blink
{
    class StateMainMenu : GameState {
        
        Vector2 screenSize;
        bool selected;

        GameState nextState;

        bool lastMoveUp;
        bool lastMoveDown;
        bool lastAccept;
        bool prematureEnter;

        Texture2D bg;
        Texture2D start, startS;
        Texture2D quit, quitS;

        public StateMainMenu(Vector2 screenSize)
        {
            this.screenSize = screenSize;
        }

        public void Initialize()
        {
            this.selected = true;
            this.nextState = null;

            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (keyState.IsKeyDown(Keys.Enter))
            {
                prematureEnter = true;
            }
        }

        public void LoadContent(ContentManager Content)
        {
            bg = Content.Load<Texture2D>("mainMenu");
            start = Content.Load<Texture2D>("start");
            startS = Content.Load<Texture2D>("startSelect");
            quit = Content.Load<Texture2D>("quit");
            quitS = Content.Load<Texture2D>("quitSelect");

           }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            bool moveUp = false;
            bool moveDown = false;
            bool accept = false;

            if (keyState.IsKeyUp(Keys.Enter))
            {
                prematureEnter = false;
            }

            if (keyState.IsKeyDown(Keys.Left))
                moveUp = true;
            if (keyState.IsKeyDown(Keys.Right))
                moveDown = true;
            if (keyState.IsKeyDown(Keys.Enter) && prematureEnter == false)
                accept = true;

            Vector2 thumbDir = padState.ThumbSticks.Left.ToPoint().ToVector2();
            if (thumbDir.Length() > .01)
            {
                if (thumbDir.Y > thumbDir.X)
                    moveUp = true;
                if (thumbDir.Y < thumbDir.X)
                    moveDown = true;
            }
            if (padState.IsButtonDown(Buttons.A))
                accept = true;


            if (moveUp && !lastMoveUp)
            {
                selected = !selected;
            }
            if (moveDown && !lastMoveDown)
            {
                selected = !selected;
            }
            if (accept && !lastAccept){
                if (selected) {
                    StateGame game = new StateGame(screenSize);
                    game.levelSelect = new StateMainMenu(screenSize);
                    nextState = new StateCharacterSelect(screenSize, game);
                }
                else
                    nextState = new StateQuit();
            }

            lastAccept = accept;
            lastMoveDown = moveDown;
            lastMoveUp = moveUp;

        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 scale = new Vector2(.25f, .25f);

            sb.Draw(bg,new Rectangle(0,0,(int)screenSize.X, (int)screenSize.Y), Color.White);
            if (selected)
                sb.Draw(startS, position: new Vector2(520, 820), origin: new Vector2(startS.Width/2, startS.Height / 2), scale: scale);
            else
                sb.Draw(quitS, position: new Vector2(1100, 820), origin: new Vector2(quitS.Width/2, quitS.Height / 2), scale: scale);

            sb.Draw(start, position: new Vector2(520, 820),origin: new Vector2(start.Width/2, start.Height / 2), scale: scale);
            sb.Draw(quit, position: new Vector2(1100, 820),origin: new Vector2(quit.Width/2, quit.Height / 2), scale: scale);
        }

        public GameState GetTransition()
        {
            return nextState;
        }
    }
}
    

