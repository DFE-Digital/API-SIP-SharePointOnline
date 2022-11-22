using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Sentry;

namespace DFE.SIP.API.SharePointOnline.Utilities
{
    public class LogOperations
    {
        private TelemetryClient telemetryClient;
        private StringBuilder logTrail = new StringBuilder();
        private readonly Guid correlationID;

        public LogOperations()
        {
            this.correlationID = Guid.NewGuid();
        }

        public string GetCorrelationId()
        {

            return this.correlationID.ToString();

        }

        public void LogInformation(string info)
        {
            SentrySdk.CaptureMessage(info);
        }

        public void LogException(Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

    }
}