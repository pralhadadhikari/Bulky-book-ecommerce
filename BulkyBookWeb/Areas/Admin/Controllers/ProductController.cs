
using BulkyBook.DataAccessLayer;
using BulkyBook.DataAccessLayer.Infrastructure.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _hostingEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }

        //API Call
        public IActionResult AllProducts()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties:"Category");
            return Json(new {data=products});
        }

        public IActionResult Index()
        {
            //ProductVM productVM=new ProductVM();
            //productVM.Products = _unitOfWork.Product.GetAll();
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

         [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM vm = new ProductVM()
            {
                Product = new(),
                Categories = _unitOfWork.Category.GetAll().Select(x =>
                new SelectListItem()
                {
                    Text=x.Name,
                    Value=x.ID.ToString()
                })
            };
            if (id == null || id == 0)
            {
                return View(vm);
            }
            else 
            {
                vm.Product = _unitOfWork.Product.GetT(x => x.Id == id);

                if (vm.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(vm);
                }
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductVM vm,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = String.Empty;
                if(file!=null)
                {
                    string uploadDir = Path.Combine(_hostingEnvironment.WebRootPath,"ProductImage");
                    fileName= Guid.NewGuid().ToString()+"-"+file.FileName;
                    string filePath=Path.Combine(uploadDir,fileName);

                    if(vm.Product.ImageUrl!=null)
                    {
                        var oldImagePath=Path.Combine(_hostingEnvironment.WebRootPath,vm.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }

                    using (var fileStream=new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    vm.Product.ImageUrl = @"\ProductImage\" + fileName;
                }

                if(vm.Product.Id==0)
                {
                    _unitOfWork.Product.Add(vm.Product);
                    TempData["Success"] = "Product Created!";
                    
                }
                else
                {
                    _unitOfWork.Product.Update(vm.Product);
                    TempData["Success"] = "Product Updated!";
                }
                
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var CategoryFromDb = _unitOfWork.Category.GetT(x => x.ID == id);

        //    if (CategoryFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(CategoryFromDb);
        //}


        [HttpDelete]
        #region DeleteAPICALL     
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);

            if (product == null)
            {
                return Json(new { Success = false, message = "Error fetching data" });
            }
            else
            {
                var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.Product.Delete(product);
                _unitOfWork.Save();
                return Json(new { Success = true, message = "Product Deleted" });
               
            }
        } 
        
        #endregion
    }
}
