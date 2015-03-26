using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace FundService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private Scraper scraper;
        private Notifier notifier;

        public override void Run()
        {
            while(true)
            { 
                Trace.TraceInformation("Running");
                this.scraper.Scrape();
                //this.notifier.Notify();
                //Wait an hour
                Thread.Sleep(3600000);
            };
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("FundService has been started");
            this.scraper = new Scraper("https://www.cibc.com/ca/rates/mutual-fund-rates.html");
            this.notifier = new Notifier();

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("FundService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("FundService has stopped");
        }
    }
}
