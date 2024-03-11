using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.IdentityModel.Abstractions;
using System;

namespace DFE.SIP.API.SharePointOnline.Utilities
{
    public static class TelemetryFactory
    {
        private static TelemetryClient _telemetryClient;

        public static TelemetryClient GetTelemetryClient(AppSettingsManager AppSettings)
        {
            if (_telemetryClient == null)
            {
                // Overrides the ApplicationInsights.config connection string
                TelemetryConfiguration.Active.ConnectionString = AppSettings.Get(AppSettings.ApplicationInsightsConnectionString);

                _telemetryClient = new TelemetryClient();

                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Environment, AppSettings.Get(AppSettings.Environment));
                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Application, "SIP API SharePoint Online");
                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Function, "Api Request");
                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.CorrelationId, Guid.NewGuid().ToString());
            }

            return _telemetryClient;
        }
    }
}