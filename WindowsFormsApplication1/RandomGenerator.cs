using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace idleQuest
{
    public class RandomGenerator
    {
        Random rand1;
        Random rand;
        Avatar pc; //need this to get player's current level (to determine strength of items and monsters to generate)
        string[] monsterNames; //a list of possible monster type names
        string[] monsterDescriptors; //a list of possible monster descriptors
        //item type names (each item type has a different possible set of names):
        string[] weaponNames;
        string[] offhandNames;
        string[] torsoNames;
        string[] headNames;
        string[] handsNames;
        string[] feetNames;
        string[] fingerNames;
        string[] backNames;
        string[] neckNames;
        string[] trashNames;

        //item descriptors (i.e. <prefix> <weapon> of <suffix>)
        string[] itemPrefixDesc;
        string[] itemSuffixDesc;

        char[] delimiterChars = { ',', '.', ':', '\t', '\n' };

        public RandomGenerator(int seed, ref Avatar player)
        {
            rand1 = new Random(seed);
            rand = new Random(rand1.Next());    //randomized seed for more random results
            pc = player;

            string tempStr;
            string path = "words/";

            //read in all the word lists
            string fileName = "";
            string[] filesToRead = { "monsternames.txt", "monsterdescriptors.txt", "weaponnames.txt",
                                     "offhandnames.txt", "torsonames.txt", "headnames.txt", "handsnames.txt",
                                     "feetnames.txt", "fingernames.txt", "backnames.txt", "necknames.txt",
                                     "trashnames.txt", "itemprefixdesc.txt", "itemsuffixdesc.txt"};
            string[][] wordLists = new string[filesToRead.Length][];
            StreamReader rdr;
            try
            {
                for (int i = 0; i < filesToRead.Length; i++)
                {
                    fileName = filesToRead[i];
                    rdr = new StreamReader(path + fileName);
                    tempStr = rdr.ReadToEnd();
                    rdr.Close();
                    wordLists[i] = tempStr.Split(delimiterChars);
                }

                monsterNames = wordLists[0];
                monsterDescriptors = wordLists[1];
                weaponNames = wordLists[2];
                offhandNames = wordLists[3];
                torsoNames = wordLists[4];
                headNames = wordLists[5];
                handsNames = wordLists[6];
                feetNames = wordLists[7];
                fingerNames = wordLists[8];
                backNames = wordLists[9];
                neckNames = wordLists[10];
                trashNames = wordLists[11];
                itemPrefixDesc = wordLists[12];
                itemSuffixDesc = wordLists[13];
            }
            catch (IOException)       //file or directory was not found
            {
                string msg = "Couldn't find word list: " + fileName + ". Make sure that the \"" + path + "\" folder exists within the same folder as the IdleQuest executable and that it contains all of the original word list files, then try running IdleQuest again.";
                System.Windows.Forms.MessageBox.Show(msg, "IdleQuest -- Word List Not Found");
                Environment.Exit(0);
            }
        }

        public Item generateItem()                      //generate random item with random type
        {
            Type enumType = Type.GetType("idleQuest.itemType");
            int numElementsInEnum = Enum.GetValues(enumType).Length;
            int iT = rand.Next(0, numElementsInEnum); //find the number of possible item types and pick a random type
            return generateItem((itemType)iT);
        }

        public Item generateItem(itemType iT)           //generate random item of a given type
        {
            int minValue = pc.level / 2 - 30;   //minimum value that an item might have
            int maxValue = pc.level / 2 + 10;   //max value that an item might have
            if (minValue < 1) minValue = 1;     //non-placeholder items should have at least 1 value

            int value = rand.Next(minValue, maxValue + 1); //pick a value for the item

            int nameNum;
            string itemNameType;

            switch (iT)
            {
                case itemType.Weapon:
                    nameNum = rand.Next(0, weaponNames.Length);
                    itemNameType = weaponNames[nameNum];
                    break;
                case itemType.Offhand:
                    nameNum = rand.Next(0, offhandNames.Length);
                    itemNameType = offhandNames[nameNum];
                    break;
                case itemType.Torso:
                    nameNum = rand.Next(0, torsoNames.Length);
                    itemNameType = torsoNames[nameNum];
                    break;
                case itemType.Head:
                    nameNum = rand.Next(0, headNames.Length);
                    itemNameType = headNames[nameNum];
                    break;
                case itemType.Hands:
                    nameNum = rand.Next(0, handsNames.Length);
                    itemNameType = handsNames[nameNum];
                    break;
                case itemType.Feet:
                    nameNum = rand.Next(0, feetNames.Length);
                    itemNameType = feetNames[nameNum];
                    break;
                case itemType.Finger:
                    nameNum = rand.Next(0, fingerNames.Length);
                    itemNameType = fingerNames[nameNum];
                    break;
                case itemType.Back:
                    nameNum = rand.Next(0, backNames.Length);
                    itemNameType = backNames[nameNum];
                    break;
                case itemType.Neck:
                    nameNum = rand.Next(0, neckNames.Length);
                    itemNameType = neckNames[nameNum];
                    break;
                case itemType.Trash:
                    nameNum = rand.Next(0, trashNames.Length);
                    itemNameType = trashNames[nameNum];
                    break;
                default:
                    nameNum = 0;
                    itemNameType = "";
                    break;
            }

            string name;
            if (iT != itemType.Trash)
            {
                int descPreNum = rand.Next(0, itemPrefixDesc.Length); //pick a random item prefix
                int descSufNum = rand.Next(0, itemSuffixDesc.Length); //pick a random item suffix
                name = itemPrefixDesc[descPreNum] + " " + itemNameType + " of " + itemSuffixDesc[descSufNum];
            }
            else name = itemNameType;

            Item i = new Item(name, iT, value);
            return i;
        }

        public Monster generateMonster()                //generate monster with random name
        {
            string name = monsterDescriptors[rand.Next(0, monsterDescriptors.Length)] + " " + monsterNames[rand.Next(0, monsterNames.Length)]; //put together a name with a random descriptor and type

            Monster m = new Monster(name, pc.level);
            return m;
        }

        //TODO: generate phrases for the various events (see the Quest class)
    }
}
