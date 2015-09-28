using System;
using Blink.GUI;

namespace Blink
{
    public class StateQuit : GameState
    {
        public StateQuit()
        {
            
        }
        public void Initialize()
        {
            Game1.running = false;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            
        }

        public void UnloadContent()
        {
            
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            
        }

        public GameState GetTransition()
        {
            return null;
        }
    }
}

