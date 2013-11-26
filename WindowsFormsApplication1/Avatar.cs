using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace idleQuest
{
    public class Avatar         //holds all attributes of the player character
    {
        public int hp;
        public int maxhp;
        public int mp;
        public int maxmp;
        public int att;
        public int magic;
        public int defense;
        public int level;
        public int exp;
        public int gold;
        public int invCount;

        public const int MAX_INV = 30;
        const float LEVEL_UP_EXP_FACTOR = 6;    //factor * player level = amount of exp to level up
        const int PLAYER_MISS_PROB = 10;        //the chance of missing an enemy on any attack is 1/MISS_PROB

        Random rand1;
        Random rand;

        public Avatar()         //create new character
        {
            initAttribs();
            rand1 = new Random();
            rand = new Random(rand1.Next());    //double-seed for better randomness
        }

        public void initAttribs()       //initialize attributes to starting values
        {
            hp = 8;
            maxhp = 8;
            mp = 3;
            maxmp = 3;
            att = 1;
            magic = 1;
            defense = 0;
            level = 1;
            exp = 0;
            gold = 0;
            invCount = 0;
            System.Console.Out.WriteLine("Player attributes intialized.");
        }

        public void heal()      //restore hp/mp
        {
            hp = maxhp;
            mp = maxmp;
        }

        public int attack()     //attack an enemy. Returns the base damage done by the attack.
        {
            int damage;
            if (rand.Next(0, PLAYER_MISS_PROB) == PLAYER_MISS_PROB / 2)
            {
                damage = 0;
            }
            else
            {
                damage = rand.Next(1, (int)Math.Ceiling(att / 2.5));
            }
            return damage;
        }

        public void levelUp()   //gain a level
        {
            exp -= (int)Math.Ceiling(level * LEVEL_UP_EXP_FACTOR);
            ++level;
            maxhp += (int)Math.Ceiling(level / 2.5);
            maxmp += (int)Math.Ceiling(level / 4.0);
            att += (int)Math.Ceiling(level / 20.0);
            defense += (int)Math.Ceiling(level / 20.0);
            magic += (int)Math.Ceiling(level / 25.0);
            heal();
        }

        public int expToLevel()   //returns the (absolute) amount of exp needed to reach the next level
        {
            return (int)Math.Ceiling(level * LEVEL_UP_EXP_FACTOR);
        }

        public void saveAvatar(BinaryWriter gameSave)   //save player
        {
            gameSave.Write(hp);
            gameSave.Write(maxhp);
            gameSave.Write(mp);
            gameSave.Write(maxmp);
            gameSave.Write(att);
            gameSave.Write(magic);
            gameSave.Write(defense);
            gameSave.Write(level);
            gameSave.Write(exp);
            gameSave.Write(gold);
            gameSave.Write(invCount);
        }

        public void loadAvatar(BinaryReader gameLoad)   //load player
        {
            hp = gameLoad.ReadInt32();
            maxhp = gameLoad.ReadInt32();
            mp = gameLoad.ReadInt32();
            maxmp = gameLoad.ReadInt32();
            att = gameLoad.ReadInt32();
            magic = gameLoad.ReadInt32();
            defense = gameLoad.ReadInt32();
            level = gameLoad.ReadInt32();
            exp = gameLoad.ReadInt32();
            gold = gameLoad.ReadInt32();
            invCount = gameLoad.ReadInt32();
        }
    }
}
