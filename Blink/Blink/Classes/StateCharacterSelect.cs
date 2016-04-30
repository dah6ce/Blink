using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace Blink.GUI
{
    enum PlayerKeys
    {
        Player1,
        Player2,
        Player3,
        Player4,
        allPlayers

    }

    public class StateCharacterSelect : GameState
    {
        const int THUMBROWSIZE = 2;

        Blink.Classes.PlayerClass player1;
        Blink.Classes.PlayerClass player2;
        Blink.Classes.PlayerClass player3;
        Blink.Classes.PlayerClass player4;
        internal Blink.Classes.PlayerClass[] players = new Blink.Classes.PlayerClass[4];
        bool[] playersInGame = { false, false, false, false };
        string[] playerTexts = { "", "", "", "" };
        int[] charNums = { -1, -1, -1, -1 };
        string default_charName;

        Vector2 screenSize;
        int[] selected = new int[4];
        IEnumerable<string> chars;

        String titleString;

        GameState nextState;
        GameState levelState;
        GameState game;

        bool[] left = { false, false, false, false };
        bool[] right = { false, false, false, false };
        bool[] up = { false, false, false, false };
        bool[] down = { false, false, false, false };
        bool[] locked = { false, false, false, false };
        bool[] back = { false, false, false, false };
        bool prematureEnter;
        bool tabPressed = false;
        bool showLoadScreen = false;
        int activePlayers = 0;
        int currentPlayer = 0;


        //Storage list for all our char names
        List<string> charNames = new List<string>();
        List<ImageButton> charThumbs = new List<ImageButton>();
        
        Texture2D background;
        Texture2D loadScreen;
        Texture2D[] playerSelect = new Texture2D[4];
        bool[] connected = { false, false, false, false };
        bool[] startButtons = { false, false, false, false };

        public StateCharacterSelect(Vector2 screenSize, String title, GameState l, GameState g)
        {
            this.levelState = l;
            this.game = g;
            this.screenSize = screenSize;
            this.titleString = title;
        }

        public void Initialize()
        {
            #if WINDOWS
            chars = Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Content\\CharData", "*.char");
            #elif LINUX
            chars = Directory.EnumerateFiles(Environment.CurrentDirectory + "/Content/CharData", "*.char");
            #endif
            showLoadScreen = false;

            foreach(var path in chars){
            default_charName = path.Remove(0, Environment.CurrentDirectory.Length + "\\Content\\CharData\\".Length);
            default_charName = default_charName.Replace(".char", "");
            }
            //for(int i = 0; i < 4; i++) { 
                //if (this.charThumbs.Count > 0)
                //    this.charThumbs[selected[i]].unselect();
                //this.selected[i] = 0;
            //}
            for (int i = 0; i < 4; i++)
            {
                if (connected[i])
                {
                    playerTexts[i] = "CharData/Sprites/" + charNames[selected[i]];
                }
            }
            player1 = new Blink.Classes.PlayerClass();
            players[0] = player1;
            player2 = new Blink.Classes.PlayerClass();
            players[1] = player2;
            player3 = new Blink.Classes.PlayerClass();
            players[2] = player3;
            player4 = new Blink.Classes.PlayerClass();
            players[3] = player4;
            player1.title = "P1";
            player2.title = "P2";
            player3.title = "P3";
            player4.title = "P4";
            this.nextState = null;
            AudioManager.TriggerCharacterSelect();
            KeyboardState keyState = Keyboard.GetState();
            //GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (keyState.IsKeyDown(Keys.Enter))
            {
                prematureEnter = true;
            }
        }

        public void LoadContent(ContentManager Content)
        {
            if (charNames.Count > 0)
            {
                for (int i = 0; i < 4; i++) { 
                    this.charThumbs[selected[i]].unlock();
                    //this.connected[i] = false;
                    //this.selected[i] = 0;
                    this.locked[i] = false;
                    back[i] = true;

                }

                return;
            }

            for(int i = 0; i < 4; i++)
            {
                playerSelect[i] = Content.Load<Texture2D>("MenuData/S" + (i + 1).ToString());
            }

            Vector2 negPos = new Vector2(0, 0);
            Vector2 offset = new Vector2(-4, -4);
            Texture2D bar = Content.Load<Texture2D>("bar");
            /*for (int i = 0; i < 4; i++)
            {
                if (players[i] != null && players[i].active)
                {
                    players[i].Initialize(Content.Load<Texture2D>(default_charName), negPos, screenSize, null, players, offset, bar);
                }
            }*/

            background = Content.Load<Texture2D>("MenuData/characterselect");
            loadScreen = Content.Load<Texture2D>("controller2");

            //Gets a list of all the .char files in our chardata folder, Platform specific paths
            #if WINDOWS
            chars = Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Content\\CharData", "*.char");
            #elif LINUX
            chars = Directory.EnumerateFiles(Environment.CurrentDirectory + "/Content/CharData", "*.char");
            #endif

            //For each char file, slice off the path to store just the char's name.
            foreach (string path in chars)
            {
                string charName = path.Remove(0, Environment.CurrentDirectory.Length + "\\Content\\CharData\\".Length);
                charName = charName.Replace(".char", "");
                charNames.Add(charName);
                Texture2D charThumbtext = Content.Load<Texture2D>("CharData/"+charName);
                ImageButton thumb = new ImageButton(charThumbtext, new Vector2(), charName, playerSelect);
                thumb.useMultiSelect();
                thumb.setSize(new Vector2(200, 200));
                charThumbs.Add(thumb);
            }

            positionThumbs(charThumbs);
        }

        public void UnloadContent()
        {

        }

        private void positionThumbs(List<ImageButton> thumbs)
        {
            for(int i = 0; i < thumbs.Count; i++)
            {
                ImageButton thumb = thumbs[i];
                thumb.setPosition(new Vector2(200 * (i % THUMBROWSIZE) + 601, (i / THUMBROWSIZE) * 200 + 278));
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            KeyboardState nullKey = new KeyboardState();
            GamePadState padState_1 = GamePad.GetState(PlayerIndex.One);
            GamePadState padState_2 = GamePad.GetState(PlayerIndex.Two);
            GamePadState padState_3 = GamePad.GetState(PlayerIndex.Three);
            GamePadState padState_4 = GamePad.GetState(PlayerIndex.Four);

            updateSelection(padState_1, keyState, 0);
            updateSelection(padState_2, keyState, 1);
            updateSelection(padState_3, keyState, 2);
            updateSelection(padState_4, keyState, 3);

            /*var player1State = Keyboard.GetState();
            var player2State = Keyboard.GetState();
            var player3State = Keyboard.GetState();
            var player4State = Keyboard.GetState();

            player1.Update(player1State, GamePad.GetState(PlayerIndex.One), gameTime);
            player2.Update(player2State, GamePad.GetState(PlayerIndex.Two), gameTime);
            player3.Update(player3State, GamePad.GetState(PlayerIndex.Three), gameTime);
            player4.Update(player4State, GamePad.GetState(PlayerIndex.Four), gameTime);*/

            if (keyState.IsKeyUp(Keys.Enter) && padState_1.IsButtonUp(Buttons.Start))
            {
                prematureEnter = false;
            }

            if (keyState.IsKeyDown(Keys.Tab) && !tabPressed)
            {
                tabPressed = true;
                currentPlayer = (currentPlayer + 1) % 4;
            }
            else if (keyState.IsKeyUp(Keys.Tab))
            {
                tabPressed = false;
            }
        }

        private void updateSelection(GamePadState pad, KeyboardState keys, int player)
        {
            if (currentPlayer != player)
            {
                keys = new KeyboardState();
            }
            //Start button functions
            if ((pad.IsButtonDown(Buttons.Start) || keys.IsKeyDown(Keys.Enter)) && !startButtons[player])
            {
                if (showLoadScreen)
                {
                    startMatch();
                }
                //Player X has entered the game!
                else if (!this.connected[player])
                {
                    this.connected[player] = true;
                    charThumbs[0].hover(player);
                    selected[player] = 0;
                }
                else
                {
                    //Is everybody readied up? Then we can gooooo!
                    bool readyToGo = true;
                    int connectedPlayers = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (connected[i])
                            connectedPlayers++;
                        if ((!locked[i] && connected[i]))
                            readyToGo = false;
                    }

                    if (readyToGo && connectedPlayers > 0)
                    {
                        showLoadScreen = true;
                    }

                    //Character X selected
                    if (!charThumbs[selected[player]].isSelected())
                    { 
                        locked[player] = true;
                        charThumbs[selected[player]].select(player);
                    }
                }
                startButtons[player] = true;
            }

            else if (pad.IsButtonUp(Buttons.Start) && keys.IsKeyUp(Keys.Enter))
                startButtons[player] = false;

            if (!connected[player])
                return;

            //B button functions
            if ((pad.IsButtonDown(Buttons.B) || keys.IsKeyDown(Keys.Back)) && !back[player])
            {
                //Unlock character selection
                if (locked[player])
                {
                    locked[player] = false;
                    charThumbs[selected[player]].unselect(player);
                }
                //Disconnect player
                else if (connected[player])
                {
                    connected[player] = false;
                    charThumbs[selected[player]].unhover(player);
                }
                back[player] = true;
                
            }
            else if (pad.IsButtonUp(Buttons.B) && keys.IsKeyUp(Keys.Back))
                back[player] = false;

            if (locked[player])
            {
                //control player
            }
            else 
            {
                //Right button functions
                if ((pad.IsButtonDown(Buttons.LeftThumbstickRight) || pad.IsButtonDown(Buttons.DPadRight) ||
                    keys.IsKeyDown(Keys.Right)) && !right[player])
                {
                    charThumbs[selected[player]].unhover(player);
                    selected[player]++;
                    if(selected[player] % THUMBROWSIZE == 0)
                    {
                        selected[player] -= THUMBROWSIZE;
                    }
                    charThumbs[selected[player]].hover(player);
                    right[player] = true;
                }
                else if (pad.IsButtonUp(Buttons.LeftThumbstickRight) && pad.IsButtonUp(Buttons.DPadRight) &&
                    keys.IsKeyUp(Keys.Right))
                    right[player] = false;

                //Left button functions
                if ((pad.IsButtonDown(Buttons.LeftThumbstickLeft) || pad.IsButtonDown(Buttons.DPadLeft) ||
                    keys.IsKeyDown(Keys.Left)) && !left[player])
                {
                    charThumbs[selected[player]].unhover(player);
                    selected[player]--;
                    if (selected[player] % THUMBROWSIZE == 1 || selected[player] < 0)
                    {
                        selected[player] += THUMBROWSIZE;
                    }
                    charThumbs[selected[player]].hover(player);
                    left[player] = true;
                }
                else if (pad.IsButtonUp(Buttons.LeftThumbstickLeft) && pad.IsButtonUp(Buttons.DPadLeft) &&
                    keys.IsKeyUp(Keys.Left))
                    left[player] = false;

                //Up button functions
                if ((pad.IsButtonDown(Buttons.LeftThumbstickUp) || pad.IsButtonDown(Buttons.DPadUp) ||
                    keys.IsKeyDown(Keys.Up)) && !up[player])
                {
                    charThumbs[selected[player]].unhover(player);
                    selected[player] -= THUMBROWSIZE;
                    if (selected[player] < 0)
                    {
                        selected[player] += charNames.Count;//(int)Math.Ceiling((double)(charNames.Count/THUMBROWSIZE));
                        if (selected[player] > charNames.Count)
                            selected[player] = charNames.Count - 1;
                    }
                    charThumbs[selected[player]].hover(player);
                    up[player] = true;
                }
                else if (pad.IsButtonUp(Buttons.LeftThumbstickUp) && pad.IsButtonUp(Buttons.DPadUp) &&
                    keys.IsKeyUp(Keys.Up))
                    up[player] = false;

                //Down button functions
                if ((pad.IsButtonDown(Buttons.LeftThumbstickDown) || pad.IsButtonDown(Buttons.DPadDown) ||
                    keys.IsKeyDown(Keys.Down)) && !down[player])
                {
                    charThumbs[selected[player]].unhover(player);
                    selected[player] += THUMBROWSIZE;
                    if (selected[player] >= charNames.Count)
                    {
                        selected[player] %= charNames.Count;
                    }
                    charThumbs[selected[player]].hover(player);
                    down[player] = true;
                }
                else if (pad.IsButtonUp(Buttons.LeftThumbstickDown) && pad.IsButtonUp(Buttons.DPadDown) &&
                    keys.IsKeyUp(Keys.Down))
                    down[player] = false;
            }
            /*else
            {
                //INSERT PLAY GAME CONTROLS HERE
            }*/
        }

        public void startMatch()
        {
            string[] playerTexts = new string[4];
            for(int i = 0; i < 4; i++)
            {
                if (connected[i])
                {
                    playerTexts[i] = "CharData/Sprites/"+charNames[selected[i]];
                }
            }
            ((StateGame)game).informGame(connected, playerTexts, selected);
            nextState = levelState;
        }

        public void Draw(SpriteBatch sb)
        {
            //title.Draw(sb);

            sb.Draw(background, new Vector2(0,0), Color.White);
            foreach (ImageButton thumb in charThumbs)
                thumb.Draw(sb);

            if (showLoadScreen)
            {
                Rectangle r = new Rectangle((int)(screenSize.X / 2) - (int)(loadScreen.Width / 4),
                    (int)(screenSize.Y / 2) - (int)(loadScreen.Height / 4), 
                    (int)(loadScreen.Width / 2), (int)(loadScreen.Height / 2));
                sb.Draw(loadScreen, r, Color.White);
            }
            //player1.Draw(sb);
            //player2.Draw(sb);
            //player3.Draw(sb);
            //player4.Draw(sb);
            //sb.Draw(selectedOverlay, new Vector2(200 * (selected % THUMBROWSIZE), (float)Math.Floor((selected / 8f)) * 120 + 600), Color.Gold);
        }

        public GameState GetTransition()
        {
            return nextState;
        }


    }
}

