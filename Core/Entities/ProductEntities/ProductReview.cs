using Core.Entities.ProductEntities;

namespace Core.Entities
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}