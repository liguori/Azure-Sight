using AzureSight.Core.Abstracts;
using AzureSight.Monitoring.Abstracts;
using AzureSight.Monitoring.Models;
using Microsoft.Azure.Management.AlertsManagement;
using Microsoft.Azure.Management.Monitor;
using Microsoft.Azure.Management.Monitor.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;

namespace AzureSight.Monitoring
{
    public class AlertRuleService : IAlertRuleService
    {
        MonitorManagementClient clMonitor;
        AlertsManagementClient clAlerts;
        private readonly IAzureResourceUtils _azureResourceUtils;
        public AlertRuleService(IAzureResourceUtils azureResourceUtils)
        {
            _azureResourceUtils = azureResourceUtils;
        }


        private async Task SetAlertOccurrencies(AlertRule current, string alertRuleID)
        {
            bool bugAllAlertRetrieved = false;
            var alertsTriggered = await clAlerts.Alerts.GetAllAsync(alertRule: alertRuleID);
            if (alertsTriggered.Count() == 0)
            {
                bugAllAlertRetrieved = true;
                //NOTE: There is a "bug", the alertRule filter may want the alert rule ID in the URL Encoded format (especially the metric alert with spaces in the name) and may not return all the results because are returned with different cases, so all the alert are retrieved and filtered locally
                alertsTriggered = await clAlerts.Alerts.GetAllAsync();
            }
            do
            {
                foreach (var item in alertsTriggered)
                {
                    if (bugAllAlertRetrieved)
                    {
                        var resourceNameStartIndex = alertRuleID.LastIndexOf("/") + 1;
                        var currentAlertOccurrenceRuleToFind = alertRuleID.Substring(0, resourceNameStartIndex) + Uri.EscapeUriString(alertRuleID.Substring(resourceNameStartIndex));
                        if (item.Properties.Essentials.AlertRule.ToLower() != currentAlertOccurrenceRuleToFind.ToLower()) continue;
                    }

                    current.Occurrencies++;
                    if (item.Properties.Essentials.StartDateTime.HasValue && current.OccurencyFirstDate == null ||
                        item.Properties.Essentials.StartDateTime < current.OccurencyFirstDate)
                    {
                        current.OccurencyFirstDate = item.Properties.Essentials.StartDateTime;
                    }
                    if (item.Properties.Essentials.StartDateTime.HasValue && current.OccurencyLastDate == null ||
                        item.Properties.Essentials.StartDateTime > current.OccurencyLastDate)
                    {
                        current.OccurencyLastDate = item.Properties.Essentials.StartDateTime;
                        current.OccurrencyLastCondition = item.Properties.Essentials.MonitorCondition;
                        current.OccurencyLastDateResolved = item.Properties.Essentials.MonitorConditionResolvedDateTime;
                    }
                }
                if (alertsTriggered.NextPageLink != null) alertsTriggered = await clAlerts.Alerts.GetAllNextAsync(alertsTriggered.NextPageLink);
            } while (alertsTriggered.NextPageLink != null);
        }


        public async Task<IEnumerable<AlertRule>> ListAlertRulesAsync(string accessToken, string subscriptionID)
        {
            var lstResults = new List<AlertRule>();
            ServiceClientCredentials serviceClientCreds = new TokenCredentials(accessToken);
            clMonitor = new MonitorManagementClient(serviceClientCreds) { SubscriptionId = subscriptionID };
            clAlerts = new AlertsManagementClient(serviceClientCreds) { SubscriptionId = subscriptionID };
            var scheduledQueryAlerts = await clMonitor.ScheduledQueryRules.ListBySubscriptionAsync();
            foreach (var item in scheduledQueryAlerts)
            {
                var rule = new AlertRule();
                rule.Name = item.Name;
                rule.Severity = (item.Action as AlertingAction).Severity;
                rule.FrequencyTimeMinutes = item.Schedule.FrequencyInMinutes;
                rule.AggregationTimeMinutes = item.Schedule.TimeWindowInMinutes;
                rule.Enabled = item.Enabled;

                var resourceInfo = _azureResourceUtils.GetInfoFromResourceID(item.Source.DataSourceId);

                rule.Threshold = (item.Action as AlertingAction).Trigger.Threshold.ToString();
                rule.Resource = resourceInfo.Name;
                rule.ResourceType = resourceInfo.ResourceType;
                rule.SignalType = "Log search";

                await SetAlertOccurrencies(rule, item.Id);

                lstResults.Add(rule);
            }
            scheduledQueryAlerts = null;


            var metricAlerts = await clMonitor.MetricAlerts.ListBySubscriptionAsync();
            foreach (var item in metricAlerts)
            {
                var rule = new AlertRule();
                rule.Name = item.Name;
                rule.Severity = item.Severity.ToString();
                rule.FrequencyTimeMinutes = item.EvaluationFrequency.Minutes;
                rule.AggregationTimeMinutes = item.WindowSize.Minutes;
                rule.Enabled = item.Enabled.ToString();

                var resourceInfo = _azureResourceUtils.GetInfoFromResourceID(item.Scopes.First());

                if (item.Criteria is MetricAlertMultipleResourceMultipleMetricCriteria)
                {
                    rule.Threshold = string.Join(",", (item.Criteria as MetricAlertMultipleResourceMultipleMetricCriteria).AllOf.Select(x => x.Name.ToString()));
                }
                else if (item.Criteria is MetricAlertSingleResourceMultipleMetricCriteria)
                {
                    rule.Threshold = string.Join(",", (item.Criteria as MetricAlertSingleResourceMultipleMetricCriteria).AllOf.Select(x => x.Threshold.ToString()));
                }

                rule.Resource = resourceInfo.Name;
                rule.ResourceType = item.TargetResourceType;
                rule.SignalType = "Metrics";

                await SetAlertOccurrencies(rule, item.Id);

                lstResults.Add(rule);
            }
            metricAlerts = null;

            var classicAlerts = await clMonitor.AlertRules.ListBySubscriptionAsync();
            var activityLogAlerts = await clMonitor.ActivityLogAlerts.ListBySubscriptionIdAsync();

            return lstResults;
        }
    }
}
