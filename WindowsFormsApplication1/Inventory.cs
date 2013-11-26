using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace idleQuest
{
    public class Inventory
    {
        Item[] items = new Item[Avatar.MAX_INV];
        Avatar pc;
        Equipment equipped;

        public Inventory(ref Avatar player)
        {
            pc = player;
            equipped = new Equipment(ref pc);
        }

        public bool addItem(Item i)     //returns true if the item was equipped and false if the item was just added to inventory.
        {
            Item invItem;
            if (i == null) return false;    //a null item can't be equipped or added to inv
            else {
                invItem = equipped.checkToEquip(i);    //equip the item if possible. Get back the removed item, or the original item if it was not equipped. Get back null if nothing was equipped in that slot.
                if (invItem != null)
                {
                    items[pc.invCount] = invItem;
                    pc.invCount++;
                }
            }
            if (invItem == i) return false;  //if the returned item is the same as the added item, it was not equipped
            else return true;                //else, or if the invItem is null, something must have been equipped
        }

        public Item popLastItem()
        {
            Item i = items[pc.invCount - 1];
            items[pc.invCount - 1] = null;
            pc.invCount--;
            return i;
        }

        public Item getItem(int i)
        {
            return items[i];
        }

        public int getNumberOfItems()       //returns the amount of items currently held
        {
            int count = 0;
            for (int i = 0; i < Avatar.MAX_INV; i++)
            {
                if (items[i] != null) count++;
            }
            return count;
        }

        public Item getEquippedItem(itemType iT)    //returns the piece of equipment that is currently equipped in the requested slot
        {
            return equipped.getEquippedItem(iT);
        }

        public void saveInventory(BinaryWriter gameSave)    //saves the inventory to a file
        {
            for (int i = 0; i < getNumberOfItems(); ++i)
            {
                items[i].saveItem(gameSave);
            }
            equipped.saveEquipment(gameSave);
        }

        public void loadInventory(BinaryReader gameLoad)    //loads the inventory from a file
        {
            for (int i = 0; i < pc.invCount; ++i)
            {
                items[i] = new Item();
                items[i].loadItem(gameLoad);
            }
            equipped.loadEquipment(gameLoad);
        }
    }
}
