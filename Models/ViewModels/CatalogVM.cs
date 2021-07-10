using System.Collections.Generic;

namespace IntroAspNet.Models.ViewModels
{
    public class CatalogVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}