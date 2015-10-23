using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blink.Classes
{
    class Animation
    {

        Texture2D sheet;
        float frameLength;
        int frameNum, loopNum, killAfter;
        Boolean loop;
        Vector2 pos;

        private float frameTimer;
        private int onFrame;
        Rectangle frame;
        List<Animation> aniList;
        

        int directionFacing; //0 for left, 1 for right

        public Animation(Texture2D s, Vector2 p, float fLen, int fNum, List<Animation> aList, int facing)
        {
            sheet = s;
            frameLength = fLen;
            frameNum = fNum;
            pos = p;
            directionFacing = facing;

            frameTimer = 0;
            onFrame = 0;
            frame = new Rectangle(0, 0, (int)(s.Width / fNum), s.Height);

            aniList = aList;
            aniList.Add(this);
            
        }

        public void Update(GameTime gameTime)
        {
            frameTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if(frameTimer > frameLength)
            {
                frameTimer -= frameLength;
                onFrame++;
                if(onFrame > frameNum)
                {
                    aniList.Remove(this);
                }
                else
                {
                    frame.X = onFrame * (int)(sheet.Width/frameNum);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (directionFacing == 1)
                sb.Draw(sheet, pos, frame, Color.White);
            else
                sb.Draw(sheet, pos, frame, Color.White, 0f, new Vector2(), 1f, SpriteEffects.FlipHorizontally,0f);

        }
    }
}
