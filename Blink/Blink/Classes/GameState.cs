using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Blink.GUI
{
	public interface GameState
	{

		void Initialize();

		void LoadContent(ContentManager Content);

		void UnloadContent();

		void Update(GameTime gameTime);

		void Draw(SpriteBatch sb);

		/** 
		 * Returns null for no transition, 
		 * returns a new GameState object to trigger a transition
		 */
		GameState GetTransition();

	}
}

