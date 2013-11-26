using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace idleQuest
{
    public class UIControl         //handles updating of all gui objects (except main rtb)
    {
        TextBox hp_tb, maxhp_tb, mp_tb, maxmp_tb, att_tb, def_tb, mag_tb, lvl_tb, exp_tb, gold_tb, weapon_tb, offhand_tb, torso_tb, head_tb, hands_tb, feet_tb, back_tb, finger_tb, neck_tb;
        Label invCount_lb;
        ListBox inv_lst;
        Avatar pc;
        Inventory inv;

        public UIControl(Avatar player, Inventory inven, TextBox hp, TextBox maxhp, TextBox mp, TextBox maxmp, TextBox attack, TextBox defense, TextBox magic, TextBox level, TextBox exp, TextBox gold, Label invCount, TextBox eqWeapon, TextBox eqOffhand, TextBox eqTorso, TextBox eqHead, TextBox eqFeet, TextBox eqHands, TextBox eqFinger, TextBox eqBack, TextBox eqNeck, ListBox invBox)
        {
            pc = player;
            inv = inven;
            hp_tb = hp;
            maxhp_tb = maxhp;
            mp_tb = mp;
            maxmp_tb = maxmp;
            att_tb = attack;
            def_tb = defense;
            mag_tb = magic;
            lvl_tb = level;
            exp_tb = exp;
            gold_tb = gold;
            invCount_lb = invCount;
            weapon_tb = eqWeapon;
            offhand_tb = eqOffhand;
            torso_tb = eqTorso;
            head_tb = eqHead;
            hands_tb = eqHands;
            feet_tb = eqFeet;
            finger_tb = eqFinger;
            back_tb = eqBack;
            neck_tb = eqNeck;
            inv_lst = invBox;
        }

        public void startUpdate()
        {
            while (true)    //infinitely update GUI
            {
                if (this.hp_tb.InvokeRequired) this.hp_tb.Invoke(new MethodInvoker(delegate { this.hp_tb.Text = "" + pc.hp; }));      //update hp
                if (this.maxhp_tb.InvokeRequired) this.maxhp_tb.Invoke(new MethodInvoker(delegate { this.maxhp_tb.Text = "" + pc.maxhp; }));      //update max hp
                if (this.mp_tb.InvokeRequired) this.mp_tb.Invoke(new MethodInvoker(delegate { this.mp_tb.Text = "" + pc.mp; }));      //update mp
                if (this.maxmp_tb.InvokeRequired) this.maxmp_tb.Invoke(new MethodInvoker(delegate { this.maxmp_tb.Text = "" + pc.maxmp; }));      //update max mp
                if (this.att_tb.InvokeRequired) this.att_tb.Invoke(new MethodInvoker(delegate { this.att_tb.Text = "" + pc.att; }));      //update att
                if (this.def_tb.InvokeRequired) this.def_tb.Invoke(new MethodInvoker(delegate { this.def_tb.Text = "" + pc.defense; }));      //update def
                if (this.mag_tb.InvokeRequired) this.mag_tb.Invoke(new MethodInvoker(delegate { this.mag_tb.Text = "" + pc.magic; }));      //update mag
                if (this.lvl_tb.InvokeRequired) this.lvl_tb.Invoke(new MethodInvoker(delegate { this.lvl_tb.Text = "" + pc.level; }));      //update lvl
                if (this.exp_tb.InvokeRequired) this.exp_tb.Invoke(new MethodInvoker(delegate { this.exp_tb.Text = "" + pc.exp; }));      //update exp
                if (this.gold_tb.InvokeRequired) this.gold_tb.Invoke(new MethodInvoker(delegate { this.gold_tb.Text = "" + pc.gold; }));      //update gold
                if (this.invCount_lb.InvokeRequired) this.invCount_lb.Invoke(new MethodInvoker(delegate { this.invCount_lb.Text = "(" + pc.invCount + "/" + Avatar.MAX_INV + ")"; }));   //update inventory count
                if (this.weapon_tb.InvokeRequired) this.weapon_tb.Invoke(new MethodInvoker(delegate { this.weapon_tb.Text = inv.getEquippedItem(itemType.Weapon).getName(); }));    //update weapon name
                if (this.offhand_tb.InvokeRequired) this.offhand_tb.Invoke(new MethodInvoker(delegate { this.offhand_tb.Text = inv.getEquippedItem(itemType.Offhand).getName(); }));    //update offhand name
                if (this.torso_tb.InvokeRequired) this.torso_tb.Invoke(new MethodInvoker(delegate { this.torso_tb.Text = inv.getEquippedItem(itemType.Torso).getName(); }));    //update torso name
                if (this.head_tb.InvokeRequired) this.head_tb.Invoke(new MethodInvoker(delegate { this.head_tb.Text = inv.getEquippedItem(itemType.Head).getName(); }));    //update head name
                if (this.feet_tb.InvokeRequired) this.feet_tb.Invoke(new MethodInvoker(delegate { this.feet_tb.Text = inv.getEquippedItem(itemType.Feet).getName(); }));    //update feet name
                if (this.hands_tb.InvokeRequired) this.hands_tb.Invoke(new MethodInvoker(delegate { this.hands_tb.Text = inv.getEquippedItem(itemType.Hands).getName(); }));    //update hands name
                if (this.finger_tb.InvokeRequired) this.finger_tb.Invoke(new MethodInvoker(delegate { this.finger_tb.Text = inv.getEquippedItem(itemType.Finger).getName(); }));    //update finger name
                if (this.back_tb.InvokeRequired) this.back_tb.Invoke(new MethodInvoker(delegate { this.back_tb.Text = inv.getEquippedItem(itemType.Back).getName(); }));    //update back name
                if (this.neck_tb.InvokeRequired) this.neck_tb.Invoke(new MethodInvoker(delegate { this.neck_tb.Text = inv.getEquippedItem(itemType.Neck).getName(); }));    //update neck name

                if (this.inv_lst.InvokeRequired) this.inv_lst.Invoke(new MethodInvoker(delegate
                    {
                        this.inv_lst.Items.Clear();
                        for (int i = 0; i < inv.getNumberOfItems(); i++)
                        {
                            if (inv.getItem(i).getName() != null) this.inv_lst.Items.Add((object)inv.getItem(i).getName());
                        }
                    }));    //update inventory list
                
                Thread.Sleep(500);  //wait before next update
            }   //end of update loop
        }
    }
}
