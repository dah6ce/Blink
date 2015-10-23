using System;
using System.IO;
using Blink.GUI;
using Blink.Classes;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Blink
{
    enum PlayerKeys
    {
        Player1,
        Player2,
        Player3,
        Player4,
        allPlayers

    }

	public class StateGame : GameState
	{
		Vector2 screenSize;
        SpearClass spear1;
        SpearClass spear2;
        SpearClass spear3;
        SpearClass spear4;
		PlayerClass player1;
		PlayerClass player2;
		PlayerClass player3;
		PlayerClass player4;
        PlayerClass[] players = new PlayerClass[4];
        SpearClass[] spears = new SpearClass[4];

		PlayerKeys currPlayer;
		KeyboardState oldState;
		KeyboardState player1State;
		KeyboardState player2State;
		KeyboardState player3State;
		KeyboardState player4State;
        string mapName = "map1";
        float roundReset = -1;
        float timeElapsed;
        public static GameTime gameTime = new GameTime();
		Map map1;
		bool[] oldStartState = new bool[4];
		bool paused;
		int playerPaused;
		SpriteFont font;
        List<Animation> animations;

        public void setMap(string map)
        {
            mapName = map;
        }

		public StateGame(Vector2 screenSize)
		{
			this.screenSize = screenSize;
		}

		public void Initialize()
		{
			player1 = new PlayerClass();
			player2 = new PlayerClass();
			player3 = new PlayerClass();
			player4 = new PlayerClass();

            animations = new List<Animation>();

            player1.title = "p1";
            player2.title = "p2";
            player3.title = "p3";
            player4.title = "p4";

            player1.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);
            player2.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);
            player3.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);
            player4.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);

			map1 = new Map();
			currPlayer = PlayerKeys.Player1;
			paused = false;
			playerPaused = 0;
		}

		public void LoadContent(ContentManager Content)
		{
			Vector2 player1Pos = new Vector2(96, 96);
			Vector2 player2Pos = new Vector2(1400, 96);
			Vector2 player3Pos = new Vector2(400, 96);
			Vector2 player4Pos = new Vector2(1120, 96);
            
            players[0] = player1;
            players[1] = player2;
            players[2] = player3;
            players[3] = player4;


            Vector2 offset = new Vector2(-4, -4);
            Texture2D bar = Content.Load<Texture2D>("bar");
            Texture2D dust = Content.Load<Texture2D>("Dust_Trail");

            player1.Initialize(Content.Load<Texture2D>("ROTH-OG-SPEARLESS"), player1Pos, screenSize, map1, players, offset, bar);
            player2.Initialize(Content.Load<Texture2D>("ROTH-RED-SPEARLESS"), player2Pos, screenSize, map1, players, offset, bar);
            player3.Initialize(Content.Load<Texture2D>("ROTH-SILVER-SPEARLESS"), player3Pos, screenSize, map1, players, offset, bar);
            player4.Initialize(Content.Load<Texture2D>("ROTH-BLACK-SPEARLESS"), player4Pos, screenSize, map1, players, offset, bar);

            player1.deadText = Content.Load<Texture2D>("spriteDead");
            player2.deadText = Content.Load<Texture2D>("spriteDead");
            player3.deadText = Content.Load<Texture2D>("spriteDead");
            player4.deadText = Content.Load<Texture2D>("spriteDead");

            player1.dustEffect = dust;
            player2.dustEffect = dust;
            player3.dustEffect = dust;
            player4.dustEffect = dust;

            player1.aniList = animations;
            player2.aniList = animations;
            player3.aniList = animations;
            player4.aniList = animations;

            spear1 = new SpearClass(player1, Content.Load<Texture2D>("spearsprite"), screenSize, map1, players, spears);
            spear2 = new SpearClass(player2, Content.Load<Texture2D>("spearsprite"), screenSize, map1, players, spears);
            spear3 = new SpearClass(player3, Content.Load<Texture2D>("spearsprite"), screenSize, map1, players, spears);
            spear4 = new SpearClass(player4, Content.Load<Texture2D>("spearsprite"), screenSize, map1, players, spears);

            spears[0] = spear1;
            spears[1] = spear2;
            spears[2] = spear3;
            spears[3] = spear4;

            StreamReader mapData;
            mapData = File.OpenText("Content/MapData/"+mapName+".map");
            map1.Initialize(Content.Load<Texture2D>("MapData/"+mapName+"Color"), mapData.ReadToEnd(), 32, 50, 30, players);
            font = Content.Load<SpriteFont>("miramo30");
        }

		public void UnloadContent()
		{
			
		}

		public void Update(GameTime gameTime)
		{
			KeyboardState currState = Keyboard.GetState();

            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].Update(gameTime);
            }

			if(paused)
			{
				if (currState.IsKeyDown(Keys.P) && currState != oldState && playerPaused == (int)currPlayer)
				{
					paused = false;
					oldState = currState;
				}
				foreach (PlayerIndex x in Enum.GetValues(typeof(PlayerIndex)))
				{
					if (playerPaused == (int)x && GamePad.GetState(x).IsButtonDown(Buttons.Start) && !oldStartState[(int)x])
					{
						paused = false;
						oldStartState[(int)x] = GamePad.GetState(x).IsButtonDown(Buttons.Start);
                    }
				}
			}

			if (!paused)
			{
				if (currState.IsKeyDown(Keys.P) && currState != oldState)
				{
					paused = true;
					oldState = currState;
					playerPaused = (int)currPlayer;
				}
				foreach (PlayerIndex x in Enum.GetValues(typeof(PlayerIndex)))
				{
					if (GamePad.GetState(x).IsButtonDown(Buttons.Start) && !oldStartState[(int)x])
					{
						paused = true;
						playerPaused = (int)x;
					}
				}
			}

			if (paused)
			{
				oldState = currState;
				oldStartState[0] = GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start);
				oldStartState[1] = GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start);
				oldStartState[2] = GamePad.GetState(PlayerIndex.Three).IsButtonDown(Buttons.Start);
				oldStartState[3] = GamePad.GetState(PlayerIndex.Four).IsButtonDown(Buttons.Start);
				return;
			}

			/* Press TAB to change player if using keyboard. *** For Testing Purposes Only ***
                If you hold down a key while pressing TAB, the previous player will continue to do that same action
                over and over again until you tab to that player again. 
                (It is kinda amusing, but could be useful for collison testing) */
			if (currState.IsKeyDown(Keys.Tab) && oldState != currState)
			{
				switch ((int)currPlayer)
				{
					case 0:
						currPlayer = PlayerKeys.Player2;
						break;

					case 1:
						currPlayer = PlayerKeys.Player3;
						break;

					case 2:
						currPlayer = PlayerKeys.Player4;
						break;

					case 3:
						currPlayer = PlayerKeys.allPlayers;
						break;

					case 4:
						currPlayer = PlayerKeys.Player1;
						break;
				}
				//Switches to the next player
			}
			if (currPlayer == PlayerKeys.Player1)
			{
				player1State = Keyboard.GetState();
			}

			if (currPlayer == PlayerKeys.Player2)
			{
				player2State = Keyboard.GetState();
			}

			if (currPlayer == PlayerKeys.Player3)
			{
				player3State = Keyboard.GetState();
			}
			if (currPlayer == PlayerKeys.Player4)
			{
				player4State = Keyboard.GetState();
			}

			if (currPlayer == PlayerKeys.allPlayers)
			{
				player1State = Keyboard.GetState();
				player2State = Keyboard.GetState();
				player3State = Keyboard.GetState();
				player4State = Keyboard.GetState();
			}
			//End of TAB code. Can now only control one player at a time using keyboard.
			player1.Update(player1State, GamePad.GetState(PlayerIndex.One));
			player2.Update(player2State, GamePad.GetState(PlayerIndex.Two));
			player3.Update(player3State, GamePad.GetState(PlayerIndex.Three));
			player4.Update(player4State, GamePad.GetState(PlayerIndex.Four));
            foreach (SpearClass sp in spears)
            {
              
                    if (sp.spearOwner == player1)
                    {
                        sp.Update(player1State, GamePad.GetState(PlayerIndex.One));
                    }

                    if (sp.spearOwner == player2)
                    {
                        sp.Update(player2State, GamePad.GetState(PlayerIndex.Two));
                    }
                    if (sp.spearOwner == player3)
                    {
                        sp.Update(player3State, GamePad.GetState(PlayerIndex.Three));
                    }
                    if (sp.spearOwner == player4)
                    {
                        sp.Update(player4State, GamePad.GetState(PlayerIndex.Four));
                    }
                    else
                    {
                        KeyboardState fakestate = new KeyboardState();
                        GamePadState faker = new GamePadState();
                        sp.Update(fakestate, faker);
                    }
            }
			oldState = currState;

            oldStartState[0] = GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start);
            oldStartState[1] = GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start);
            oldStartState[2] = GamePad.GetState(PlayerIndex.Three).IsButtonDown(Buttons.Start);
            oldStartState[3] = GamePad.GetState(PlayerIndex.Four).IsButtonDown(Buttons.Start);
            //If a timer is running, decrement here

            timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(roundReset > 0)
            {
                roundReset -= timeElapsed;
                if(roundReset < 0)
                {
                    resetMap();
		}
            }
        }

		public void Draw(SpriteBatch sb)
		{
			map1.Draw(sb);
			player1.Draw(sb);
			player2.Draw(sb);
			player3.Draw(sb);
			player4.Draw(sb);
            spear1.Draw(sb);
            spear2.Draw(sb);
            spear3.Draw(sb);
            spear4.Draw(sb);

            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].Draw(sb);
            }

			if (paused)
			{
				sb.DrawString(font, "P" + (playerPaused + 1) + " paused", new Vector2(screenSize.X / 2, screenSize.Y / 2), Color.Black);
			}
			if (roundReset > 0)
			{
				
				Vector2 temp = new Vector2(screenSize.X / 2, 300);
				sb.DrawString(font, "SCORES", temp, Color.White);

				temp.Y += 32;
				for (int i = 0; i < players.Length; i ++)
				{
					sb.DrawString(font, "P" + (i + 1) + ": " + players[i].score, temp, Color.White);
					temp.Y += 32;
				}
			}
		}

		public GameState GetTransition() 
		{
			return null;
		}

        private void playerKilled(Object sender, DeathEventArgs args)
        {
            //Do things like announce death/method of death

            detectWinner();
        }

        public void detectWinner()
        {
            Boolean survivor = false;
            PlayerClass victor = null;
            foreach (PlayerClass p in players)
            {
                if (victor == null && !p.isDead())
                {
                    victor = p;
                    survivor = true;
                }
                else if (victor != null && !p.isDead())
                {
                    victor = null;
                    break;
                }
            }

            if (victor != null || (victor == null && !survivor))
            {
                declareVictor(victor);
            }
        }

        private void declareVictor(PlayerClass victor)
        {
            victor.winner();
            roundReset = 3;
        }

        private void resetMap()
        {
            map1.reset();
            foreach(PlayerClass p in players)
            {
                p.reset();
            }

            spear1.reset(players[0]);
            spear2.reset(players[1]);
            spear3.reset(players[2]);
            spear4.reset(players[3]);
        }
	}
}

