using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace IntroAspNet.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Preview { get; set; }
        public string PreviewText { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }

    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Price).NotNull().InclusiveBetween(0, double.MaxValue);
        }
    }
}
