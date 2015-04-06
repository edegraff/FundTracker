using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorGenerator.Testing;
using HtmlAgilityPack;
using FundPortfolio;
using FundPortfolio.Controllers;
using Common.Models;

namespace FundPortfolio.Tests.Controllers
{
    [TestClass]
    public class ReportControllerTest
    {
        private DatabaseContext db = new DatabaseContext();

        /*
         * 3.2.1.1.1 Visitor and Users will be able to select fund from the search to see the history of the performance. 
         * (This view displays the history of performance for the fund.)
         *
         * 3.2.1.5.1 Predictions about the future performance/value of funds will be available, using existing prediction algorithms.  
         * (This view will return prediction values by default.)
         */
        [TestMethod]
        public void ReportIndex()
        {
            // Arrange
            ReportController controller = new ReportController();
            String id = "476";
            FundEntity fund = db.Funds.Find(id);

            // Act
            ViewResult result = controller.Index(id) as ViewResult;
            Report rep = (Report) result.Model;

            // test headers
            Assert.IsTrue(rep.Headers[0].Equals("Date"));
            Assert.IsTrue(rep.Headers[1].Equals(fund.Name));

            // test all dates are there
            DateTime now = DateTime.Today.Date;
            DateTime earliest = now.AddMonths(-1);
            bool incompleteMonth = false;
            if (earliest < fund.getFirstDate())
            {
                earliest = fund.getFirstDate();
                incompleteMonth = true;
            }
            int past = now.Subtract(earliest).Days;
            int i = 0;
            for (int d = FundProjector.projectionLimit; d >= -past; d--)
            {
                Assert.IsTrue(rep.Data[0][i].Equals(formatDate(now.AddDays(d))));
                i++;
            }

            /* Check values */
            i = 0;
            // check that projection range is numbers
            for (int d = FundProjector.projectionLimit; d > 0; d--)
            {
                float.Parse(rep.Data[1][i]);
                i++;
            }

            // check non-prediction range
            for (int d = 0; d >= -past; d--)
            {
                Assert.IsTrue(float.Parse(rep.Data[1][i]) == fund.GetValueByDate(now.AddDays(d)));
                i++;
            }

            if (incompleteMonth)
            {
                Assert.IsTrue(rep.Data[1][i].Equals("Earlier Data Unavailable"));
                i++;
            }

            // Ensure we checked all the data
            Assert.IsTrue(i == rep.Data[0].Count);
        }


        /*
         * 3.2.1.1.2 Visitors and Users will be able to compare mutual funds side by side. 
         * (This view accepts multiple ids of funds that will be compared in the returned view with (optionally) a graph and tabular data)
         * 
         * 3.2.1.5.1 Predictions about the future performance/value of funds will be available, using existing prediction algorithms.  
         * (If users selected a date into the future when creating the report predictions will be shown.)
         */
        [TestMethod]
        public void ReportCustomReport()
        {
            // Arrange
            ReportController controller = new ReportController();
            String id1 = "475";
            String id2 = "476";
            FundEntity fund1 = db.Funds.Find(id1);
            FundEntity fund2 = db.Funds.Find(id2);
            DateTime start = fund1.getFirstDate();
            if (start > fund2.getFirstDate())
            {
                start = fund2.getFirstDate();
            }
            start = start.Date;
            DateTime end = DateTime.Today.Date.AddDays(FundProjector.projectionLimit + 1);

            // Act
            ViewResult result = controller.CustomReport(start, end, id1 + "," + id2, true) as ViewResult;
            Report rep = (Report) result.Model;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewBag.Graph = true);
            
            // test headers
            Assert.IsTrue(rep.Headers[0].Equals("Date"));
            Assert.IsTrue(rep.Headers[1].Equals(fund1.Name));
            Assert.IsTrue(rep.Headers[2].Equals(fund2.Name));

            // test all dates are there
            DateTime now = DateTime.Today.Date;
            int i = 0;
            int range = end.Subtract(start).Days;
            for (int d = FundProjector.projectionLimit +1; d >= FundProjector.projectionLimit +1 - range; d--)
            {
                Assert.IsTrue(rep.Data[0][i].Equals(formatDate(now.AddDays(d))));
                i++;
            }

            // Ensure we checked all the data
            Assert.IsTrue(i == rep.Data[0].Count);

            /* Check values */

            /* fund 1 */
            i = 0;
            Assert.IsTrue(rep.Data[1][i].Equals("Further Projection Unavailable"));
            i++;

            // check that projection range is numbers
            for (int d = FundProjector.projectionLimit; d > 0; d--)
            {
                float.Parse(rep.Data[1][i]);
                i++;
            }

            // check non-prediction range
            range = now.Subtract(fund1.getFirstDate().Date).Days;
            for (int d = 0; d >= -range; d--)
            {
                Assert.IsTrue(float.Parse(rep.Data[1][i]) == fund1.GetValueByDate(now.AddDays(d)));
                i++;
            }

            // dates earlier than we have data for.
            while (i < rep.Data[0].Count)
            {
                Assert.IsTrue(rep.Data[1][i].Equals("Earlier Data Unavailable"));
                i++;
            }

            /* fund 2 */
            i = 0;
            Assert.IsTrue(rep.Data[2][i].Equals("Further Projection Unavailable"));
            i++;

            // check that projection range is numbers
            for (int d = FundProjector.projectionLimit; d > 0; d--)
            {
                float.Parse(rep.Data[2][i]);
                i++;
            }

            // check non-prediction range
            range = now.Subtract(fund2.getFirstDate().Date).Days;
            for (int d = 0; d >= -range; d--)
            {
                Assert.IsTrue(float.Parse(rep.Data[2][i]) == fund2.GetValueByDate(now.AddDays(d)));
                i++;
            }

            // dates earlier than we have data for.
            while (i < rep.Data[0].Count)
            {
                Assert.IsTrue(rep.Data[2][i].Equals("Earlier Data Unavailable"));
                i++;
            }

        }

        private void testExport()
        {

        }

        private String formatDate(DateTime date)
        {
            return date.Date.Year + "/" + date.Date.Month + "/" + date.Date.Day;
        }

    }
}
