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
        // Initialize the Report object
        public Report(DateTime start, DateTime end, List<FundEntity> funds)
        {
            /* Initializations */
            start = start.Date;
            end = end.Date;
            Headers = new List<String>(); 
            Data = new List<String>[funds.Count + 1];


            Headers.Add("Date");
            Data[0] = new List<String>();
            for (int i = 0; i < funds.Count; i++)
            {
                Headers.Add(funds[i].Name);
                Data[i+1] = new List<String>();
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
                    Data[0].Add(formatDate(date));
                    for (int f = 0; f < funds.Count; f++)
                    {
                        // Label the end of projection
                        Data[f+1].Add("Projection Unavailable");
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
                    Data[0].Add(formatDate(date));
                    for (int f = 0; f < funds.Count; f++)
                    {
                        // add the prediction value for each fund
                        Data[f+1].Add(projections[f].getPredictionByDate(date).ToString());
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
                Data[0].Add(formatDate(date));
                endOfData = true;
                for (int f = 0; f < funds.Count; f++)
                {
                    try
                    {
                        // Try to add the value from the date
                        val = funds[f].GetValueByDate(date);
                        Data[f+1].Add(val.ToString());
                        endOfData = false;
                    }
                    catch (InvalidOperationException e)
                    {
                        // If no value found add End of Data message instead
                        Data[f+1].Add("Earlier Data Unavailable");
                    }
                }

                if (endOfData) // There are no more valid entries
                {
                    break;
                }
                date = date.AddDays(-1);
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
