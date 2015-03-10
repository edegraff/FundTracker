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
            Dates = new List<DateTime>();
            FundNames = new List<String>(); 
            FundsData = new List<String>[funds.Count];


            for (int i = 0; i < funds.Count; i++)
            {
                FundNames.Add(funds[i].name);
                FundsData[i] = new List<String>();
            }

            System.TimeSpan days = end.Subtract(start);
            DateTime date = start;
            Boolean includeDate;
            float val;

            for (int i = 0; i <= days.Days; i++)
            {
                includeDate = false;
                for (int f = 0; f < funds.Count; f++)
                {
                    try
                    {
                        // Try to add the value from the date
                        val = funds[f].GetValueByDate(date);
                        FundsData[f].Add(val.ToString());
                        includeDate = true;
                    }
                    catch (InvalidOperationException e)
                    {
                        // If no value found add "-" instead
                        FundsData[f].Add("-");
                    }
                }

                if (includeDate) // There are valid entries at date so include it
                {
                    Dates.Add(date);
                }
                else // Remove the empty entries
                {
                    for (int f = 0; f < funds.Count; f++)
                    {
                        FundsData[f].RemoveAt(FundsData[f].Count - 1);
                    }
                }
                date = date.AddDays(1);
            }
        }

        public List<String> FundNames { get; set; }
        
        public List<DateTime> Dates { get; set; }
        public List<String>[] FundsData { get; set; }
    }
}
