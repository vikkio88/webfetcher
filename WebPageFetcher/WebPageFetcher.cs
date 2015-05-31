using System;
using System.Net;

namespace WebPageFetcher
{
    public static class Fetcher
    {
        public static string ToString(string url)
        {
            string result;
            using (WebClient client = new WebClient())
            {
                try
                {
                    result = client.DownloadString(url);
                }
                catch (Exception)
                {
                    result = string.Empty;
                }
            }

            return result;
        }
    }

}
