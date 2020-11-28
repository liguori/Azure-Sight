using AzureSight.Monitoring.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSight.Monitoring.Abstracts
{
    public interface IAlertRuleService
    {
        Task<IEnumerable<AlertRule>> ListAlertRulesAsync(string accessToken, string subscriptionID);
    }
}
