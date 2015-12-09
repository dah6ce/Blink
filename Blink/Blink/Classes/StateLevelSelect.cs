using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Blink.Classes;


namespace Blink.GUI
{

    public class StateLevelSelect : GameState
    {
        const int THUMBROWSIZE = 5;

        Vector2 screenSize;
        int selected;
        List<TextButton> buttons;
        IEnumerable<string> maps;
        Label title;

        String titleString;
        String[] optionsStrings;
        mapSet selectedMap;

        GameState nextState;
        public GameState prevState;
        GameState game;

        bool lastMoveLeft;
        bool lastMoveRight;
        bool lastAccept;
        bool prematureEnter;


        //Storage list for all our map names
        Dictionary<string, mapSet> mapNames = new Dictionary<string, mapSet>();
        List<mapSet> mapSets = new List<mapSet>();
        //Storage for large map images
        List<Texture2D> mapImages = new List<Texture2D>();
        List<ImageButton> mapThumbs = new List<ImageButton>();

        Texture2D menuOverlay;

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
            if (this.mapThumbs.Count > 0)
                this.mapThumbs[selected].unselect();
            this.selected = 0;
            this.nextState = null;

            AudioManager.TriggerLevelSelect();

            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (keyState.IsKeyDown(Keys.Enter))
            {
                prematureEnter = true;
            }
        }

        public void LoadContent(ContentManager Content)
        {
            if (mapNames.Count > 0)
            {
                foreach (mapSet ms in mapSets)
                    ms.unselect();
                this.mapSets[selected].select();
                return;
            }

            selectedOverlay = Content.Load<Texture2D>("MenuData/mapcursor");
            menuOverlay = Content.Load<Texture2D>("MenuData/mapselect");

            //Gets a list of all the .map files in our mapdata folder, Platform specific paths
            #if WINDOWS
            maps = Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Content\\MapData", "*.map");
            #elif LINUX
            maps = Directory.EnumerateFiles(Environment.CurrentDirectory + "/Content/MapData", "*.map");
            #endif

            //For each map file, slice off the path to store just the map's name.
            foreach (string path in maps)
            {
                string mapName = path.Remove(0, Environment.CurrentDirectory.Length + "\\Content\\MapData\\".Length);
                mapName = mapName.Replace(".map", "");
                string mapGroup = mapName.Split('_')[0];
                if (!mapNames.ContainsKey(mapGroup))
                {
                    mapNames.Add(mapGroup, new mapSet(mapGroup));
                    mapNames[mapGroup].setBackground(Content.Load<Texture2D>("MapData/" + mapGroup + "_background"));
                    mapNames[mapGroup].setColumn(mapSets.Count);
                    mapSets.Add(mapNames[mapGroup]);
                }
                mapSet group;
                mapNames.TryGetValue(mapGroup, out group);
                group.addMap(mapName);

                //Texture2D mapImage = Content.Load<Texture2D>("MapData/"+mapName + "Color");
                //mapImages.Add(mapImage);
                
                
            }

            //Get thumbnails
            foreach(mapSet set in mapSets)
            {
                set.getRandomThumbs(Content);
            }
            

            //positionThumbs(mapThumbs);

            Vector2 pos = new Vector2(screenSize.X / 2, 50);
            title = new Label(titleString, Content.Load<SpriteFont>("miramo"), pos, new Vector2(0.5f, 0));
            pos.Y += 50;

            mapSets[0].select();
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
            if (padState.IsButtonDown(Buttons.B))
            {
                nextState = prevState;
            }


            if (moveLeft && !lastMoveLeft)
            {
                mapSets[selected].unselect();
                selected--;
                if (selected % THUMBROWSIZE == 7)
                    selected += THUMBROWSIZE;
                else if(selected < 0)
                {
                    selected += mapSets.Count;
                }
                mapSets[selected].select();
            }
            if (moveRight && !lastMoveRight)
            {
                mapSets[selected].unselect();
                selected++;
                //if (selected % THUMBROWSIZE == 0)
                //    selected -= THUMBROWSIZE;
                if(selected >= mapSets.Count)
                {
                    selected = 0;//-= (selected % THUMBROWSIZE);
                }
                mapSets[selected].select();
            }
            if (accept && !lastAccept)
            {
                selectedMap = mapSets[selected];
                nextState = game;
            }

            lastAccept = accept;
            lastMoveRight = moveRight;
            lastMoveLeft = moveLeft;

        }

        public void Draw(SpriteBatch sb)
        {
            //title.Draw(sb);
            //sb.Draw(mapImages[selected], new Vector2(0,0));
            sb.Draw(mapSets[selected].getBackground(), new Vector2(0, 0), Color.White);
            foreach (mapSet set in mapSets)
            {
                Texture2D[] thumbnails = set.Thumbs();
                for(int i = 0; i < 5; i++)
                {
                    sb.Draw(thumbnails[i], new Rectangle(set.getColumn() * 215 + 262, i * 95 + 468,223,100), Color.White);
                }
                if (set.isSelected())
                {
                    sb.Draw(selectedOverlay, new Vector2(set.getColumn() * 215 + 262, 491), Color.Gold);
                }
            }
            sb.Draw(menuOverlay, new Vector2(0, 0), Color.White);
            //sb.Draw(selectedOverlay, new Vector2(200 * (selected % THUMBROWSIZE), (float)Math.Floor((selected / 8f)) * 120 + 600), Color.Gold);
        }

        public GameState GetTransition()
        {
            return nextState;
        }

        public mapSet getSelectedMap()
        {
            return selectedMap;
        }
    }
}

