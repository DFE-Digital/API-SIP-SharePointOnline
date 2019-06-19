using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DFE.SIP.API.SharePointOnline.Utilities
{
    public static class InsightMetrics
    {
        public const string CorrelationId = "CorrelationId";
        public const string Application = "Application";
        public const string Desc = "Description";
        public const string Environment = "Environment";
        public const string Function = "Function";
    }



    public class LogOperations
    {
        private TelemetryClient _telemetry;
        private StringBuilder logTrail = new StringBuilder();
        private const string FUNCTION = "Function";
        private AppSettingsManager AppSettings;





        public LogOperations() { }

        public LogOperations(AppSettingsManager  AppSettings)
        {

            this.AppSettings = AppSettings;
            _telemetry = new TelemetryClient();
            TelemetryConfiguration.Active.InstrumentationKey = this.AppSettings.APPINSIGHTS_KEY;
            _telemetry.Context.GlobalProperties.Add(InsightMetrics.Environment, this.AppSettings.Environment);
            _telemetry.Context.GlobalProperties.Add(InsightMetrics.Application, "Apply to Convert");
            _telemetry.Context.GlobalProperties.Add(InsightMetrics.Function, "Api Request");
            _telemetry.Context.GlobalProperties.Add(InsightMetrics.CorrelationId, Guid.NewGuid().ToString());

#if DEBUG
            if (TelemetryConfiguration.Active.TelemetryChannel != null)
                TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif

        }


        public string GetCorrelationID()
        {

            if (_telemetry != null)
                return _telemetry.Context.GlobalProperties[InsightMetrics.CorrelationId];

            return "";

        }

        public void LogTrailMessage(string msg)
        {
            if (logTrail.Length == 0)
                logTrail.Append("Trail: ");

            logTrail.Append($"{msg}>");

        }

        public void LogEvent(string eventName, IEnumerable<(string key, string value)> properties = null, IEnumerable<(string key, double value)> metrics = null)
        {
            var dictionary = properties?.ToDictionary(p => p.key, p => p.value.Substring(0, p.value.Length > 8000 ? 8000 : p.value.Length)) ?? new Dictionary<string, string>();
            _telemetry.TrackEvent(eventName, dictionary, metrics?.ToDictionary(m => m.key, m => m.value));
        }

        public void LogEvent(string eventName, string description, IEnumerable<(string key, string value)> properties = null, IEnumerable<(string key, double value)> metrics = null)
        {
            var dictionary = properties?.ToDictionary(p => p.key, p => p.value.Substring(0, p.value.Length > 8000 ? 8000 : p.value.Length)) ?? new Dictionary<string, string>();
            dictionary["Description"] = description;
            _telemetry.TrackEvent(eventName, dictionary, metrics?.ToDictionary(m => m.key, m => m.value));
        }



        public void LogTrace(string message, SeverityLevel level, IEnumerable<(string key, string value)> properties = null)
        {
            var dictionary = properties?.ToDictionary(p => p.key, p => p.value.Substring(0, p.value.Length > 8000 ? 8000 : p.value.Length)) ?? new Dictionary<string, string>();
            _telemetry.TrackTrace(message, level, dictionary);
        }

        public void LogException(Exception ex, IEnumerable<(string key, string value)> properties = null, IEnumerable<(string key, double value)> metrics = null)
        {

            StringBuilder errorMsg = new StringBuilder();
            if (logTrail.Length > 0)
                errorMsg.Append(logTrail);

            errorMsg.Append(ex.Message);

            var dictionary = properties?.ToDictionary(p => p.key, p => p.value.Substring(0, p.value.Length > 8000 ? 8000 : p.value.Length)) ?? new Dictionary<string, string>();
            dictionary["ExceptionMessage"] = errorMsg.ToString().Substring(0, errorMsg.Length > 8000 ? 8000 : errorMsg.Length);

            _telemetry.TrackException(ex, dictionary, metrics?.ToDictionary(m => m.key, m => m.value));
        }




    }
}