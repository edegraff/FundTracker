using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Models;

namespace FundPortfolio.Tests
{
    [TestClass]
    public class TestProjector
    {
        /*
         * 3.2.1.5.1 Predictions about the future performance/value of funds will be available, using existing prediction algorithms.
         * The projectror object performs predictions on the passed in FundEntity.  The results reside in the projection property.
         */
        [TestMethod]
        public void ProjectorObjectInit()
        {
            // Stub
            DateTime now = DateTime.Today.Date;
            DateTime start = now.AddDays(-5);

            ITimeSeriesFundData fund = createStub(start);

            FundProjector proj = new FundProjector(fund, now);

            // test headers
            Assert.IsTrue(proj.Projection.Count == FundProjector.projectionLimit);

            // Ensure prediction is within error bounds
            double error = 0.00005;
            Assert.IsTrue(Math.Abs(proj.Projection[0] - 4.65) < error);
            Assert.IsTrue(Math.Abs(proj.Projection[1] - 4.416666) < error);
            Assert.IsTrue(Math.Abs(proj.Projection[2] - 4.270833) < error);
        }

        /*
         * Initializes some fake data.
         * fund1 - name = "test fund"
         *       - id = "fundtest"
         *       - data
         *          - start+0 : 2
         *          - start+1 : 3
         *          - start+2 : 4
         *          - start+4 : 5
         */
        private ITimeSeriesFundData createStub(DateTime start)
        {
            FundEntity fund;
            FundData data;
            List<FundData> funddata;

            /* set up fund 1 */
            fund = new FundEntity();
            fund.Id = "fundtest";
            fund.Name = "test fund";
            funddata = new List<FundData>();

            // Data starts before start date
            data = new FundData();
            data.FundEntity = fund;
            data.FundEntityId = fund.Id;
            data.Date = start;
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

            return fund;
        }

        private String formatDate(DateTime date)
        {
            return date.Date.Year + "/" + date.Date.Month + "/" + date.Date.Day;
        }
    }
}
