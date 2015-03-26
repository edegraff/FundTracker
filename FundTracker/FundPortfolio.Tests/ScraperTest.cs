using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Models;

namespace FundPortfolio.Tests
{
    [TestClass]
    public class ScraperTest
    {
        [TestMethod]
        public void TestScraper()
        {
            //FundService.Scraper scraper = new FundService.Scraper("https://www.cibc.com/ca/rates/mutual-fund-rates.html");
            //scraper.Scrape();
            ReadFundData();
        }

        private void ReadFundData()
        {
            Console.WriteLine(DateTime.Now);
            using (var db = new DatabaseContext())
            {
                var funds = db.Funds;
                foreach (var f in funds)
                {
                    Console.WriteLine(f.name);
                    foreach (var fd in f.FundHistory)
                    {
                        Console.WriteLine(fd.Date + " -> " + fd.Value);
                    }
                }
            }
        }
    }
}
