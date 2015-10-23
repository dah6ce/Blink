using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace Blink.GUI
{

    public class StateLevelSelect : GameState
    {
        const int THUMBROWSIZE = 8;

        Vector2 screenSize;
        int selected;
        List<TextButton> buttons;
        IEnumerable<string> maps;
        Label title;

        String titleString;
        String[] optionsStrings;
        String selectedMap;

        GameState nextState;
        GameState game;

        bool lastMoveLeft;
        bool lastMoveRight;
        bool lastAccept;
        bool prematureEnter;


        //Storage list for all our map names
        List<string> mapNames = new List<string>();
        //Storage for large map images
        List<Texture2D> mapImages = new List<Texture2D>();
        List<mapThumb> mapThumbs = new List<mapThumb>();

        Texture2D selectedOverlay;

        public StateLevelSelect(Vector2 screenSize, String title, String[] options, GameState g)
        {
            this.game = g;
            this.screenSize = screenSize;
            this.titleString = title;
            this.optionsStrings = options;
            this.buttons = new List<TextButton>();
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

            selectedOverlay = Content.Load<Texture2D>("select");

            //Gets a list of all the .map files in our mapdata folder
            maps = Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Content\\MapData", "*.map");

            //For each map file, slice off the path to store just the map's name.
            foreach (string path in maps)
            {
                string mapName = path.Remove(0, Environment.CurrentDirectory.Length + "\\Content\\MapData\\".Length);
                mapName = mapName.Replace(".map", "");
                mapNames.Add(mapName);
                Texture2D mapImage = Content.Load<Texture2D>("MapData/"+mapName + "Color");
                mapImages.Add(mapImage);
                Texture2D mapThumbtext = Content.Load<Texture2D>("MapData/"+mapName + "Thumb");
                mapThumb thumb = new mapThumb(mapThumbtext, new Vector2(), mapName);
                thumb.selectionOverlay = selectedOverlay;
                mapThumbs.Add(thumb);
            }

            positionThumbs(mapThumbs);

            Vector2 pos = new Vector2(screenSize.X / 2, 50);
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

        private void positionThumbs(List<mapThumb> thumbs)
        {
            for(int i = 0; i < thumbs.Count; i++)
            {
                mapThumb thumb = thumbs[i];
                thumb.setPosition(new Vector2(200 * (i % THUMBROWSIZE), (float)Math.Floor((i / 8f)) * 120 + 600));
            }
        }

        public void UnloadContent()
        {

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
                mapThumbs[selected].unselect();
                selected--;
                if (selected % THUMBROWSIZE == 7)
                    selected += THUMBROWSIZE;
                else if(selected < 0)
                {
                    if (mapThumbs.Count > 8)
                        selected = 7;
                    else
                    {
                        selected = mapThumbs.Count - 1;
                    }
                }
                mapThumbs[selected].select();
            }
            if (moveRight && !lastMoveRight)
            {
                mapThumbs[selected].unselect();
                selected++;
                if (selected % THUMBROWSIZE == 0)
                    selected -= THUMBROWSIZE;
                else if(selected >= mapThumbs.Count)
                {
                    selected -= (selected % THUMBROWSIZE);
                }
                mapThumbs[selected].select();
            }
            if (accept && !lastAccept)
            {
                selectedMap = mapNames[selected];
                nextState = game;
            }

            lastAccept = accept;
            lastMoveRight = moveRight;
            lastMoveLeft = moveLeft;

        }

        public void Draw(SpriteBatch sb)
        {
            //title.Draw(sb);
            sb.Draw(mapImages[selected], new Vector2(0,0));
            foreach (mapThumb thumb in mapThumbs)
                thumb.Draw(sb);
            //sb.Draw(selectedOverlay, new Vector2(200 * (selected % THUMBROWSIZE), (float)Math.Floor((selected / 8f)) * 120 + 600), Color.Gold);
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

