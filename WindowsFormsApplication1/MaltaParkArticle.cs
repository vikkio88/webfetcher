using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication1
{
    public class MaltaParkArticle
    {
        private const string ARTICLE_URL = @"http://www.maltapark.com/item.aspx?ItemID=";
        private const string RE_ITEM = @"class=""item"">(.+?)<div style=""clear:both";
        private const string RE_ID = @"temID=(.+?)""";
        private const string RE_PRICE = @"&euro;&nbsp;(.+?)&nbsp;";
        private const string RE_TITLE = @"title"">.+ItemID=\d+"">(.+?)<\/a";

        public int Id { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }
        public string Html { get; set; }
        public bool Showed { get; set; }
        internal static List<MaltaParkArticle> LoadFromSectionHtml(HtmlNode section)
        {
            List<MaltaParkArticle> list = new List<MaltaParkArticle>();
            
            string text = section.InnerHtml;
            Regex regex = new Regex(@"(\r\n|\r|\n)+");
            Regex cleanPrice = new Regex(@"\.\d\d");
            text = regex.Replace(text, "");


            MatchCollection matches = Regex.Matches(text, RE_ITEM);
            foreach (var item in matches)
            {

                list.Add(
                    new MaltaParkArticle 
                    { 
                        Id = int.Parse(Filter(item.ToString(), RE_ID)),
                        Title = Filter(item.ToString(),RE_TITLE),
                        Price = double.Parse((Filter(item.ToString(), RE_PRICE)).Replace(",","")),
                        Html = item.ToString() 
                    }
                    );
            }



            return list;
        }

        private static string Filter(string p, string RE)
        {
            return Regex.Match(p, RE).Groups[1].Value;
        }

        public bool MatchCriteria
        {
            get
            {
                if (Price > Form1.MIN_PRICE && Price < Form1.MAX_PRICE) return true;
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("Title: {0}\r\nPrice:{1} Euro\r\nUrl: {2}{3}", Title, Price, ARTICLE_URL, Id);
        }
    }
}
