using System;
using System.IO;
using Blink.GUI;
using Blink.Classes;
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
		PlayerClass player1;
		PlayerClass player2;
		PlayerClass player3;
		PlayerClass player4;
        PlayerClass[] players = new PlayerClass[4];
		PlayerKeys currPlayer;
		KeyboardState oldState;
		KeyboardState player1State;
		KeyboardState player2State;
		KeyboardState player3State;
		KeyboardState player4State;
		Map map1;
		bool paused;
		int playerPaused;
		SpriteFont font;

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
            player1.title = "p1";
            player2.title = "p2";
            player3.title = "p3";
            player4.title = "p4";
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

            player1.Initialize(Content.Load<Texture2D>("sprite"), player1Pos, screenSize, map1, players);
            player2.Initialize(Content.Load<Texture2D>("sprite"), player2Pos, screenSize, map1, players);
            player3.Initialize(Content.Load<Texture2D>("sprite"), player3Pos, screenSize, map1, players);
            player4.Initialize(Content.Load<Texture2D>("sprite"), player4Pos, screenSize, map1, players);

            player1.deadText = Content.Load<Texture2D>("spriteDead");
            player2.deadText = Content.Load<Texture2D>("spriteDead");
            player3.deadText = Content.Load<Texture2D>("spriteDead");
            player4.deadText = Content.Load<Texture2D>("spriteDead");

            StreamReader mapData;
            mapData = File.OpenText("Content/map1.map");
            map1.Initialize(Content.Load<Texture2D>("map1Color"), mapData.ReadToEnd(), 32, 50, 30, players);
			font = Content.Load<SpriteFont>("miramo");
        }

		public void UnloadContent()
		{
			
		}

		public void Update(GameTime gameTime)
		{
			KeyboardState currState = Keyboard.GetState();

			if(paused)
			{
				if (currState.IsKeyDown(Keys.P) && currState != oldState && playerPaused == (int)currPlayer)
				{
					paused = false;
					oldState = currState;
				}
				foreach (PlayerIndex x in Enum.GetValues(typeof(PlayerIndex)))
				{
					if (playerPaused == (int)x && GamePad.GetState(x).Buttons.Start == ButtonState.Pressed)
					{
						paused = false;
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
					if (GamePad.GetState(x).Buttons.Start == ButtonState.Pressed)
					{
						paused = true;
						playerPaused = (int)x;
					}
				}
			}

			if (paused)
			{
				oldState = currState;
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
			oldState = currState;
        }

		public void Draw(SpriteBatch sb)
		{
			map1.Draw(sb);
			player1.Draw(sb);
			player2.Draw(sb);
			player3.Draw(sb);
			player4.Draw(sb);

			if (paused)
			{
				sb.DrawString(font, "P" + (playerPaused + 1) + " paused", new Vector2(screenSize.X / 2, screenSize.Y / 2), Color.Black);
			}
		}

		public GameState GetTransition() 
		{
			return null;
		}
	}
}

