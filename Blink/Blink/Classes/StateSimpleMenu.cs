using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace Blink.GUI
{
    public class StateSimpleMenu : GameState
    {
        Vector2 screenSize;
        int selected;
        List<TextButton> buttons;
        IEnumerable<string> maps;
        Label title;

        String titleString;
        String[] optionsStrings;
        String selectedMap;
        GameState[] triggers;

        GameState nextState;

        bool lastMoveUp;
        bool lastMoveDown;
        bool lastAccept;

        public StateSimpleMenu(Vector2 screenSize, String title, String[] options, GameState[] triggers)
        {
            this.screenSize = screenSize;
            this.titleString = title;
            this.optionsStrings = options;
            this.buttons = new List<TextButton>();
            this.triggers = triggers;
        }

        public void Initialize()
        {
            this.selected = 0;
            this.nextState = null;
        }

        public void LoadContent(ContentManager Content)
        {


            //Gets a list of all the .map files in our mapdata folder
            maps = Directory.EnumerateFiles(Environment.CurrentDirectory+"\\Content\\MapData","*.map");
            //Storage list for all our map names
            List < string > mapNames = new List<string>();
            //For each map file, slice off the path to store just the map's name.
            foreach (string path in maps)
            {
                string mapName = path.Remove(0, Environment.CurrentDirectory.Length + "\\Content\\MapData".Length);
                mapName = mapName.Replace(".map", "");
                mapNames.Add(mapName);
                
            }

            //Temporary random map picker, remove this when a map select screen is implemented //////////////////
            Random r = new Random();
            int map = r.Next(mapNames.Count);

            selectedMap = mapNames[map];
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            
            Vector2 pos = new Vector2(screenSize.X/2, 50);
            title = new Label(titleString, Content.Load<SpriteFont>("miramo"), pos, new Vector2(0.5f, 0));
            pos.Y += 50;
            foreach (String s in optionsStrings)
            {
                pos.Y += 50;
                buttons.Add(new TextButton(s, Content.Load<SpriteFont>("miramo"), pos, Content.Load<Texture2D>("buttonUp"), 
                    Content.Load<Texture2D>("buttonDown"), new Vector2(0.5f, 0)));
            }

            buttons[0].Select();
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

            if (keyState.IsKeyDown(Keys.Up))
                moveUp = true;
            if (keyState.IsKeyDown(Keys.Down))
                moveDown = true;
            if (keyState.IsKeyDown(Keys.Enter))
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
                buttons[selected].UnSelect();
                selected++;
                selected %= buttons.Count;
                buttons[selected].Select();
            }
            if (moveDown && !lastMoveDown)
            {
                buttons[selected].UnSelect();
                selected--;
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

        public void Draw(SpriteBatch sb)
        {
            title.Draw(sb);
            foreach (TextButton b in buttons)
                b.Draw(sb);
        }

        public GameState GetTransition()
        {
            return nextState;
        }

        public string getSelectedMap()
        {
            return selectedMap;
        }
    }
}

