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
        int ultimateWin = -1;
        internal PlayerClass[] players = new PlayerClass[4];
        SpearClass[] spears = new SpearClass[4];

        bool[] playersInGame = { false, false, false, false };
        string[] playerTexts = { "", "", "", "" };

        public GameState levelSelect;
        public GameState Win;
        GameState returnState;

        SoundEffectInstance p1Death;
        SoundEffectInstance p2Death;
        SoundEffectInstance p3Death;
        SoundEffectInstance p4Death;

        Random mapPicker = new Random();

        PlayerKeys currPlayer;
		KeyboardState oldState;
		KeyboardState player1State;
		KeyboardState player2State;
		KeyboardState player3State;
		KeyboardState player4State;
        mapSet maps;
        float roundReset = -1;
        float timeElapsed;
        public static GameTime gameTime = new GameTime();
		Map[] mapObs = new Map[5];
        int currentMap = 0;
		bool[] oldStartState = new bool[4];
		bool paused;
		int playerPaused;
        int activePlayers = 0;
		SpriteFont font;
        List<Animation> animations;

        public void setMaps(mapSet map)
        {
            maps = map;
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

            returnState = null;

            animations = new List<Animation>();

            player1.title = "P1";
            player2.title = "P2";
            player3.title = "P3";
            player4.title = "P4";

            player1.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);
            player2.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);
            player3.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);
            player4.onPlayerKilled += new PlayerClass.PlayerKilledHandler(playerKilled);
            
			currPlayer = PlayerKeys.Player1;
			paused = false;
			playerPaused = 0;
            AudioManager.TriggerBattle();
            
		}

        public void informGame(bool[] playersConnected, string[] playerChars)
        {
            playersInGame = playersConnected;
            playerTexts = playerChars;
        }

		public void LoadContent(ContentManager Content)
		{
			Vector2 negPos = new Vector2(-100, -100);
            activePlayers = 0;
            if (playersInGame[0]) { 
                players[0] = player1;
                activePlayers++;
            }
            else
                player1.active = false;
            if (playersInGame[1]) { 
                players[1] = player2;
                activePlayers++;
            }
            else
                player2.active = false;
            if (playersInGame[2]) { 
                players[2] = player3;
                activePlayers++;
            }
            else
                player3.active = false;
            if (playersInGame[3]) { 
                players[3] = player4;
                activePlayers++;
            }
            else
                player4.active = false;


            Vector2 offset = new Vector2(-4, -4);
            Texture2D bar = Content.Load<Texture2D>("bar");
            Texture2D dust = Content.Load<Texture2D>("Dust_Trail");
            Texture2D dustPoof = Content.Load<Texture2D>("Dust_Poof");

            for(int i = 0; i < 4; i++)
            {
                if (players[i] != null && players[i].active)
                {
                    players[i].Initialize(Content.Load<Texture2D>(playerTexts[i]), negPos, screenSize, null, players, offset, bar);
                }
            }
            


            //Setup for common player resources
            foreach (PlayerClass p in players)
            {
                if (p != null)
                {

                    p.deadText = Content.Load<Texture2D>("spriteDead");

                    p.Death_Sound = Content.Load<SoundEffect>("audio/sfx/Player_Death").CreateInstance();

                    p.Jump_Sound = Content.Load<SoundEffect>("audio/sfx/Player_Jump").CreateInstance();
            
                    p.Blink_Sound = Content.Load<SoundEffect>("audio/sfx/Player_Blink").CreateInstance();

                    p.Unblink_Sound = Content.Load<SoundEffect>("audio/sfx/Player_Blink").CreateInstance();

                    p.dustEffect = dust;

                    p.dustPoof = dustPoof;

                    p.aniList = animations;

                }
            }

            
            spear1 = new SpearClass(player1, Content.Load<Texture2D>("spearsprite"), screenSize, null, players);
            spear2 = new SpearClass(player2, Content.Load<Texture2D>("spearsprite"), screenSize, null, players);
            spear3 = new SpearClass(player3, Content.Load<Texture2D>("spearsprite"), screenSize, null, players);
            spear4 = new SpearClass(player4, Content.Load<Texture2D>("spearsprite"), screenSize, null, players);

            spears[0] = spear1;
            spears[1] = spear2;
            spears[2] = spear3;
            spears[3] = spear4;


            //Setup for common spear resources
            foreach (SpearClass s in spears)
            {
                s.Throw_Sound = Content.Load<SoundEffect>("audio/sfx/Spear_Throw").CreateInstance();

                s.Hit_Player_Sound = Content.Load<SoundEffect>("audio/sfx/Spear_Player_Hit").CreateInstance();

                s.Hit_Wall_Sound = Content.Load<SoundEffect>("audio/sfx/Spear_Wall_Hit").CreateInstance();

                s.Stab_Sound = Content.Load<SoundEffect>("audio/sfx/Spear_Attack").CreateInstance();

            }

            StreamReader[] mapData = new StreamReader[5];
            for(int i = 0; i < 5; i++) { 
                mapData[i] = File.OpenText("Content/MapData/"+maps.Maps()[i]+".map");
                mapObs[i] = new Map();
                mapObs[i].Initialize(Content.Load<Texture2D>("MapData/"+maps.Maps()[i]+"Color"), mapData[i].ReadToEnd(), 32, 50, 30, players);
            }
            font = Content.Load<SpriteFont>("miramo30");

            resetMap();
        }
        

		public void UnloadContent()
		{
            /*p1Death.Dispose();
            p2Death.Dispose();
            p3Death.Dispose();
            p4Death.Dispose();
            */
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

                GamePadState p1 = GamePad.GetState(PlayerIndex.One);
                if (p1.IsButtonDown(Buttons.LeftTrigger) && p1.IsButtonDown(Buttons.RightTrigger) && p1.IsButtonDown(Buttons.LeftShoulder) && p1.IsButtonDown(Buttons.RightShoulder))
                    returnState = levelSelect;
                GamePadState p2 = GamePad.GetState(PlayerIndex.Two);
                if (p2.IsButtonDown(Buttons.LeftTrigger) && p2.IsButtonDown(Buttons.RightTrigger) && p2.IsButtonDown(Buttons.LeftShoulder) && p2.IsButtonDown(Buttons.RightShoulder))
                    returnState = levelSelect;
                GamePadState p3 = GamePad.GetState(PlayerIndex.Three);
                if (p3.IsButtonDown(Buttons.LeftTrigger) && p3.IsButtonDown(Buttons.RightTrigger) && p3.IsButtonDown(Buttons.LeftShoulder) && p3.IsButtonDown(Buttons.RightShoulder))
                    returnState = levelSelect;
                GamePadState p4 = GamePad.GetState(PlayerIndex.Four);
                if (p4.IsButtonDown(Buttons.LeftTrigger) && p4.IsButtonDown(Buttons.RightTrigger) && p4.IsButtonDown(Buttons.LeftShoulder) && p4.IsButtonDown(Buttons.RightShoulder))
                    returnState = levelSelect;
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                    returnState = levelSelect;
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
			player1.Update(player1State, GamePad.GetState(PlayerIndex.One), gameTime);
            player2.Update(player2State, GamePad.GetState(PlayerIndex.Two), gameTime);
            player3.Update(player3State, GamePad.GetState(PlayerIndex.Three), gameTime);
            player4.Update(player4State, GamePad.GetState(PlayerIndex.Four), gameTime);
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
                    if (ultimateWin > 0)
                    {
                        returnState = Win;
                        return;
                    }
                    else
                    {
                    resetMap();
		        }
            }
            }

            //Update animations
            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].Update(gameTime);
            }
        }

		public void Draw(SpriteBatch sb)
		{
			mapObs[currentMap].Draw(sb);
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
                string pauseMessage = "P" + (playerPaused + 1) + " paused";
                sb.DrawString(font, pauseMessage, new Vector2(screenSize.X / 2 - font.MeasureString(pauseMessage).X/2, screenSize.Y / 2), Color.Black);
			}
			if (roundReset > 0)
			{
                Vector2 temp = new Vector2(screenSize.X / 2 - font.MeasureString("SCORES").X / 2, 300);                
				    sb.DrawString(font, "SCORES", temp, Color.White);

				    temp.Y += 32;
                    for (int i = 0; i < players.Length; i++)
				    {
                            if (players[i] != null)
                            {
                                sb.DrawString(font, "P" + (i + 1) + ": " + players[i].score, temp, Color.White);
                                temp.Y += 32;
                            }
				    }
			    
		    }
		}

		public GameState GetTransition() 
		{
			return returnState;
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
                if(p != null) { 
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
            }

            if (victor != null || (victor == null && !survivor))
            {
                declareVictor(victor);
            }
        }

        private void declareVictor(PlayerClass victor)
        {
            if(victor != null)
                victor.winner();
            roundReset = 3;
            detectEndOfGame();
        }

        public void detectEndOfGame()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if(players[i] != null)
                {

                     if(players[i].score >= winScore())
                    {
                        roundReset = 5;
                        ultimateWin = i + 1;


                    }
                }
            }
        }
        public int winScore()
        {
            if (activePlayers == 4) return 10;
            if (activePlayers == 3) return 7;
            if (activePlayers == 2) return 5;
            return 5;
        }
        private void resetMap()
        {
            currentMap = (int)Math.Floor((double)mapPicker.Next(5));
            mapObs[currentMap].reset();
            foreach(PlayerClass p in players)
            {
                if(p != null)
                p.reset(mapObs[currentMap]);
            }
            ultimateWin = -1;
            spear1.reset(players[0],mapObs[currentMap]);
            spear2.reset(players[1], mapObs[currentMap]);
            spear3.reset(players[2], mapObs[currentMap]);
            spear4.reset(players[3], mapObs[currentMap]);
        }
	}
}

