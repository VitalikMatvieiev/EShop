using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _webHostEnviromnet;

        public ProductController(IProductRepository productRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = productRepo;
            _webHostEnviromnet = webHostEnvironment;
        }


        public IActionResult Index()
        {
            IEnumerable<Product> objList = _productRepo.GetAll(includeProperties: "Category,ApplicationType");
/*            foreach (var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
                obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            }*/
            return View(objList);
        }


        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectionList = _productRepo.GetAllDrobDownList(WC.CategoryName),
                ApplicationTypeSelectList = _productRepo.GetAllDrobDownList(WC.ApplicationTypeName),

            };

            if (id == null)
            {
                //this for create
                return View(productVM);
            } 
            else
            {
                productVM.Product = _productRepo.Find(id.GetValueOrDefault());
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnviromnet.WebRootPath;

                 if (productVM.Product.Id == 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;
                    _productRepo.Add(productVM.Product);

                    TempData[WC.Success] = "Product was created";
                }
                else
                {
                    var objFromDb = _productRepo.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking: false);

                    if(files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;

                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _productRepo.Update(productVM.Product);

                    TempData[WC.Success] = "Product was edited";
                }
                _productRepo.Save();
                return RedirectToAction("Index"); 
            }

            productVM.CategorySelectionList = _productRepo.GetAllDrobDownList(WC.CategoryName);
            productVM.ApplicationTypeSelectList = _productRepo.GetAllDrobDownList(WC.ApplicationTypeName);

            return View(productVM);
        }

        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            ProductVM productVM = new ProductVM();

            productVM.Product = _productRepo.FirstOrDefault(u => u.Id == id, includeProperties: "Category,ApplicationType");
            /*productVM.Product.Category = _db.Category.Find(productVM.Product.CategoryId);*/
            if (productVM == null)
            {
                return NotFound();
            }

            return View(productVM);
        }
        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            string webRootPath = _webHostEnviromnet.WebRootPath;

            var product = _productRepo.Find(id.GetValueOrDefault());

            string upload = webRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, product.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _productRepo.Remove(product);
            _productRepo.Save();

            TempData[WC.Success] = "Product was deleted";

            return RedirectToAction("Index");

        }
    }
}
