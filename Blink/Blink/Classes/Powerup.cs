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
        Array types = Enum.GetValues(typeof(PowerupEnum.powerUpEnum));
        Random random = new Random();
        public powerUpEnum getRandomType()
        {
            //types.length-1 because we don't want to randomly generate the powerup to have type of 'none'
            return (powerUpEnum)types.GetValue(random.Next(types.Length - 1));
        }
        public int getTimer()
        {
            return random.Next(2, 7);
        }
    }
    public class Powerup
    {
        public PowerupEnum.powerUpEnum type { get; }
        public Rectangle hitbox { get; }
        public Boolean visible { get; set; }
        public float spawnTime { get; set; }
        public int timer { get; set; }
        public Powerup(Rectangle r)
        {
            type = new PowerupEnum().getRandomType();
            hitbox = r;
            visible = false;
            timer = new PowerupEnum().getTimer();
        }
    }
}
