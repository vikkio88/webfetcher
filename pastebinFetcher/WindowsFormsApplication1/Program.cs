using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public static class StringFromUrl
    {


        public static string exec( string url)
        {
            string result = "";
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                try
                {
                    result = client.DownloadString(url);
                }
                catch (Exception)
                {

                    result = "";
                }
               
            }

            return result;
        }
    }
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
