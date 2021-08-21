namespace Domain.Models.ProductModels
{
    public class Photo : BaseModel
    {
        public string PictureUrl { get; set; }
        public string FileName { get; set; }
        public bool IsMain { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}