using System;
using System.IO;
using Blink.GUI;
using Blink.Classes;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Blink
{
    class StateWin : GameState
    {
        Vector2 screenSize;
        GameState nextState;
        string mapName = "map1";
        Map map1;
        public PlayerClass[] players;
        public GameState menu;
        //get what map you are on. 
        //load the correct win screen that corresponds to that map
        public StateWin(Vector2 screenSize)
        {
            this.screenSize = screenSize;
        }

        public void Initialize()
        {
            nextState = null;
            map1 = new Map();
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyUp(Keys.Enter))
            {
                nextState = menu;
                return;
            }
        }

        public void LoadContent(ContentManager Content)
        {
            Vector2 player1Pos = new Vector2(96, 96);
            Vector2 player2Pos = new Vector2(1400, 96);
            Vector2 player3Pos = new Vector2(400, 96);
            Vector2 player4Pos = new Vector2(1120, 96);

            StreamReader mapData;
            mapData = File.OpenText("Content/MapData/" + mapName + ".map");
            map1.Initialize(Content.Load<Texture2D>("MapData/" + mapName + "Color"), mapData.ReadToEnd(), 32, 50, 30, players);
        }

        public void Draw(SpriteBatch sb)
        {
            map1.Draw(sb);
        }

        public GameState GetTransition()
        {
            return nextState;
        }

        public void setMap(string map)
        {
            mapName = map;
        }

        public void getPlayers(PlayerClass[] players)
        {
            this.players = players;
        }

    }
}
