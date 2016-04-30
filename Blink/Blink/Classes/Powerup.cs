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
        public Boolean visible { get; set; }
        public float spawnTime { get; set; }
        private int timer;
        public Powerup(Rectangle r)
        {
            Array types = Enum.GetValues(typeof(PowerupEnum.powerUpEnum));
            Random random = new Random();
            type = (PowerupEnum.powerUpEnum)types.GetValue(random.Next(types.Length));
            hitbox = r;
            visible = false;
            timer = random.Next(2, 7);
        }
    }
}
