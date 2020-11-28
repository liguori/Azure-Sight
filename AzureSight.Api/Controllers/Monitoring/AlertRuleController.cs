using AzureSight.Monitoring.Abstracts;
using AzureSight.Monitoring.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSight.Api.Controllers.Monitoring
{

    [ApiController]
    [Route("api/monitoring/[controller]")]
    public class AlertRuleController : ControllerBase
    {
        private readonly IAlertRuleService _alertRuleService;
        public AlertRuleController(IAlertRuleService alertRuleService)
        {
            this._alertRuleService = alertRuleService;
        }

        [HttpGet]
        public Task<IEnumerable<AlertRule>> GetAsync(string subscriptionID)
        {
            return this._alertRuleService.ListAlertRulesAsync(Request.Headers["AzureBearer"], subscriptionID);
        }
    }
}
