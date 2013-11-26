using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace idleQuest
{
    public enum itemType {Weapon, Offhand, Torso, Head, Feet, Hands, Back, Finger, Neck, Trash};
    
    public class Item
    {
        string name;
        itemType type;
        int attrValue;       //value of the attribute that the item adds, ex. weapon adds attack, torso adds def, ring adds mag. Also the sale value.

        public Item()       //blank constructor
        {
            name = null;
            type = itemType.Trash;
            attrValue = 0;
        }

        public Item(itemType iT)       //"placeholder item" constructor
        {
            name = "--";
            type = iT;
            attrValue = 0;
        }

        public Item(string itemName, itemType iType, int attributeValue)     //constructor
        {
            name = itemName;
            type = iType;
            attrValue = attributeValue;
        }

        public string getName()
        {
            return name;
        }

        public int getAttrValue()       //returns attribute/gold value of the item
        {
            return attrValue;
        }

        public itemType getType()
        {
            return type;
        }

        public void saveItem(BinaryWriter gameSave)
        {
            gameSave.Write(name);
            gameSave.Write((int)type);
            gameSave.Write(attrValue);
        }

        public void loadItem(BinaryReader gameLoad)
        {
            name = gameLoad.ReadString();
            type = (itemType)gameLoad.ReadInt32();
            attrValue = gameLoad.ReadInt32();
        }
    }
}
