using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Blink
{
    public class AudioManager
    {
        public const double FADE_TIME = 1.0f;

        static SoundEffectInstance menuLayer1;
        static SoundEffectInstance menuLayer2;
        static SoundEffectInstance battleMusic;

        enum State {
            enterFade,
            menu1,
            menuFade,
            menu2,
            battleFade,
            battle,
            exitFade
        }

        static State state;

        static double timer;

        public static void LoadContent(ContentManager Content) {
            menuLayer1 = Content.Load<SoundEffect>("menu1").CreateInstance();
            menuLayer2 = Content.Load<SoundEffect>("menu2").CreateInstance();
            battleMusic = Content.Load<SoundEffect>("crowd").CreateInstance();
        }

        public static void Initialize() {
            menuLayer1.IsLooped = true;
            menuLayer2.IsLooped = true;
            battleMusic.IsLooped = true;

            menuLayer1.Volume = 0.0f;
            menuLayer2.Volume = 0.0f;
            battleMusic.Volume = 0.0f;

            timer = FADE_TIME;
            state = State.enterFade;

            menuLayer1.Play();
            menuLayer2.Play();
        }

        public static void Update(GameTime t) {
            if (timer > 0)
                timer -= t.ElapsedGameTime.TotalSeconds;
            if (timer < 0)
                timer = 0;

            switch (state)
            {
                case State.enterFade:
                    menuLayer1.Volume = (float)(1.0 - timer / FADE_TIME);
                    if (timer == 0)
                        state = State.menu1;
                    break;
                case State.menu1:
                    break;
                case State.menuFade:
                    menuLayer2.Volume = (float)(1.0 - timer / FADE_TIME);
                    if (timer == 0)
                        state = State.menu2;
                    break;
                case State.menu2:
                    break;
                case State.battleFade:
                    menuLayer1.Volume = (float)(timer / FADE_TIME);
                    menuLayer2.Volume = (float)(timer / FADE_TIME);
                    battleMusic.Volume = (float)(1.0 - timer / FADE_TIME);
                    if (timer == 0)
                    {
                        state = State.battle;
                        menuLayer1.Stop();
                        menuLayer2.Stop();
                    }
                    break;
                case State.battle:
                    break;
                case State.exitFade:
                    menuLayer1.Volume = (float)(1.0 - timer / FADE_TIME);
                    battleMusic.Volume = (float)(timer / FADE_TIME);
                    if (timer == 0)
                    {
                        state = State.menu1;
                        battleMusic.Stop();
                    }
                    break;
            }
        }

        public static void TriggerCharacterSelect() {
            if (state != State.menu1)
                return;
            state = State.menuFade;
            timer = FADE_TIME;
        }

        public static void TriggerBattle() {
            if (state != State.menu2)
                return;
            
            state = State.battleFade;
            timer = FADE_TIME;
            battleMusic.Volume = 0.0f;
            battleMusic.Play();
        }

        public static void TriggerExit() {
            if (state != State.battle)
                return;

            state = State.exitFade;
            timer = FADE_TIME;
            menuLayer1.Volume = 0.0f;
            menuLayer1.Play();
            menuLayer2.Play();
        }
            
    }
}

