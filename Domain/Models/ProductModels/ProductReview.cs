namespace Domain.Models.ProductModels
{
    public class Review : BaseModel
    {
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}