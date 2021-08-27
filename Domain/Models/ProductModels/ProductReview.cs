using System;

namespace Domain.Models.ProductModels
{
    public class Review : BaseModel
    {
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public string ReviewerName { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}