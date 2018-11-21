using System;
using Newtonsoft.Json;

namespace WebApp.Models
{
    public class Batch
    {
        public Guid OrderId { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }

        public Guid BatchId { get; set; }

        public string Key { get; set; }

        public int Value { get; set; }

        public bool IsCompleted { get; set; }


        public Batch()
        {
            IsCompleted = false;
        }

        public Batch(string key, int value)
        {
            Key = key;
            Value = value;
        }
    }
}
