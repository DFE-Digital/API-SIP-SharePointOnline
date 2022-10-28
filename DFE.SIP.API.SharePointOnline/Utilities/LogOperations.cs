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
        private TelemetryClient telemetryClient;
        private StringBuilder logTrail = new StringBuilder();
        private const string FUNCTION = "Function";
        private AppSettingsManager AppSettings;





        public LogOperations() { }

        public LogOperations(AppSettingsManager  AppSettings)
        {

            this.AppSettings = AppSettings;



            //TelemetryConfiguration.Active.InstrumentationKey = "701cb5ad-8a37-49b3-ad10-ea1c66fb9dac";
            
            //var telemetryClient = new TelemetryClient();
            //telemetryClient.TrackTrace("ConsoleShareOiunt  app- Trace Test COnstructor");
            //telemetryClient.TrackException(new Exception("API ShareOiunt app- COnstructor  "));
            //telemetryClient.Flush();

            var config = new TelemetryConfiguration();
            config.ConnectionString = $"InstrumentationKey={this.AppSettings.Get(this.AppSettings.APPINSIGHTS_KEY)}";

            telemetryClient = new TelemetryClient(config);
            telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Environment, this.AppSettings.Get(this.AppSettings.Environment));
            telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Application, "SIP API SharePoint Online");
            telemetryClient.Context.GlobalProperties.Add(InsightMetrics.Function, "Api Request");
            telemetryClient.Context.GlobalProperties.Add(InsightMetrics.CorrelationId, Guid.NewGuid().ToString());

            //  _telemetry.TrackException(new Exception("Test Exception"));
            //  _telemetry.TrackTrace("Hello World4 - from SP Api!");
            //  _telemetry.Flush();



            //TelemetryConfiguration.Active.InstrumentationKey = "701cb5ad-8a37-49b3-ad10-ea1c66fb9dac";
            ////TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            //var _telemetry = new TelemetryClient();
            //_telemetry.TrackTrace("Hello World5!");
            //_telemetry.Flush();


#if DEBUG
            if (TelemetryConfiguration.Active.TelemetryChannel != null)
                TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif

        }


        public string GetCorrelationID()
        {

            if (telemetryClient != null)
                return telemetryClient.Context.GlobalProperties[InsightMetrics.CorrelationId];

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


            

            telemetryClient.TrackEvent(eventName, dictionary, metrics?.ToDictionary(m => m.key, m => m.value));
        }

        public void LogEvent(string eventName, string description, IEnumerable<(string key, string value)> properties = null, IEnumerable<(string key, double value)> metrics = null)
        {
            var dictionary = properties?.ToDictionary(p => p.key, p => p.value.Substring(0, p.value.Length > 8000 ? 8000 : p.value.Length)) ?? new Dictionary<string, string>();
            dictionary["Description"] = description;

           
            telemetryClient.TrackEvent(eventName, dictionary, metrics?.ToDictionary(m => m.key, m => m.value));
        }



        public void LogTrace(string message, SeverityLevel level, IEnumerable<(string key, string value)> properties = null)
        {
            var dictionary = properties?.ToDictionary(p => p.key, p => p.value.Substring(0, p.value.Length > 8000 ? 8000 : p.value.Length)) ?? new Dictionary<string, string>();

                                   telemetryClient.TrackTrace(message, level, dictionary);
        }

        public void LogException(Exception ex, IEnumerable<(string key, string value)> properties = null, IEnumerable<(string key, double value)> metrics = null)
        {

            StringBuilder errorMsg = new StringBuilder();
            if (logTrail.Length > 0)
                errorMsg.Append(logTrail);

            errorMsg.Append(ex.Message);

            var dictionary = properties?.ToDictionary(p => p.key, p => p.value.Substring(0, p.value.Length > 8000 ? 8000 : p.value.Length)) ?? new Dictionary<string, string>();
            dictionary["ExceptionMessage"] = errorMsg.ToString().Substring(0, errorMsg.Length > 8000 ? 8000 : errorMsg.Length);

            telemetryClient.TrackException(ex, dictionary, metrics?.ToDictionary(m => m.key, m => m.value));
        }




    }
}