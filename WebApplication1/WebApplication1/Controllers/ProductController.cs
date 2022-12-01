using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using static System.Net.WebRequestMethods;

namespace WebApplication1.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(ApplicationDbContext db , IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
            _webHostEnvironment = webHostEnvironment;
        }


		public IActionResult Index()
		{
			IEnumerable<Product> objList = _db.Product;

            foreach (var obj in objList)
            {
                obj.Category = _db.category.FirstOrDefault(u=> u.Id == obj.CategoryId);
            }


			return View(objList);
		}

        //upsert for create and edit 
		// GET - Upsert
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _db.category.Select(
            //i => new SelectListItem
            //{
            //    Text = i.Name  , 
            //    Value = i.Id.ToString()
            //});

            //View bag => transfer data from controller to view 
            // View bag is a wrapper around ViewData

            //ViewData["CategoryDropDown"] = CategoryDropDown;

            //Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if(id== null)
            {
                // this is for create 
                return View(productVM);
            

            }
            else
            {
                productVM.Product = _db.Product.Find(id);
                if(productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }

            return View();
        }


        // POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (productVM.Product.Id ==0)
                {
                    // creating 
                    string upload = webRootPath + WC.ImagePaht;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload,fileName+extension) , FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;
                    _db.Product.Add(productVM.Product);

                }
                else
                {
                    // updating
                    // in update to make sure an image is updating 
                    // remove last image 
                    // and upload the second image 
                    // and we want to retrive an entity from DB then update it 

                    // let's retrieve actual product from DB
                    // nad we will have the att here
                    var objFromDB = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);

                    // we will chack 
                    // files has updated                    
                    if(files.Count>0)
                    {
                        string upload = webRootPath + WC.ImagePaht;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        // we want to remove the old file

                        var oldFile = Path.Combine(upload,objFromDB.Image);
                        // then we will check if the file exist 
                        if(System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        // here will modified 
                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        // this mean the file not updated , but else thing was updated
                        // we will keep image as is , not modified 
                        productVM.Product.Image = objFromDB.Image;


                    }

                    // that upadte everthing change in  product 
                    _db.Product.Update(productVM.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _db.category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(productVM);

        }   
			



         // GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // we will the product 
            Product product = _db.Product.Include(u=> u.Category).FirstOrDefault(u=> u.Id== id);
            product.Category = _db.category.Find(product.CategoryId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }





        // POST - Delete
        [HttpPost , ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Product.Find(id);
           if(obj == null)
            {
                return NotFound();
            }        
            
                string upload = _webHostEnvironment.WebRootPath + WC.ImagePaht;
              
                // we want to remove the old file

                var oldFile = Path.Combine(upload, obj.Image);
                // then we will check if the file exist 
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }

                _db.Product.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            
            return View(obj);
        }

    }
}
