﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Blink.Classes;
using System;
using Blink.GUI;
using System.IO;

namespace Blink
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>



    public class Game1 : Game
    {
        public static bool running;

        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState currState;
        GameState mainMenu;
        GameState charMenu;
        GameState levelMenu;
        GameState game;
		GameState creditsMenu;
        GameState winScreen;

        public Game1()
        {
            running = true;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 960;
            //graphics.ToggleFullScreen();
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


            base.Initialize();
            AudioManager.Initialize();  
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Should probably be in Initialize, but screen size is updated after Initialize causing weird collision issues
            Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Right, GraphicsDevice.Viewport.TitleSafeArea.Bottom);
            game = new StateGame(screenSize);
            levelMenu = new StateLevelSelect(screenSize, "Map Select", new string[] { "Start", "Start", "Quit" }, game);
			creditsMenu = new StateCredits(screenSize, "Credits", new string[] { "Menu" });
			charMenu = new StateCharacterSelect(screenSize, "Character Select", levelMenu, game);
            //mainMenu = new StateSimpleMenu(screenSize, "Blink", new string[] { "Start", "Credits", "Quit" }, new GameState[] { charMenu, creditsMenu, new StateQuit() });
            mainMenu = new StateMainMenu(screenSize, new GameState[] { charMenu, creditsMenu, new StateQuit() });
            winScreen = new StateWin(screenSize);
            ((StateGame)game).levelSelect = levelMenu;
            ((StateGame)game).Win = winScreen;
			((StateCredits)creditsMenu).getMenu(mainMenu);
            ((StateLevelSelect)levelMenu).prevState = charMenu;
            ((StateWin)winScreen).levelSelect = levelMenu;
            currState = mainMenu;
            currState.Initialize();
            currState.LoadContent(Content);
            AudioManager.LoadContent(Content);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            currState.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || !running)
                Exit();

            currState.Update(gameTime);
            AudioManager.Update(gameTime);

            // Handle GameState transitions
            GameState newState = currState.GetTransition();
            if (newState != null)
            {
                //Set map
                if(newState == game)
                    ((StateGame)game).setMaps(((StateLevelSelect)levelMenu).getSelectedMap());
                if (newState == winScreen)
                {
                    ((StateWin)winScreen).setMap(((StateLevelSelect)levelMenu).getSelectedMap().getName()+"/victory");
                    ((StateWin)winScreen).setPlayers(((StateGame)game).players);
                }

                //State unload/load
                currState.UnloadContent();
                currState = newState;
                currState.Initialize();
                currState.LoadContent(Content);

            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            currState.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
