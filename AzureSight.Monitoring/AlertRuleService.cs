using AzureSight.Monitoring.Abstracts;
using AzureSight.Monitoring.Models;
using Microsoft.Azure.Management.AlertsManagement;
using Microsoft.Azure.Management.Monitor;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSight.Monitoring
{
    public class AlertRuleService : IAlertRuleService
    {

        public async Task<IEnumerable<AlertRule>> ListAlertRulesAsync(string accessToken, string subscriptionID)
        {
            var lstResults = new List<AlertRule>();
            ServiceClientCredentials serviceClientCreds = new TokenCredentials(accessToken);
            AlertsManagementClient cl = new AlertsManagementClient(serviceClientCreds) { SubscriptionId = subscriptionID };
            var alerts = await cl.Alerts.GetAllAsync();
            do
            {
                foreach (var item in alerts)
                {
                    Console.WriteLine(item.Name);
                }
                if (alerts.NextPageLink != null) alerts = await cl.Alerts.GetAllNextAsync(alerts.NextPageLink);
            } while (alerts.NextPageLink != null);



            MonitorManagementClient clMon = new MonitorManagementClient(serviceClientCreds) { SubscriptionId = subscriptionID };
            var rr = await clMon.ScheduledQueryRules.ListBySubscriptionAsync();
            var rules = await clMon.MetricAlerts.ListBySubscriptionAsync();
            var rulesAL = await clMon.AlertRules.ListBySubscriptionAsync();


            return lstResults;
        }
    }
}
