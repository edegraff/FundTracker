using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace Common.Models
{
    public class Report
    {
        private DatabaseContext db = new DatabaseContext();

        // Initialize the Report object
        public Report(DateTime start, DateTime end, List<FundEntity> funds)
        {
            /* Initializations */
            start = start.Date;
            end = end.Date;
            Dates = new List<DateTime>();
            FundNames = new List<String>(); 
            FundsData = new List<String>[funds.Count];


            for (int i = 0; i < funds.Count; i++)
            {
                FundNames.Add(funds[i].name);
                FundsData[i] = new List<String>();
            }

            int daySpan;
            DateTime nowDate = DateTime.Now.Date;
            DateTime date;
            Boolean endOfData;
            float val;

            /* Section 1, prediction values */
            daySpan = end.Subtract(nowDate).Days;
            date = end;

            // Will we have a prediction portion?
            if (daySpan > 0)
            {
                // Initialize the projections
                List<FundProjector> projections = new List<FundProjector>();
                foreach (FundEntity fund in funds)
                {
                    projections.Add(new FundProjector(fund));
                }

                // Check if requested dates go beyond our prediction limit
                if (daySpan > FundProjector.projectionLimit)
                {
                    // Set date to the first day beyond the prediction range
                    date = date.AddDays(FundProjector.projectionLimit - daySpan + 1);
                    Dates.Add(date);
                    for (int f = 0; f < funds.Count; f++)
                    {
                        // Label the end of projection
                        FundsData[f].Add("Projection Unavailable");
                    }
                    date = date.AddDays(-1);
                }

                // Set the daySpan correctly
                if (nowDate > start.Date)
                {
                    daySpan = date.Subtract(nowDate).Days;
                }
                else 
                {
                    daySpan = date.Subtract(start.Date).Days + 1;
                }
                
                // Fill in any prediction values
                for (int i = 0; i < daySpan; i++)
                {
                    Dates.Add(date);
                    for (int f = 0; f < funds.Count; f++)
                    {
                        // add the prediction value for each fund
                        FundsData[f].Add(projections[f].getPredictionByDate(date).ToString());
                    }
                    date = date.AddDays(-1);
                }

                // End if the start date was in the prediction range
                if (start > nowDate)
                {
                    return;
                }
            }

            /* Section 2, past values */
            daySpan = date.Subtract(start).Days;

            for (int i = 0; i <= daySpan; i++)
            {
                Dates.Add(date);
                endOfData = true;
                for (int f = 0; f < funds.Count; f++)
                {
                    try
                    {
                        // Try to add the value from the date
                        val = funds[f].GetValueByDate(date);
                        FundsData[f].Add(val.ToString());
                        endOfData = false;
                    }
                    catch (InvalidOperationException e)
                    {
                        // If no value found add End of Data message instead
                        FundsData[f].Add("Earlier Data Unavailable");
                    }
                }

                if (endOfData) // There are no more valid entries
                {
                    break;
                }
                date = date.AddDays(-1);
            }
        }

        public List<String> FundNames { get; set; }
        
        public List<DateTime> Dates { get; set; }
        public List<String>[] FundsData { get; set; }
    }
}
