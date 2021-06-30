using System.ComponentModel.DataAnnotations;

namespace IntroAspNet.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Preview { get; set; }
        public string PreviewText { get; set; }
        public string Description { get; set; }
        [Required, Range(0, double.MaxValue)]
        public double Price { get; set; }
        
        public virtual Category Category { get; set; }
    }
}
