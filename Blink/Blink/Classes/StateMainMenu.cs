using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blink.GUI
{
    class StateMainMenu : GameState
    {
        Vector2 screenSize;
        int selected;
        List<TextButton> buttons;

        String[] optionsStrings;
        GameState[] triggers;

        GameState nextState;

        bool lastMoveUp;
        bool lastMoveDown;
        bool lastAccept;
        bool prematureEnter;

        Texture2D bg;

        public StateMainMenu(Vector2 screenSize, GameState[] triggers)
        {
            this.screenSize = screenSize;
            this.buttons = new List<TextButton>();
            this.triggers = triggers;
        }

        public void Initialize()
        {
            this.selected = 0;
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
            float x = screenSize.X / 2;
            float y = screenSize.Y - 180;
            bg = Content.Load<Texture2D>("MenuData/title");
            buttons.Add(new TextButton("Start", Content.Load<SpriteFont>("miramo"), new Vector2(x - 200, y),
                Content.Load<Texture2D>("buttonUp"), Content.Load<Texture2D>("buttonDown"), new Vector2(0.5f, 0)));
            buttons.Add(new TextButton("Credits", Content.Load<SpriteFont>("miramo"), new Vector2(x, y),
                Content.Load<Texture2D>("buttonUp"), Content.Load<Texture2D>("buttonDown"), new Vector2(0.5f, 0)));
            buttons.Add(new TextButton("Quit", Content.Load<SpriteFont>("miramo"), new Vector2(x + 200, y),
                Content.Load<Texture2D>("buttonUp"), Content.Load<Texture2D>("buttonDown"), new Vector2(0.5f, 0)));
            buttons[0].Select();
        }

        public void UnloadContent()
        {
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(bg, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
            foreach (TextButton b in buttons)
                b.Draw(sb);
        }

        public void reset()
        {
            this.selected = 0;
            this.nextState = null;
            lastMoveDown = false;
            lastAccept = false;
            lastAccept = false;

            buttons[1].UnSelect();
            buttons[2].UnSelect();
            buttons[0].Select();
            Console.WriteLine("test");
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
                if (thumbDir.X > thumbDir.Y)
                    moveDown = true;
                if (thumbDir.X < thumbDir.Y)
                    moveUp = true;
            }
            if (padState.IsButtonDown(Buttons.A))
                accept = true;


            if (moveUp && !lastMoveUp)
            {
                buttons[selected].UnSelect();
                selected--;
                // Need extra step because mod of a negative is negative
                selected = (selected + buttons.Count) % buttons.Count;
                buttons[selected].Select();
            }
            if (moveDown && !lastMoveDown)
            {
                buttons[selected].UnSelect();
                selected++;
                selected = (selected + buttons.Count) % buttons.Count;
                buttons[selected].Select();
            }
            if (accept && !lastAccept)
            {
                nextState = triggers[selected];
            }

            lastAccept = accept;
            lastMoveDown = moveDown;
            lastMoveUp = moveUp;

        }

        public GameState GetTransition()
        {
            return nextState;
        }
    }
}
