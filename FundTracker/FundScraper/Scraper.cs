using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FundScraper
{
    class Scraper
    {
        private string url;
        private string html;
        private string[] ids;
        private List<FundEntity> funds;

        public Scraper(string url)
        {
            this.url = url;
            this.ids = new string[] {"580", "581", "483", "583", "937"};
        }

        static void Main(string[] args)
        {
            Scraper scraper = new Scraper("https://www.cibc.com/ca/rates/mutual-fund-rates.html");
            scraper.Scrape();
        }

        public void Scrape()
        {
            RetrieveHTML();
            ParseHTML();
            ParseJSON();
        }

        private void RetrieveHTML()
        {
            string data = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            this.html = data;
        }

        private void ParseHTML()
        {
            this.funds = new List<FundEntity>();
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(this.html);
            foreach(string id in ids)
            {
                FundEntity f = new FundEntity();
                f.id = id;
                f.name = htmlDoc.DocumentNode.SelectSingleNode("//*[contains(text(), '" + id + ".rates.x1_Month.rate\"')]").ParentNode.ParentNode.ChildNodes[1].FirstChild.InnerText;
                this.funds.Add(f);
            }
        }

        public string MakeRequest(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void ParseJSON()
        {
            foreach (FundEntity fund in this.funds)
            {
                string response = MakeRequest("https://www.cibc.com/ratesservice/rds?lobId=7&sourceProductCode=" + fund.id);
                Newtonsoft.Json.Linq.JObject json = JsonConvert.DeserializeObject(response.Substring(response.IndexOf("{"))) as Newtonsoft.Json.Linq.JObject;
                fund.currentValue = float.Parse(json.Last.First.Last.ElementAt(3).ToString());
                string[] date = json.Root.ElementAt(2).ToString().Split();
                fund.currentDate = DateTime.Now;
            }
        }

    }
}
