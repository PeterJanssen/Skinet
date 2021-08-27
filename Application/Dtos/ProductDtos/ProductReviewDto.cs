using System;

namespace Application.Dtos.ProductDtos
{
    public class ProductReviewDto
    {
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public string ReviewerName { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}