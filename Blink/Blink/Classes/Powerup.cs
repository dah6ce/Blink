using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blink.Classes
{
    public enum PowerupEnum
    {
        bombSpear,
        backupSpear,
        unblinker,
        shield,
        spearCatch,
        none
    }
    public class Powerup
    {
        public PowerupEnum type { get; }
        public Rectangle hitbox { get; }
        public Boolean visible { get; set; }
        public float spawnTime { get; set; }
        public int timer { get; set; }

        private static Array types = Enum.GetValues(typeof(PowerupEnum));
        private static Random random = new Random();

        public Powerup(Rectangle r)
        {
            type = (PowerupEnum)types.GetValue(random.Next(types.Length - 1));
            hitbox = r;
            visible = false;
            timer = random.Next(2, 7);
        }
    }
}
