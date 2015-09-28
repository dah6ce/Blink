using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Blink.Classes;
using System;
using System.IO;

namespace Blink
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    
    enum PlayerKeys {Player1, Player2, Player3, Player4, allPlayers}

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PlayerClass player1;
        PlayerClass player2;
        PlayerClass player3;
        PlayerClass player4;
        PlayerKeys currPlayer;
        KeyboardState oldState;
        KeyboardState player1State;
        KeyboardState player2State;
        KeyboardState player3State;
        KeyboardState player4State;
        Map map1;
        float GRAVITY = 0.1f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 960;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player1 = new PlayerClass();
            player2 = new PlayerClass();
            player3 = new PlayerClass();
            player4 = new PlayerClass();
            map1 = new Map();
            currPlayer = PlayerKeys.Player1;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);



            // TODO: use this.Content to load your game content here
            Vector2 player1Pos = new Vector2(96, 96);
            Vector2 player2Pos = new Vector2(1400, 96);
            Vector2 player3Pos = new Vector2(400, 96);
            Vector2 player4Pos = new Vector2(1120, 96);

            Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Right,GraphicsDevice.Viewport.TitleSafeArea.Bottom);
            player1.Initialize(Content.Load<Texture2D>("sprite"), player1Pos, screenSize, map1);
            player2.Initialize(Content.Load<Texture2D>("sprite"), player2Pos, screenSize, map1);
            player3.Initialize(Content.Load<Texture2D>("sprite"), player3Pos, screenSize, map1);
            player4.Initialize(Content.Load<Texture2D>("sprite"), player4Pos, screenSize, map1);

            StreamReader mapData;
            mapData = File.OpenText("Content/map1.map");
            map1.Initialize(Content.Load<Texture2D>("map1Color"), mapData.ReadToEnd(), 32, 50, 30);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
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
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            map1.Draw(spriteBatch);
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            player3.Draw(spriteBatch);
            player4.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
