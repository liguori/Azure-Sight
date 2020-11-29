using System;
using System.Collections.Generic;
using System.Text;

namespace AzureSight.Monitoring.Models
{
    public class AlertRule
    {
        public string Name { get; set; }
        public string Enabled { get; set; }
        public string Resource { get; set; }
        public string ResourceType { get; set; }
        public string SignalType { get; set; }
        public string Severity { get; set; }
        public string Threshold { get; set; }
        public int AggregationTimeMinutes { get; set; }
        public int FrequencyTimeMinutes { get; set; }
        public int Occurrencies { get; set; }
        public DateTime? OccurencyFirstDate { get; set; }
        public DateTime? OccurencyLastDate { get; set; }
        public DateTime? OccurencyLastDateResolved { get; set; }
        public string OccurrencyLastCondition { get; set; }
    }
}
