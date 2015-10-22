using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blink.Classes
{
    class DeathEventArgs : EventArgs
    {
        public PlayerClass Killed { get; private set; }
        public PlayerClass Killer { get; private set; }
        public string Method { get; private set; }

        public DeathEventArgs(PlayerClass killed, PlayerClass killer, String method)
        {
            Killed = killed;
            Killer = killer;
            Method = Method;
        }
    }
}
