using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Newtonsoft.Json;

namespace FundService
{
	public class Scraper
	{
		private string url;
		private string html;
		private string[] ids;
		private List<FundEntity> funds;
        private string[] exclusions = { "581", "482", "480", "580", "483", "583" };
        private DateTime date;
        private Dictionary<String, int> months;

		public Scraper(string url)
		{
            this.url = url;
            months = new Dictionary<string, int> { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };
		}

		static void Main(string[] args)
		{
			Scraper scraper = new Scraper("https://www.cibc.com/ca/rates/mutual-fund-rates.html");
			scraper.Scrape();
		}

		public void Scrape()
		{
            ParseDate();
			RetrieveHTML();
			ParseIds();
			ParseNames();
			ParseJSON();
			SaveFundData();
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

        private void ParseDate()
        {
            string response = MakeRequest("https://www.cibc.com/ratesservice/rds?lobId=7&sourceProductCode=483");
            Newtonsoft.Json.Linq.JObject json = JsonConvert.DeserializeObject(response.Substring(response.IndexOf("{"))) as Newtonsoft.Json.Linq.JObject;
            var date = json.Last.First.Last.ElementAt(5).ToString().Split();
            this.date = new DateTime(DateTime.Now.Year, months[date[1]], int.Parse(date[2]));
        }

		private void ParseIds()
		{
			this.funds = new List<FundEntity>();
			HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
			htmlDoc.LoadHtml(this.html);
			var nodes = htmlDoc.DocumentNode.SelectNodes("//*[contains(text(), '.rates.x1_Month.rate\"')]");
			var pids = new SortedSet<string>();
			foreach (var node in nodes)
			{
				pids.Add(node.InnerText.Substring(node.InnerText.IndexOf("p") + 1, 3));
			}
			this.ids = pids.ToArray<string>();
		}

		private void ParseNames()
		{
            
			this.funds = new List<FundEntity>();
			HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
			htmlDoc.LoadHtml(this.html);
			foreach (string id in ids)
			{
                if (exclusions.Contains(id))
                    continue;
				FundEntity f = new FundEntity();
				f.Id = id;
				f.name = htmlDoc.DocumentNode.SelectSingleNode("//*[contains(text(), '" + id + ".rates.x1_Month.rate\"')]").ParentNode.ParentNode.ChildNodes[1].Element("a").InnerText;
				this.funds.Add(f);
			}
		}

		private string MakeRequest(string url)
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

		private void ParseJSON()
		{
			foreach (FundEntity fund in this.funds)
			{
				string response = MakeRequest("https://www.cibc.com/ratesservice/rds?lobId=7&sourceProductCode=" + fund.Id);
				Newtonsoft.Json.Linq.JObject json = JsonConvert.DeserializeObject(response.Substring(response.IndexOf("{"))) as Newtonsoft.Json.Linq.JObject;
				fund.CurrentValue = float.Parse(json.Last.First.Last.ElementAt(3).ToString());
                fund.FundHistory[0].Date = this.date;
            }
		}

		private void SaveFundData()
		{
			using (var db = new DatabaseContext())
			{
				foreach (var fund in this.funds)
				{
					var existingFund = db.Funds.Find(fund.Id);
					if (existingFund != null)
					{
                        if (existingFund.CurrentValue != fund.CurrentValue)
                        {
                            if (existingFund.FundHistory.Last().Date.DayOfYear == fund.FundHistory[0].Date.DayOfYear)
                            {
                                // Value has updated for this date
                                existingFund.FundHistory.Last().Value = fund.CurrentValue;
                            }
                            else
                            {
                                existingFund.FundHistory.AddRange(fund.FundHistory);
                            }
                        }
						existingFund.name = fund.name;
					}
					else
					{
						db.Funds.Add(fund);
					}
				}

				db.SaveChanges();
			}
		}
	}
}
