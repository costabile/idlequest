using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace idleQuest
{
    class Equipment
    {
        public Item weapon, offhand, torso, head, hands, feet, back, finger, neck;      //currently equipped items
        Avatar pc;

        public Equipment(ref Avatar player)      //constructor
        {
            //initialize all equip slots as empty (use placeholder items with 0 stats)
            weapon = new Item(itemType.Weapon);
            offhand = new Item(itemType.Offhand);
            torso = new Item(itemType.Torso);
            head = new Item(itemType.Head);
            hands = new Item(itemType.Hands);
            feet = new Item(itemType.Feet);
            back = new Item(itemType.Back);
            finger = new Item(itemType.Finger);
            neck = new Item(itemType.Neck);
            pc = player;
        }

        public Item getEquippedItem(itemType iT)
        {      //returns the item which occupies the requested slot
            switch (iT)
            {
                case itemType.Weapon:
                    return weapon;
                case itemType.Offhand:
                    return offhand;
                case itemType.Torso:
                    return torso;
                case itemType.Head:
                    return head;
                case itemType.Feet:
                    return feet;
                case itemType.Hands:
                    return hands;
                case itemType.Finger:
                    return finger;
                case itemType.Back:
                    return back;
                case itemType.Neck:
                    return neck;
                default:
                    return null;
            }
        }

        public Item checkToEquip(Item i)    //returns whichever item was unequipped (to be put back in inventory). Returns null if nothing was previously equipped in the relevant slot.
        {
            itemType iT = i.getType();
            Item unequipped;
            if (!(iT == itemType.Trash) && (checkAtt(iT, i.getAttrValue())))      //vendor trash items can't be equipped
            {
                unequipped = equip(i);   //equip new item. Return whatever item was replaced.
            }
            else
            {
                unequipped = i;          //item was not equipped. Return item.
            }
            if (unequipped.getAttrValue() == 0) return null;    //discard placeholder items.
            else return unequipped;
        }

        private Item equip(Item i)      //wear an item. Apply attribute value to player's stats. Returns the item that was removed.
        {
            itemType iT = i.getType();
            Item removedItem = remove(iT);     //take off old item first
            switch (iT)
            {
                case itemType.Weapon:
                    weapon = i;
                    pc.att += weapon.getAttrValue();        //weapons add attack
                    break;
                case itemType.Offhand:
                    offhand = i;
                    pc.att += offhand.getAttrValue();       //offhand weapons add attack
                    break;
                case itemType.Torso:
                    torso = i;
                    pc.defense += torso.getAttrValue();     //torso items add def
                    break;
                case itemType.Head:
                    head = i;
                    pc.defense += head.getAttrValue();      //head items add def
                    break;
                case itemType.Feet:
                    feet = i;
                    pc.defense += feet.getAttrValue();      //feet items add def
                    break;
                case itemType.Hands:
                    hands = i;
                    pc.defense += hands.getAttrValue();     //hands items add def
                    break;
                case itemType.Back:
                    back = i;
                    pc.defense += back.getAttrValue();      //back items add def
                    break;
                case itemType.Finger:
                    finger = i;
                    pc.magic += finger.getAttrValue();      //finger items add magic
                    break;
                case itemType.Neck:
                    neck = i;
                    pc.magic += neck.getAttrValue();        //neck items add magic
                    break;
            }
            return removedItem;
        }

        private Item remove(itemType iT)    //take off an item (only occurs when equipping a better item). Subtract attribute value from stats. Returns the removed item.
        {
            switch (iT)
            {
                case itemType.Weapon:
                    pc.att -= weapon.getAttrValue();        //weapons add attack
                    return weapon;
                case itemType.Offhand:
                    pc.att -= offhand.getAttrValue();       //offhand weapons add attack
                    return offhand;
                case itemType.Torso:
                    pc.defense -= torso.getAttrValue();     //torso items add def
                    return torso;
                case itemType.Head:
                    pc.defense -= head.getAttrValue();      //head items add def
                    return head;
                case itemType.Feet:
                    pc.defense -= feet.getAttrValue();      //feet items add def
                    return feet;
                case itemType.Hands:
                    pc.defense -= hands.getAttrValue();     //hands items add def
                    return hands;
                case itemType.Finger:
                    pc.magic -= finger.getAttrValue();      //finger items add magic
                    return finger;
                case itemType.Back:
                    pc.defense -= back.getAttrValue();      //back items add def
                    return back;
                case itemType.Neck:
                    pc.magic -= neck.getAttrValue();        //neck items add magic
                    return neck;
                default:
                    return null;
            }
        }

        private bool checkAtt(itemType iT, int attrVal)
        {        //checks whether the attribute value of an item is better than the one currently equipped
            switch (iT)
            {
                case itemType.Weapon:
                    if (attrVal > weapon.getAttrValue()) return true;
                    break;
                case itemType.Offhand:
                    if (attrVal > offhand.getAttrValue()) return true;
                    break;
                case itemType.Torso:
                    if (attrVal > torso.getAttrValue()) return true;
                    break;
                case itemType.Head:
                    if (attrVal > head.getAttrValue()) return true;
                    break;
                case itemType.Feet:
                    if (attrVal > feet.getAttrValue()) return true;
                    break;
                case itemType.Hands:
                    if (attrVal > hands.getAttrValue()) return true;
                    break;
                case itemType.Finger:
                    if (attrVal > finger.getAttrValue()) return true;
                    break;
                case itemType.Back:
                    if (attrVal > back.getAttrValue()) return true;
                    break;
                case itemType.Neck:
                    if (attrVal > neck.getAttrValue()) return true;
                    break;
            }
            return false;
        }

        public void saveEquipment(BinaryWriter gameSave)
        {
            weapon.saveItem(gameSave);
            offhand.saveItem(gameSave);
            torso.saveItem(gameSave);
            head.saveItem(gameSave);
            hands.saveItem(gameSave);
            feet.saveItem(gameSave);
            back.saveItem(gameSave);
            finger.saveItem(gameSave);
            neck.saveItem(gameSave);
        }

        public void loadEquipment(BinaryReader gameLoad)
        {
            weapon.loadItem(gameLoad);
            offhand.loadItem(gameLoad);
            torso.loadItem(gameLoad);
            head.loadItem(gameLoad);
            hands.loadItem(gameLoad);
            feet.loadItem(gameLoad);
            back.loadItem(gameLoad);
            finger.loadItem(gameLoad);
            neck.loadItem(gameLoad);
        }
    }
}
