using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Blink.Classes;

namespace Blink
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PlayerClass player1;
        PlayerClass player2;
        PlayerClass player3;
        PlayerClass player4;
        Map map1;
        float GRAVITY = 0.1f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
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
            Vector2 player1Pos = new Vector2(48, 48);
            Vector2 player2Pos = new Vector2(700, 48);
            Vector2 player3Pos = new Vector2(200, 48);
            Vector2 player4Pos = new Vector2(560, 48);

            Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Right,GraphicsDevice.Viewport.TitleSafeArea.Bottom);
            player1.Initialize(Content.Load<Texture2D>("sprite"), player1Pos, screenSize, map1);
            player2.Initialize(Content.Load<Texture2D>("sprite"), player2Pos, screenSize, map1);
            player3.Initialize(Content.Load<Texture2D>("sprite"), player3Pos, screenSize, map1);
            player4.Initialize(Content.Load<Texture2D>("sprite"), player4Pos, screenSize, map1);

            map1.Initialize(Content.Load<Texture2D>("map1Color"), Content.Load<Texture2D>("map1"), 16);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            player1.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
            player2.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.Two));
            player3.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.Three));
            player4.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.Four));

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
