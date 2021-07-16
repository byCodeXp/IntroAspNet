using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntroAspNet.Data;
using IntroAspNet.Models;
using Microsoft.AspNetCore.Hosting;
using IntroAspNet.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using IntroAspNet.Services;

namespace IntroAspNet.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product
        public IActionResult Index()
        {
            CatalogVM catalogVm = new()
            {
                Products = _context.Product.Include(i => i.Category),
                Categories = _context.Category,
            };
            return View(catalogVm);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var carts = HttpContext.Session.Get<IEnumerable<Cart>>("carts");

            if (id == null)
            {
                return NotFound();
            }
            
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            bool inCart = false;
            
            if (carts != null)
            {
                inCart = carts.Any(e => e.ProductId == id);
            }
            
            DetailProductVM detailProductVm = new()
            {
                Product = product,
                InCart = inCart
            };

            return View(detailProductVm);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult AddToCart(int id)
        {
            List<Cart> shoppingCartList = new();

            var context = HttpContext.Session.Get<IEnumerable<Cart>>("carts");
            if (context != null && context.Any())
            {
                shoppingCartList = context.ToList();
            }

            if (_context.Product.Any(e => e.Id == id))
            {
                shoppingCartList.Add(new Cart { ProductId = id });
                HttpContext.Session.Set("carts", shoppingCartList);
            }
            
            return RedirectToAction(nameof(Details));
        }
        
        public IActionResult RemoveFromCart(int id)
        {
            List<Cart> shoppingCartList = new List<Cart>();

            var carts = HttpContext.Session.Get<IEnumerable<Cart>>("carts");
            
            if (carts != null && carts.Any())
            {
                shoppingCartList = carts.ToList();
            }

            var removeItem = shoppingCartList.SingleOrDefault(u => u.ProductId == id);
            if (removeItem != null)
            {
                shoppingCartList.Remove(removeItem);
            }

            HttpContext.Session.Set("carts", shoppingCartList);

            return Redirect("/Product/Details/" + id);
        }
        
        // GET: Product/Upsert
        
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _context.Category.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _context.Product.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }

                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                string upload = webRootPath + ENV.Path.ProductPreview;
                string fileName = Guid.NewGuid().ToString();
                string extentions = Path.GetExtension(files[0].FileName);

                if (productVM.Product.Id == 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extentions), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Preview = fileName + extentions;
                    _context.Product.Add(productVM.Product);
                }
                else
                {
                    var formObject = _context.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
                    if (files.Count > 0)
                    {
                        var oldFile = Path.Combine(upload, formObject.Preview);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extentions), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Preview = fileName + extentions;
                    }
                    else
                    {
                        productVM.Product.Preview = formObject.Preview;
                    }
                    _context.Product.Update(productVM.Product);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _context.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(productVM);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
