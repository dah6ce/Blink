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
        public GameState levelSelect;
        GameState nextState;
        string mapName = "map1";
        Map map1;
        public PlayerClass[] players;
        PlayerClass[] rank;
        Vector2[] pos;
        List<PlayerClass> rankp;
        List<PlayerClass> ranking;
        PlayerClass player1;
        PlayerClass player2;
        PlayerClass player3;
        PlayerClass player4;
        PlayerClass dummy;
        SpriteFont font;
      
        public StateWin(Vector2 screenSize)
        {
            this.screenSize = screenSize;
        }

        public void Initialize()
        {
            player1 = players[0];
            player2 = players[1];
            player3 = players[2];
            player4 = players[3];
            dummy = new PlayerClass();
            dummy.score = -100;
            rank = new PlayerClass[4];
            rankp = new List<PlayerClass>();
            ranking = new List<PlayerClass>();
            pos = new Vector2[4];
            nextState = null;
            map1 = new Map();
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState faker = new GamePadState();
            if (keyState.IsKeyDown(Keys.Enter))
            {
                nextState = levelSelect;
                return;
            }

            rank[0].jumpForJoy();
        }

        public void LoadContent(ContentManager Content)
        {
           
            pos[0] = new Vector2(96, 96);
            pos[1] = new Vector2(400, 96);
            pos[2] = new Vector2(1120, 96);
            pos[3] = new Vector2(1400, 96);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null)
                {
                    players[i].reset();
                    
                }
                ranking.Add(players[i]);
            }
            for(int i= 0; i < 4; i++) 
            {
                PlayerClass highest = dummy;
                int k = 0;
                int high = 0;
                foreach(PlayerClass p in ranking)
                {
                    if(p!= null)
                    {
                        if (p.score > highest.score)
                        {
                            highest = p;
                            high = k;
                        }
                    }
                    k++;
                }
                rankp.Add(highest);
                ranking.RemoveAt(high);
            }
          
            int left = 0;
            if (rankp.Count < 4)
            {
                left = 4 - rankp.Count;
            }
            for (int i = 0; i < left; i++)
            {
                PlayerClass faker = null;
                rankp.Add(faker);
            }
            rank = rankp.ToArray();
            StreamReader mapData;
            mapData = File.OpenText("Content/MapData/" + mapName + ".map");
            map1.Initialize(Content.Load<Texture2D>("MapData/" + mapName + "Color"), mapData.ReadToEnd(), 32, 50, 30, rank);
            font = Content.Load<SpriteFont>("miramo30");
        }

        public void Draw(SpriteBatch sb)
        {
            map1.Draw(sb);
            foreach (PlayerClass p in players)
            {
                if (p != null)
                {
                    p.Draw(sb);
                }
            }


            Vector2 temp = new Vector2(screenSize.X / 2 - font.MeasureString("SCORES").X / 2, 300);
            sb.DrawString(font, "SCORES", temp, Color.White);

            temp.Y += 32;
            for (int i = 0; i < rank.Length; i++)
            {
                if (rank[i] != null && rank[i].score != -100)
                {
                    sb.DrawString(font, "P" + (i + 1) + ": " + rank[i].score, temp, Color.White);
                    temp.Y += 32;
                }
            }
			    
        }

        public GameState GetTransition()
        {
            return nextState;
        }

        public void setMap(string map)
        {
            mapName = map;
        }

        public void setPlayers(PlayerClass[] players)
        {
            this.players = players;
        }

    }
}
