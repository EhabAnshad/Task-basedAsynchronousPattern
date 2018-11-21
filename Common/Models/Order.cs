using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Order: IModel
    {
        public Guid OrderId { get; set; }

        [Required]
        [Range(minimum: 1, maximum: 10, ErrorMessage = "Invalid Range, Values must be between 1-10")]
        public int BatchCount { get; set; }

        [Required]
        [Range(minimum: 1, maximum: 10, ErrorMessage = "Invalid Range, Values must be between 1-10")]
        public int NumberPerBatch { get; set; }

        public int TotalOrdersCount { get; set; }

        public List<Batch> CompletedPerBatch { get; set; }

        public int CompletedOrdersCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public Order()
        {
        }

        public Order(Guid orderId, int batchCount, int numberPerBatch)
        {
            OrderId = orderId;
            BatchCount = batchCount;
            NumberPerBatch = numberPerBatch;
        }

        public Order(Guid orderId, string batchId, int finalNumber)
        {
           
        }
    }
}
