using System.ComponentModel.DataAnnotations;

namespace TradeProject.Model.DBO
{
    public class Product
    {
        [Key]
        public string? ArticleNumber { get; set; }

        public string? Name { get; set; }

        public decimal? Cost { get; set; }

        public string? Manufacturer { get; set; }
        public int? QuantityInStock { get; set; }

        public string? Description { get; set; }

        public string? Photo { get; set; }

        public virtual Category Category { get; set; } = null!;
    }
}
