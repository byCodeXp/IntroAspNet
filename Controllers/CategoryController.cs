using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntroAspNet.Data;
using IntroAspNet.Models;
using Microsoft.AspNetCore.Hosting;

namespace IntroAspNet.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.Category.ToListAsync());
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Upsert

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                return View(category);
            }
            else
            {
                category = _context.Category.Find(id);

                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
        }

        // POST: Category/Upsert

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                
                string upload = null, fileName = null, extension = null;
                upload = webRootPath + ENV.Path.CategoryPreview;
                fileName = Guid.NewGuid().ToString();
                
                if (files.Count > 0)
                {
                    extension = Path.GetExtension(files[0].FileName);
                }
                
                if (category.Id == 0)
                {
                    if (files.Count > 0)
                    {
                        using (FileStream fileStream = new (Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream); 
                        }
                    }

                    if (extension != null)
                    {
                        category.Preview = fileName + extension;
                    }
                    
                    _context.Category.Add(category);
                }
                else
                {
                    var formObject = _context.Category.AsNoTracking().FirstOrDefault(u => u.Id == category.Id);
                    
                    if (files.Count > 0)
                    {
                        var oldFile = Path.Combine(upload, formObject.Preview);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (FileStream fileStream = new (Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        
                        category.Preview = fileName + extension;
                    }
                    
                    _context.Category.Update(category);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.FindAsync(id);
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
