﻿using Microsoft.ApplicationInsights.Extensibility;
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
                var config = new TelemetryConfiguration();
                config.ConnectionString = $"InstrumentationKey={AppSettings.Get(AppSettings.APPINSIGHTS_KEY)}";

                _telemetryClient = new TelemetryClient(config);
                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Environment, AppSettings.Get(AppSettings.Environment));
                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Application, "SIP API SharePoint Online");
                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Function, "Api Request");
                _telemetryClient.Context.GlobalProperties.Add(InsightMetrics.CorrelationId, Guid.NewGuid().ToString());
            }

            return _telemetryClient;
        }
    }
}