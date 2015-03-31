using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace Common.Models
{
    public class Report
    {
        // Initialize the Report object
		public Report(DateTime start, DateTime end, IEnumerable<ITimeSeriesFundData> funds)
        {
            /* Initializations */
            start = start.Date;
            end = end.Date;
            Headers = new List<String>(); 
            Data = new List<String>[funds.Count() + 1];
            DateTime nowDate = DateTime.Now.Date;
            List<float>[] tempData = new List<float>[funds.Count()];

            Headers.Add("Date");
            Data[0] = new List<String>();
            for (int i = 0; i < funds.Count(); i++)
            {
                Headers.Add(funds.ElementAt(i).Name);
                Data[i+1] = new List<String>();
                tempData[i] = funds.ElementAt(i).GetDataInRange(start, end, nowDate).Item1;
            }

            int dataLen = tempData[0].Count; // how many eatries in each list in tempData

            /* Section 1 - start data NaN removal/replacement */
            bool previousEmpty = false;
            bool currentEmpty;
            int j = 0;
            while (j < dataLen)
            {
                currentEmpty = true;
                for (int i = 0; i < funds.Count(); i++)
                {
                    if (!float.IsNaN(tempData[i][j]))
                    {
                        currentEmpty = false;
                    }
                }

                if (currentEmpty)
                {
                    previousEmpty = true;
                }
                else
                {
                    if (previousEmpty)
                    {
                        Data[0].Add(formatDate(start.AddDays(j-1)));
                        for (int i = 0; i < funds.Count(); i++)
                        {
                            Data[i + 1].Add("Earlier Data Unavailable");
                        }
                    }
                    break;
                }
                j++;
            }

            start = start.AddDays(j);


            /* Section 2 - Data up to the end or nowDate */
            while (j < dataLen && start <= nowDate && start <= end)
            {
                Data[0].Add(formatDate(start));
                for (int i = 0; i < funds.Count(); i++)
                {
                    if (float.IsNaN(tempData[i][j]))
                    {
                        Data[i + 1].Add("Earlier Data Unavailable");
                    }
                    else
                    {
                        Data[i + 1].Add(tempData[i][j].ToString());
                    }
                }
                j++;
                start = start.AddDays(1);
            }

            /* Section 3 - any prediction data */
            if (nowDate < end)
            {
                // Initialize the projections
                FundProjector[] prjs = new FundProjector[funds.Count()];
                for (int i = 0; i < funds.Count(); i++)
                {
                    prjs[i] = new FundProjector(funds.ElementAt(i), nowDate);
                }

                nowDate = nowDate.AddDays(FundProjector.projectionLimit); // now the limit of projection
                j = 0;
                while (j < FundProjector.projectionLimit && start <= nowDate && start <= end)
                {
                    Data[0].Add(formatDate(start));
                    for (int i = 0; i < funds.Count(); i++)
                    {
                        Data[i + 1].Add(prjs[i].Projection[j].ToString());
                    }
                    j++;
                    start = start.AddDays(1);
                }
            }
            
            /* Section 4 - Data out of projection range */
            if (start <= end)
            {
                Data[0].Add(formatDate(start));
                for (int i = 0; i < funds.Count(); i++)
                {
                    Data[i + 1].Add("Further Projection Unavailable");
                }
                start = start.AddDays(1);
            }

            /* Section 5 - reverse order, so newest first */
            for (int i = 0; i < funds.Count()+1; i++)
            {
                Data[i].Reverse();
            }
        }

        [NotMapped]
        public List<String> Headers { get; set; }

        [NotMapped]
        public List<String>[] Data { get; set; }

        private String formatDate(DateTime date)
        {
            return date.Date.Year + "/" + date.Date.Month + "/" + date.Date.Day;
        }
    }
}
