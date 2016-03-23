using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blink.Classes
{
    public class PowerupEnum
    {
        public enum powerUpEnum
        {
            spearCatch,
            shield,
            bombSpear,
            backupSpear,
            unblinker,
            none
        }
    }
    public class Powerup
    {
        public PowerupEnum.powerUpEnum type { get; }
        public Rectangle hitbox { get; }
        public Powerup(PowerupEnum.powerUpEnum t, Rectangle r)
        {
            type = t;
            hitbox = r;
        }
    }
}
