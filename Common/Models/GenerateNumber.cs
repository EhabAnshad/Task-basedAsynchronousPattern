using System;

namespace Common.Models
{
    public class GenerateNumber
    {
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }

        public int BatchCount { get; set; }

        public GenerateNumber(Guid id, int batchCount)
        {
            OrderId = id;
            CreatedAt = DateTime.UtcNow;
            BatchCount = batchCount;
        }
    }
}
