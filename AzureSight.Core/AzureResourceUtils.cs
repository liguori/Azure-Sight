using AzureSight.Core.Abstracts;
using AzureSight.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureSight.Core
{
    public class AzureResourceUtils : IAzureResourceUtils
    {
        public ResourceInfo GetInfoFromResourceID(string resourceID)
        {
            var resourceMatch = Regex.Match(resourceID.ToLower(), @"\/subscriptions\/(?<subscriptionID>(?s)(.*))\/resourcegroups\/(?<ResourceGroup>(?s)(.*))\/providers\/(?<resourceType>(?s)(.*))\/(?<resourceName>(?s)(.*))");
            if (resourceMatch.Success)
            {
                return new ResourceInfo
                {
                    SubscriptionID = resourceMatch.Groups["subscriptionID"].Value,
                    ResourceGroup = resourceMatch.Groups["ResourceGroup"].Value,
                    Name = resourceMatch.Groups["resourceName"].Value,
                    ResourceType = resourceMatch.Groups["resourceType"].Value
                };

            }
            return null;
        }

    }
}
