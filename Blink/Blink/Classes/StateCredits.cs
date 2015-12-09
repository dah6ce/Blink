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
	public class StateCredits : GameState
	{
		Vector2 screenSize;
		String title;
		String[] options;
		GameState menu;
		GameState nextState;
		bool prematureEnter;
		SpriteFont x;
		KeyboardState old;
		Texture2D bg;

		public StateCredits(Vector2 screenSize, String title, String[] options)
		{
			this.screenSize = screenSize;
			this.title = title;
			this.options = options;

		}
		public void getMenu(GameState menu)
		{
			this.menu = menu;
		}
		public void Draw(SpriteBatch sb)
		{
			sb.Draw(bg, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
		}

		public GameState GetTransition()
		{
			return nextState;
		}

		public void Initialize()
		{
			nextState = null;
			GamePadState padState = GamePad.GetState(PlayerIndex.One);
			KeyboardState keyState = Keyboard.GetState();
			if (keyState.IsKeyDown(Keys.Enter))
			{
				prematureEnter = true;
			}
		}

		public void LoadContent(ContentManager Content)
		{
			x = Content.Load<SpriteFont>("miramo");
			bg = Content.Load<Texture2D>("credits");
        }

		public void UnloadContent()
		{

		}

		public void Update(GameTime gameTime)
		{
			GamePadState padState = GamePad.GetState(PlayerIndex.One);
			KeyboardState keyState = Keyboard.GetState();
			if(prematureEnter && keyState.IsKeyUp(Keys.Enter))
			{
				prematureEnter = false;
			}
			if (!prematureEnter && (keyState.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.A)))
			{
				((StateSimpleMenu)menu).reset();
				nextState = menu;
			}
		}



	}
}