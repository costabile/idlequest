using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace idleQuest
{
    public class Monster
    {
        string name;
        int level;
        int hp;
        Random rand1;
        Random rand;

        const int ENEMY_MISS_PROB = 10;         //the chance of an attack missing the player on any attack is 1/MISS_PROB

        public Monster(string mName, int mLevel)
        {
            name = mName;
            level = mLevel;
            rand1 = new Random();
            rand = new Random(rand1.Next());    //double-seed for better randomness
            hp = rand.Next(level, level * 2);
        }

        public int maxDamage()      //the maximum amount of damage a monster can do with a successful attack
        {
            int maxDamage = (int)Math.Floor(level * 1.2);
            return maxDamage < 1 ? 1 : maxDamage;
        }

        public int minDamage()      //the minimum amount of damage a monster can do with a successful attack
        {
            int minDamage = (maxDamage() - level * 2) % level;
            return minDamage < 1 ? 1 : minDamage;
        }

        public string getName()
        {
            return name;
        }

        public int strength(bool isExpGain = false)         //rating of monster toughness. Used for attacking and for determining exp earned by defeating it.
        {
            int str;
            if (!isExpGain && rand.Next(0, ENEMY_MISS_PROB) == ENEMY_MISS_PROB / 2)   //attacks can miss, but exp gain should always be >= 1
            {
                str = 0;
            }
            else
            {
                str = rand.Next(minDamage(), maxDamage() + 1);
            }
            return str;
        }

        public bool takeDamage(int damage)      //get hit by a player's attack. Returns true if the attack killed the monster.
        {
            hp -= damage;
            if (hp <= 0) return true;
            else return false;
        }
    }
}
