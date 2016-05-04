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
            bombSpear,
            backupSpear,
            unblinker,
            spearCatch,
            shield,
            none
        }
    }
    public class Powerup
    {
        public PowerupEnum.powerUpEnum type { get; }
        public Rectangle hitbox { get; }
        public Boolean visible { get; set; }
        public float spawnTime { get; set; }
        public int timer { get; set; }
        private static Random random = new Random();

        public Powerup(Rectangle r)
        {
            Array types = Enum.GetValues(typeof(PowerupEnum.powerUpEnum));
            type = (PowerupEnum.powerUpEnum)types.GetValue(random.Next(3));// types.Length-1));
            hitbox = r;
            visible = false;
            timer = random.Next(2, 7);
        }
    }
}
