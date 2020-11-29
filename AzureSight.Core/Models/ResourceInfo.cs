using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSight.Core.Models
{
    public class ResourceInfo
    {
        public string Name { get; set; }

        public string ResourceType { get; set; }

        public string ResourceGroup { get; set; }

        public string SubscriptionID { get; set; }
    }
}
