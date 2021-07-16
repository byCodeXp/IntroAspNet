using System.Collections.Generic;
using System.Linq;
using IntroAspNet.Data;
using IntroAspNet.Models;
using IntroAspNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntroAspNet.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public IActionResult Index()
        {
            List<Cart> carts = new ();

            var context = HttpContext.Session.Get<IEnumerable<Cart>>("carts");

            if (context != null && context.Any())
            {
                carts = context.ToList();
            }
            
            int[] productInCart = carts.Select(i => i.ProductId).ToArray();
            
            IEnumerable<Product> products = _db.Product.Where(i => productInCart.Contains(i.Id));

            return View(products);
        }
    }
}