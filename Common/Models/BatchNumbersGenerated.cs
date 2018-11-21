using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class BatchNumbersGenerated
    {
        [Required]
        public Guid OrderId { get; set; }

        [Range(minimum: 1, maximum: 10, ErrorMessage = "Invalid Range, Values must be between 1-10")]
        public string BatchId { get; set; }

        public int MultiplicationNumber { get; set; }

        public BatchNumbersGenerated()
        {
        }

        public BatchNumbersGenerated(Guid orderId, string batchId, int multiplicationNumber)
        {
            OrderId = orderId;
            BatchId = batchId;
            MultiplicationNumber = multiplicationNumber;
        }
    }
}
