using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Multiplication
    {
        public Guid OrderId { get; set; }

        [Range(minimum: 1, maximum: 10, ErrorMessage = "Invalid Range, Values must be between 1-10")]
        public string BatchId { get; set; }

        public int MultiplicationNumber { get; set; }

        public int FinalNumber { get; set; }

        public Multiplication(Guid id, string batchId, int multiplicationNumber, int finalNumber)
        {
            OrderId = id;
            BatchId = batchId;
            MultiplicationNumber = multiplicationNumber;
            FinalNumber = finalNumber;
        }

        public Multiplication()
        {
        }
    }
}
