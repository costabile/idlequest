using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.IO;

namespace idleQuest
{
    public class Quest      //controls the gameplay
    {
        string saveFilePath = "idlechar.iqst";
        enum gameMode { Combat=0, Treasure, HealEvent, Loot, Free, Treasure2, Shop, ShopSell, Dead, Dead2, HealEvent2, Loot2, EnemyAttack, PlayerAttack, CombatWin, LevelUp};
        const int FREE_ACTIONS = 3;             //when 'free', game will randomly choose a new mode from the first FREE_ACTIONS gameModes.
        const int TICK_TIME = 750;              //time between each tick/turn in ms
        const int LOOT_ITEM_PROB = 500;         //probability (out of 1000) of getting one more item from looting a monster
        const int LOOT_ITEM_MAX = 2;            //the maximum amount of items you can loot off of one monster
        const int FIRST_ATTACK_PROB = 3;        //the chance of getting the first attack on an enemy is 1/FIRST_ATTACK_PROB
        const int MAX_MAINTXT_LINES = 100;      //the maximum number of lines to have in the textbox at once. Adding more lines will cause the oldest line to be removed.

        //probabilities of specific "free actions" out of 1000 (must be between 0 and 1000)
        const int TREASURE_PROB = 120;  //probability of finding treasure
        const int HEAL_PROB = 80;       //probability of getting a free heal
        //probability of combat is assumed to be = 1000 - <sum of other probabilities>

        RichTextBox rtb;             //rich text box to publish output to
        Avatar pc;                   //holds player attributes
        Inventory inv;               //items
        RandomGenerator generator;   //generates monsters, items, some phrases.
        gameMode mode = gameMode.Free;
        Random rand;                 //to randomize the seed of other Randoms
        Random modeGen;              //to pick which mode to enter next
        Random rng;                  //general-purpose RNG (for loot, gold, etc.)
        int actionNum;               //used to pick next action
        int gold;                    //gold found or lost
        int lootedItems;             //counter for the amount of items looted off of one monster
        int damage;                  //damage done by player or enemy in combat
        int expGained;               //experience earned
        Monster enemy;               //monster that the player is currently fighting
        Item item;                   //item that is being sold or acquired
        bool isFirstStart;

        public Quest(RichTextBox tb)
        {
            this.rtb = tb;
            rand = new Random();
            modeGen = new Random(rand.Next());
            rng = new Random(rand.Next());
            pc = new Avatar();
            inv = new Inventory(ref pc);
            generator = new RandomGenerator(rand.Next(), ref pc);
        }

        public void saveQuest()     //saves relevant game info to file
        {
            FileStream stream;
            BinaryWriter gameSave;
            try
            {
                stream = new FileStream(saveFilePath, FileMode.Create);
                gameSave = new BinaryWriter(stream);
                pc.saveAvatar(gameSave);
                inv.saveInventory(gameSave);
                gameSave.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("SAVE FAILED: " + e.ToString());
            }
        }

        public void loadQuest()         //loads relevant game info from file
        {
            System.Console.Out.WriteLine("Checking for existing save file...");
            FileStream stream;
            BinaryReader gameLoad;
            if (File.Exists(saveFilePath)) {
                try
                {
                    System.Console.Out.WriteLine("Save file found. Loading...");
                    stream = new FileStream(saveFilePath, FileMode.Open);
                    gameLoad = new BinaryReader(stream);
                    try
                    {
                        pc.loadAvatar(gameLoad);
                        inv.loadInventory(gameLoad);
                    }
                    catch
                    {
                        gameLoad.Close();
                        throw;
                    }
                    gameLoad.Close();
                    isFirstStart = false;
                }
                catch (Exception)     //save file invalid
                {
                    System.Console.Out.WriteLine("Save file invalid. Deleting...");
                    File.Delete(saveFilePath);
                    System.Console.Out.WriteLine("Creating new save file...");
                    File.Create(saveFilePath);
                    pc.initAttribs();              //avatar/inv might have been affected by invalid save file; re-initialize
                    inv = new Inventory(ref pc);
                    isFirstStart = true;
                }
            }
            else {      //first time starting IdleQuest
                isFirstStart = true;
                try
                {
                    System.Console.Out.WriteLine("No save file found. Creating new save file...");
                    File.Create(saveFilePath);   //create blank save file
                }
                catch (Exception e)
                {
                    MessageBox.Show("SAVE FILE CREATION FAILED: " + e.ToString());
                }
            }
        }

