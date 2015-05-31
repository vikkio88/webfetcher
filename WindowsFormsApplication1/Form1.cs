using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

using System.Windows.Forms;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;
using WebPageFetcher;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private const string URL = @"http://www.maltapark.com/property/listings.aspx?category=247"; //long lets
        public const double MAX_PRICE = 450;
        public const double MIN_PRICE = 100;

        public const int DEEP = 20;

        private int counter = 0;
        //const string CONTAINS

        private List<MaltaParkArticle> _generalList;
        public List<MaltaParkArticle> GeneralList
        {
            set
            {
                _generalList = value;
            }
            get
            {
                if (_generalList == null)
                {
                    _generalList = new List<MaltaParkArticle>();
                }
                return _generalList;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Avvia")
            {


                int val = comboBox1.SelectedIndex + 1;
                if (val == -1) val = 2;
                counter = 0;

                LogStatus("Started, interval minutes: " + val);
                LogStatus(string.Format("Criteria: Range ({0},{1}) euros", MIN_PRICE, MAX_PRICE));



                timer1.Interval = val * 1000 * 60;
                timer1_Tick(new object(), new EventArgs());
                timer1.Start();
                button1.Text = "Stop";

                comboBox1.Enabled = false;


            }
            else
            {
                timer1.Stop();
                button1.Text = "Avvia";

                comboBox1.Enabled = true;
                LogStatus("Stopped");
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (counter == 0)
            {
                LogStatus("Starting Deep Search");
                for (int i = 1; i <= DEEP; i++)
                {
                    LogStatus(string.Format("Getting page {0}/{1}", i, DEEP));
                    string page = GetPage(i);
                    HtmlNode section = GetSection(page);
                    List<MaltaParkArticle> newlist = MaltaParkArticle.LoadFromSectionHtml(section);
                    FillGeneral(newlist);
                    LogStatus("Total article cached: " + GeneralList.Count);
                    foreach (var article in GeneralList)
                    {
                        if (!article.Showed && article.MatchCriteria)
                        {
                            LogStatus("Found Matching Article ID:" + article.Id);
                            Show(article);
                            Save(article);
                            article.Showed = true;
                        }
                    }
                }
                counter += 1;
            }
            else
            {
                LogStatus("Fetching First Page");
                LogStatus("Getting page 1");
                string page = GetPage();
                HtmlNode section = GetSection(page);
                List<MaltaParkArticle> newlist = MaltaParkArticle.LoadFromSectionHtml(section);
                FillGeneral(newlist);
                LogStatus("Total article cached: " + GeneralList.Count);
                foreach (var article in GeneralList)
                {
                    if (!article.Showed && article.MatchCriteria)
                    {
                        LogStatus("Found Matching Article ID:" + article.Id);
                        Show(article);
                        Save(article);
                        article.Showed = true;
                    }
                }
            }


        }

        private void Save(MaltaParkArticle article)
        {
            File.WriteAllText(article.Id + ".txt", article.ToString());
        }

        private void Show(MaltaParkArticle article)
        {
            if (checkBox1.Checked)
            {
                string notifica = "Trovato '" + article.Title + "' e salvato in " + article.Id + ".txt";
                notifyIcon1.Icon = SystemIcons.Information;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000, "Salvataggio", notifica, ToolTipIcon.Info);
            }

            textBox1.Text += "****************\r\n" + article.ToString() + "\r\n";
        }

        private void FillGeneral(List<MaltaParkArticle> newlist)
        {
            foreach (var article in newlist)
            {
                if (!GeneralList.Any(a => a.Id == article.Id))
                {
                    GeneralList.Add(article);
                }
            }
        }

        private HtmlNode GetSection(string page)
        {
            //divListings
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);

            return doc.GetElementbyId("divListings");
        }

        private string GetPage(int page = 0)
        {
            if (page <= 1)
            {
                return Fetcher.ToString(URL);
            }
            else
            {
                return Fetcher.ToString(string.Format("{0}?page={1}", URL, page));
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

        private void LogStatus(string message)
        {
            statusBox.Text += string.Format("\r\n{0} : {1}", DateTime.Now, message);
        }

    }
}
