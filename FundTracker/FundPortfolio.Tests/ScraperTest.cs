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
            FundService.Scraper scraper = new FundService.Scraper("https://www.cibc.com/ca/rates/mutual-fund-rates.html");
            scraper.Scrape();

            Assert.IsTrue(scraper.date != null);
            Assert.IsTrue(scraper.html != null);
            Assert.IsTrue(scraper.html.Contains("html"));
            Assert.IsTrue(scraper.ids != null);
            Assert.IsTrue(scraper.ids.Length > 10);
            Assert.IsTrue(scraper.funds != null);
            Assert.IsTrue(scraper.funds.Count > 10);
        }

    }
}
