using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Models;

namespace FundPortfolio.Tests
{
    [TestClass]
    public class TestReport
    {
        /*
         * 3.2.1.1.2 Visitors and Users will be able to compare mutual funds side by side.
         * The report object conglomerates data so that it is easier to display comparison data.
         */
        [TestMethod]
        public void ReportObjectInit()
        {
            // Stub
            int difference = 7;
            DateTime start = DateTime.Today.AddDays(-3).Date;
            DateTime end = start.AddDays(difference);

            IEnumerable<ITimeSeriesFundData> funds = createStub(start);

            Report rep = new Report(start, end, funds);

            // test headers
            Assert.IsTrue(rep.Headers[0].Equals("Date"));
            Assert.IsTrue(rep.Headers[1].Equals("test fund 1"));
            Assert.IsTrue(rep.Headers[2].Equals("test fund 2"));

            // test all dates are there
            for (int i = 0; i <= difference; i++)
            {
                Assert.IsTrue(rep.Data[0][i].Equals(formatDate(end.AddDays(-i))));
            }

            /* check values */
            // fund 1
            Assert.IsTrue(rep.Data[1][0].Equals("Further Projection Unavailable"));
            float.Parse(rep.Data[1][1]);  // projection value, we don't know what, just ensure it is a number
            float.Parse(rep.Data[1][2]);  // projection value, we don't know what, just ensure it is a number
            float.Parse(rep.Data[1][3]);  // projection value, we don't know what, just ensure it is a number
            Assert.IsTrue(float.Parse(rep.Data[1][4]) == 4);
            Assert.IsTrue(float.Parse(rep.Data[1][5]) == 4);
            Assert.IsTrue(float.Parse(rep.Data[1][6]) == 3);
            Assert.IsTrue(float.Parse(rep.Data[1][7]) == 2);
            //fund 2
            Assert.IsTrue(rep.Data[2][0].Equals("Further Projection Unavailable"));
            float.Parse(rep.Data[2][1]);  // projection value, we don't know what, just ensure it is a number
            float.Parse(rep.Data[2][2]);  // projection value, we don't know what, just ensure it is a number
            float.Parse(rep.Data[2][3]);  // projection value, we don't know what, just ensure it is a number
            Assert.IsTrue(float.Parse(rep.Data[2][4]) == 2);
            Assert.IsTrue(float.Parse(rep.Data[2][5]) == 1);
            Assert.IsTrue(rep.Data[2][6].Equals("Earlier Data Unavailable"));
            Assert.IsTrue(rep.Data[2][7].Equals("Earlier Data Unavailable"));
            
        }

        /*
         * Initializes some fake data.
         * includes 2 fake funds.
         * fund1 - name = "test fund 1"
         *       - id = "fund1test"
         *       - data
         *          - start-1 : 2
         *          - start+1 : 3
         *          - start+2 : 4
         *          - start+4 : 5
         * fund2 - name = "test fund 2"
         *       - id = "fund2test"
         *       - data
         *          - start+2 : 1
         *          - start+3 : 2
         */
        private IEnumerable<ITimeSeriesFundData> createStub(DateTime start) 
        {
            List<ITimeSeriesFundData> result = new List<ITimeSeriesFundData>();
            FundEntity fund;
            FundData data;
            List<FundData> funddata;

            /* set up fund 1 */
            fund = new FundEntity();
            fund.Id = "fund1test";
            fund.Name = "test fund 1";
            funddata = new List<FundData>();

            // Data starts before start date
            data = new FundData();
            data.FundEntity = fund;
            data.FundEntityId = fund.Id;
            data.Date = start.AddDays(-1);
            data.Value = 2;
            funddata.Add(data);
            
            // No Data on start day, a day is skipped
            data = new FundData();
            data.FundEntity = fund;
            data.FundEntityId = fund.Id; 
            data.Date = start.AddDays(1);
            data.Value = 3;
            funddata.Add(data);

            data = new FundData();
            data.FundEntity = fund;
            data.Date = start.AddDays(2);
            data.Value = 4;
            funddata.Add(data);

            // Data ends after end date
            data = new FundData();
            data.FundEntity = fund;
            data.Date = start.AddDays(4);
            data.Value = 5;
            funddata.Add(data);

            fund.FundData = funddata;

            result.Add(fund);

            /* set up fund 2 */
            fund = new FundEntity();
            fund.Id = "fund2test";
            fund.Name = "test fund 2";
            funddata = new List<FundData>();

            // Data starts after start date
            data = new FundData();
            data.FundEntity = fund;
            data.FundEntityId = fund.Id;
            data.Date = start.AddDays(2);
            data.Value = 1;
            funddata.Add(data);

            // Data ends after end date
            data = new FundData();
            data.FundEntity = fund;
            data.Date = start.AddDays(3);
            data.Value = 2;
            funddata.Add(data);

            fund.FundData = funddata;

            result.Add(fund);

            return result;
        }

        private String formatDate(DateTime date)
        {
            return date.Date.Year + "/" + date.Date.Month + "/" + date.Date.Day;
        }
    }
}
