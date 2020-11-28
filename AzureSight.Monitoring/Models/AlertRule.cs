using System;
using System.Collections.Generic;
using System.Text;

namespace AzureSight.Monitoring.Models
{
    public class AlertRule
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Resource { get; set; }
        public string ResourceType { get; set; }
        public string SignalType { get; set; }
        public int Severity { get; set; }
        public double Threshold { get; set; }
        public double AggregationTime { get; set; }
        public double FrequencyTime { get; set; }
        public int Occurrencies { get; set; }
        public DateTime OccurencyFirstDate { get; set; }
        public DateTime OccurencyLastDate { get; set; }
        public string OccurrencyLastState { get; set; }
    }
}
