using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace idleQuest
{
    public partial class idleForm : Form
    {
        Thread gameThread;        //handles all "game" mechanics
        Thread guiThread;         //handles updating of most GUI objects
        Quest iQuest;
        UIControl guiUpdater;
        Avatar pc;                //holds character attributes
        Inventory inv;            //items

        public idleForm()
        {
            InitializeComponent(); 
        }

        private void idleForm_Load(object sender, EventArgs e)
        {
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "http://jasoncostabile.com/";
            lnk_JC.Links.Add(link);

            ActiveControl = rtb_main;       //keep focus on main textbox

            startQuest();
        }

        private void startQuest()       //beginning the "game"
        {
            pc = new Avatar();
            inv = new Inventory(ref pc);
            iQuest = new Quest(this.rtb_main);
            iQuest.loadQuest();
            guiUpdater = new UIControl(iQuest.getPlayer(), iQuest.getInventory(), this.txt_hp, this.txt_maxhp, this.txt_mp, this.txt_maxmp, this.txt_att, this.txt_def, this.txt_mag, this.txt_level, this.txt_exp, this.txt_gold, this.lbl_invCount, this.txt_weapon, this.txt_offhand, this.txt_torso, this.txt_head, this.txt_feet, this.txt_hands, this.txt_finger, this.txt_back, this.txt_neck, this.lst_inv);  //give the gui updater access to all controls it is responsible for
            gameThread = new Thread(new ThreadStart(iQuest.go));
            gameThread.Name = "GAME_THREAD";
            guiThread = new Thread(new ThreadStart(guiUpdater.startUpdate));
            guiThread.Name = "GUI_THREAD";
            gameThread.Start();         //start "game" mechanics
            guiThread.Start();          //start refreshing GUI
        }

        private void idleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            iQuest.saveQuest();
            Environment.Exit(0);
        }

        private void lnk_JC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);      //send the URL to the OS
        }

        private void rtb_main_Leave(object sender, EventArgs e)
        {
            ActiveControl = rtb_main;       //keep focus on the main textbox
        }

        private void rtb_main_TextChanged(object sender, EventArgs e)
        {
            this.rtb_main.Select(this.rtb_main.TextLength, 0);    //move caret to end of text
            this.rtb_main.ScrollToCaret();                        //scroll to the bottom whenever text is added
        }
    }
}






