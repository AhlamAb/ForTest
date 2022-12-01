using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _db;

		public CategoryController(ApplicationDbContext db)
		{
			_db = db;
		}


		public IActionResult Index()
		{
			IEnumerable<Category> objList = _db.category;

			return View(objList);
		}
		// GET - Create
        public IActionResult Create()
        {
            

            return View();
        }

		// POST - Create
		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult Create( Category obj)
        {
			if (ModelState.IsValid)
			{
                _db.category.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
			return View(obj);
        }

        // GET - Edit
        public IActionResult Edit(int? id)
        {
			if(id == null || id==0)
			{
				return NotFound();
			}
			var obj = _db.category.Find(id) ;
			if(obj == null)
			{
				return NotFound();
			}

            return View(obj);
        }

        // POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.category.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //--------------------------------

        // GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.category.Find(id);
           if(obj == null)
            {
                return NotFound();
            }
                _db.category.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            
            return View(obj);
        }

    }
}