        public void nextTurn()      //make getGoldAmount method for treasure and loot?
        {
            if (pc.invCount == Avatar.MAX_INV && mode != gameMode.Shop && mode != gameMode.ShopSell) mode = gameMode.Shop;
            else if (pc.hp <= 0 && mode != gameMode.Dead && mode != gameMode.Dead2)
            {
                pc.hp = 0;
                mode = gameMode.Dead;
            }
            switch (mode)
            {
                case gameMode.Free:
                    printEvent("You depart into the wilderness, ready for the next encounter.", Color.Gray);       //TODO: randomly pick among a few phrases?
                    actionNum = modeGen.Next(1, 1001);      //pick new action
                    if (actionNum <= TREASURE_PROB) mode = gameMode.Treasure;
                    else if ((actionNum > TREASURE_PROB) && (actionNum <= TREASURE_PROB + HEAL_PROB)) mode = gameMode.HealEvent;
                    else mode = gameMode.Combat;
                    break;

                //"free actions" i.e. events that can occur after being in Free mode

                case gameMode.Combat:
                    enemy = generator.generateMonster();
                    printEvent("A(n) " + enemy.getName() + " leaps out of the shrubbery!", Color.Tomato);  //TODO: random phrase?
                    if (rng.Next(1, FIRST_ATTACK_PROB) == 1) mode = gameMode.PlayerAttack;
                    else mode = gameMode.EnemyAttack;
                    break;

                case gameMode.Treasure:
                    printEvent("You stumble across an old, battered treasure chest.", Color.Goldenrod);                   //TODO: random phrase
                    mode = gameMode.Treasure2;
                    break;

                case gameMode.HealEvent:
                    printEvent("Wandering through the wilderness, you happen upon a fountain filled with clear, sparkling water.  You quench your thirst.", Color.DeepSkyBlue);      //TODO: random phrase
                    mode = gameMode.HealEvent2;
                    break;

                //---end free actions (remaining actions can only occur as a result of a previous action or condition)

                case gameMode.LevelUp:
                    printEvent("You have learned much in your travels. You gain a level!", Color.Green);      //TODO: random phrase?
                    pc.levelUp();
                    mode = gameMode.Loot;   //player must have just defeated an enemy; still needs to loot it
                    break;

                case gameMode.PlayerAttack:
                    //TODO: use magic
                    if (inv.getEquippedItem(itemType.Weapon).getAttrValue() == 0) printEvent("You savagely swing your fists at the enemy!", Color.Navy);       //TODO: random phrase?
                    else printEvent("You take a swing at the enemy with your " + inv.getEquippedItem(itemType.Weapon).getName() + "!", Color.Navy);      //TODO: random phrase?
                    damage = pc.attack();
                    if (damage == 0)
                    {
                        printEvent("Your attack misses its mark!", Color.PaleTurquoise);      //TODO: random phrase?
                    }
                    else
                    {
                        printEvent("Your attack deals " + damage + " damage to the " + enemy.getName() + ".", Color.Navy);     //TODO: random phrase?
                    }
                    if (enemy.takeDamage(damage)) mode = gameMode.CombatWin;
                    else mode = gameMode.EnemyAttack;
                    break;

                case gameMode.EnemyAttack:
                    printEvent("The " + enemy.getName() + " attacks!", Color.Red);      //TODO: random phrase?
                    damage = enemy.strength();
                    if (damage == 0)
                    {
                        printEvent("Its attack whistles harmlessly by!", Color.Red);      //TODO: random phrase?
                    }
                    else
                    {
                        damage -= rand.Next(0, pc.defense / 8);
                        if (damage < enemy.minDamage()) damage = enemy.minDamage();
                        if (damage > pc.hp) damage = pc.hp;         //prevent HP from becoming negative
                        printEvent("The attack does " + damage + " damage!", Color.Red);     //TODO: random phrase?
                        pc.hp -= damage;
                    }
                    mode = gameMode.PlayerAttack;
                    break;

                case gameMode.CombatWin:
                    printEvent("The corpse of the " + enemy.getName() + " crumples at your feet.", Color.SeaGreen);
                    expGained = enemy.strength(true);
                    printEvent("You gain " + expGained + " experience.", Color.SeaGreen);
                    pc.exp += expGained;
                    if (pc.exp >= pc.expToLevel())
                    {
                        mode = gameMode.LevelUp;
                    }
                    else
                    {
                        mode = gameMode.Loot;
                    }
                    break;

                case gameMode.Treasure2:
                    gold = rng.Next(10 + pc.level, (pc.level + 10) * 3);     //more gold at higher levels
                    printEvent("Opening the chest reveals " + gold + " gold pieces!", Color.Goldenrod);
                    pc.gold += gold;
                    mode = gameMode.Free;
                    break;

                case gameMode.HealEvent2:
                    printEvent("You are fully healed!", Color.DeepSkyBlue);
                    pc.heal();
                    mode = gameMode.Free;
                    break;

                case gameMode.Loot:     //finding gold/items after combat
                    //loot gold before items (you always get gold, but don't always get items)
                    gold = rng.Next(pc.level + 1, (pc.level + 8) * 2);      //gold from monsters tends to be less than from treasure chests
                    printEvent("Searching the mangled corpse of your adversary, you find " + gold + " gold pieces!", Color.Goldenrod);       //TODO: random phrase (just change "mangled"?)
                    pc.gold += gold;
                    lootedItems = 0;
                    if (rng.Next(1, 1001) < LOOT_ITEM_PROB) mode = gameMode.Loot2;
                    else mode = gameMode.Free;
                    break;

                case gameMode.Loot2:    //looting items
                    item = generator.generateItem();
                    inv.addItem(item);
                    printEvent("You find a(n) " + item.getName() + ".", Color.Plum);
                    lootedItems++;
                    if (lootedItems < LOOT_ITEM_MAX && (rng.Next(1, 1001) < LOOT_ITEM_PROB)) mode = gameMode.Loot2;
                    else mode = gameMode.Free;
                    break;

                case gameMode.Dead:
                    printEvent("Your last breath escapes your lungs as your battered corpse crumples to the ground.", Color.DarkRed);        //TODO: random phrase
                    gold = rng.Next(pc.level + 10, (pc.level + 10) * 5);
                    if (gold > pc.gold) gold = pc.gold;
                    printEvent("You lose " + gold + " gold pieces!", Color.DarkRed);
                    pc.gold -= gold;
                    mode = gameMode.Dead2;
                    break;

                case gameMode.Dead2:
                    printEvent("Several hours later, you awaken in a dark room at an inn. Unphased by this mysterious intervention, you quickly proceed outside to continue your adventure.", Color.DarkRed);
                    pc.heal();
                    mode = gameMode.Free;
                    break;

                case gameMode.Shop:
                    printEvent("Your pack has become swollen with the spoils of battle. You make your way to the nearest town to lighten your burden.", Color.Olive);     //TODO: random phrase
                    //counts as resting in town.  Fully heal character.
                    pc.heal();
                    mode = gameMode.ShopSell;
                    break;

                case gameMode.ShopSell:
                    item = inv.popLastItem();
                    printEvent("You sell the " + item.getName() + " for " + item.getAttrValue() + " gold pieces.", Color.Goldenrod);
                    pc.gold += item.getAttrValue();
                    if (pc.invCount == 0) mode = gameMode.Free;
                    break;

                default:
                    break;
            }
        }

