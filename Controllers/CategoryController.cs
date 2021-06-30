using IntroAspNet.Data;
using IntroAspNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace IntroAspNet.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDataContext _db;
        public CategoryController(ApplicationDataContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.Category;
            return View(categories);
        }

        // Upsert

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                return View(category);
            }
            else
            {
                category = _db.Category.Find(id);

                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _db.Category.Add(category);
                }
                else
                {
                    var formObject = _db.Category.AsNoTracking().FirstOrDefault(u => u.Id == category.Id);
                    _db.Category.Update(category);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // Delete

        public IActionResult Delete(int? id)
        {
            return View();
        }

        [Route("[controller]/DeleteCategory/"), HttpDelete("{id:int}"), ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            var category = _db.Category.Find(id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                _db.Category.Remove(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
