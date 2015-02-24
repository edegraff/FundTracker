using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Newtonsoft.Json;

namespace FundTracker.FundScraper
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
		}

		static void Main(string[] args)
		{
			Scraper scraper = new Scraper("https://www.cibc.com/ca/rates/mutual-fund-rates.html");
			scraper.Scrape();
		}

		public void Scrape()
		{
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
				FundEntity f = new FundEntity();
				f.id = id;
				f.name = htmlDoc.DocumentNode.SelectSingleNode("//*[contains(text(), '" + id + ".rates.x1_Month.rate\"')]").ParentNode.ParentNode.ChildNodes[1].Element("a").InnerText;
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
				fund.CurrentValue = float.Parse(json.Last.First.Last.ElementAt(3).ToString());
			}
		}

		private void SaveFundData()
		{
			using (var db = new DatabaseContext())
			{
				foreach (var fund in this.funds)
				{
					var existingFund = db.Funds.Find(fund.id);
					if (existingFund != null)
					{
						existingFund.FundHistory.AddRange(fund.FundHistory);
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