        public Avatar getPlayer()
        {
            return pc;
        }

        public Inventory getInventory()
        {
            return inv;
        }

        public void go()        //start running the "game"
        {
            if (isFirstStart)
            {
                this.addText(Color.Red, 2, "With the wind howling and a storm at your back, you embark upon the most important journey the world has ever seen.", true);
                this.addText(Color.Red, 2, "You begin...", false);
                this.addText(Color.Red, 1, "IdleQuest.", true);
            }
            else
            {
                this.addText(Color.Red, 2, "You wake from your rest and eagerly set out to continue your adventures. You resume...", false);
                this.addText(Color.Red, 1, "IdleQuest.", true);
            }
            while (true)
            {
                this.nextTurn();
                Thread.Sleep(TICK_TIME);
            }
        }

        public void printEvent(string text, Color c)      //write text with default format (and EOL)
        {
            addText(c, 0, text, true);
        }

        public void addText(Color c, int style, string text, bool addEOL)   //append text to rtb with specified properties.
        {
            //style: 0 = normal, 1 = bold, 2 = italic, 3 = underline, 4 = strikethrough
            Font oldf = this.rtb.Font;
            Font f;
            FontStyle fs;
            switch (style)
            {
                case 1:
                    fs = FontStyle.Bold;
                    break;
                case 2:
                    fs = FontStyle.Italic;
                    break;
                case 3:
                    fs = FontStyle.Underline;
                    break;
                case 4:
                    fs = FontStyle.Strikeout;
                    break;
                default:
                    fs = FontStyle.Regular;
                    break;
            }
            f = new Font(oldf.FontFamily, oldf.Size, fs);
            if (this.rtb.InvokeRequired)        //allows a thread to use the rtb control which was created in a different thread
            {
                this.rtb.Invoke(new MethodInvoker(delegate
                {
                    this.rtb.Select(this.rtb.TextLength, 0);    //move caret to end of text so we don't overwrite selected text
                    this.rtb.SelectionFont = f;
                    this.rtb.SelectionColor = c;
                    this.rtb.SelectedText = text;
                    if (addEOL) this.rtb.AppendText("\n");

                    if (addEOL && this.rtb.Lines.Length > MAX_MAINTXT_LINES)    //remove oldest line in the textbox if a new one was added after the limit has been reached 
                    {
                        this.rtb.Select(0, rtb.GetFirstCharIndexFromLine(1));   //select the first line
                        this.rtb.ReadOnly = false;                              //the rtb being read-only prevents replacement of text with empty string
                        this.rtb.SelectedText = String.Empty;
                        this.rtb.ReadOnly = true;
                        this.rtb.Select(this.rtb.TextLength, 0);                //move caret back to end
                    }
                }));
            }
        }
    }
}
