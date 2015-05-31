using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

using System.Windows.Forms;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text=="Avvia")
            {
                int val = comboBox1.SelectedIndex + 1;
                if (val == -1) val = 2;
              
                   

                timer1.Interval = val * 1000;
                timer1_Tick(new object(), new EventArgs());
                timer1.Start();
                button1.Text = "Stop";
                button4.Enabled = true;
                comboBox1.Enabled = false;
                txtfilter.Enabled = false;
            }else{
                timer1.Stop();
                button1.Text = "Avvia";
                button4.Enabled = true;
                comboBox1.Enabled = true;
                txtfilter.Enabled = true;
            }
            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string result = StringFromUrl.exec("http://pastebin.com");
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(result);

            HtmlNode a = doc.GetElementbyId("menu_2");
            Regex reg = new Regex("<ul class=\"right_menu\"><li><a href=\"/(.+?)\"");
            string url = reg.Match(a.OuterHtml).Groups[1].ToString();
            //MessageBox.Show(reg.Match(a.OuterHtml).Groups[1].ToString());
            if (url!=null)
            {
                result = StringFromUrl.exec("http://pastebin.com/raw.php?i=" + url);
                label1.Text = "pastebin.com/" + url;
                lblhide.Text = url;
                File.WriteAllText("back.txt", textBox1.Text);
                //implementare funzione back
                textBox1.Text = result;

                if (txtfilter.Text != "")
                {
                    if (result.IndexOf(txtfilter.Text) != -1)
                    {
                        File.WriteAllText(lblhide.Text + ".txt", textBox1.Text);
                    

                        if (checkBox1.Checked)
                        {
                       
                            string notifica = "Trovato '"+txtfilter.Text+"' e salvato in "+lblhide.Text + ".txt";
                            notifyIcon1.Icon = SystemIcons.Information;
                            notifyIcon1.Visible = true;
                            notifyIcon1.ShowBalloonTip(900, "Salvataggio",notifica, ToolTipIcon.Info);

                        }

                    }
                }
            }
           
        }

        private void label1_Click(object sender, EventArgs e)
        {
            StringBuilder clipboard = new StringBuilder();
            
            clipboard.Append(label1.Text + "\n");
            Clipboard.SetText(clipboard.ToString());
            MessageBox.Show(clipboard.ToString() + " copiato negli Appunti");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lblhide.Text != "")
            {
                File.WriteAllText(lblhide.Text + ".txt", textBox1.Text);
                if (File.Exists(lblhide.Text + ".txt"))
                {
                    MessageBox.Show("File salvato con successo\n" + lblhide.Text + ".txt", "Avviso");
                }
                else
                {
                    MessageBox.Show("Errore nel salvataggio", "Errore!");
                }
            }
            else
            {
                MessageBox.Show("Errore!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1_Click(new object(), new EventArgs());
            textBox1.Text =  File.ReadAllText("back.txt");
            MessageBox.Show("Restore Effettuato");
            button4.Enabled = false;
        }


   






    }
}
