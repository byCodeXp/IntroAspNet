using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace IntroAspNet.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Preview { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }

    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotNull();
        }
    }
}