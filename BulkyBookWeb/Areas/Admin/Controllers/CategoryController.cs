
using BulkyBook.DataAccessLayer;
using BulkyBook.DataAccessLayer.Infrastructure.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            CategoryVM categoryVM=new CategoryVM();
            categoryVM.categories = _unitOfWork.Category.GetAll();
            return View(categoryVM);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Created";
                return RedirectToAction("Index");
            }


            return View(obj);
        }
        public IActionResult CreateUpdate(int? id)
        {
            CategoryVM vm = new CategoryVM();
            if (id == null || id == 0)
            {
                return View(vm);
            }
            else 
            {
                vm.Category = _unitOfWork.Category.GetT(x => x.ID == id);

                if (vm.Category == null)
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
        public IActionResult CreateUpdate(CategoryVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Category.ID == 0)
                {
                    _unitOfWork.Category.Add(vm.Category);
                    TempData["Success"] = "Category created";
                }
                else
                {
                    _unitOfWork.Category.Update(vm.Category);
                    TempData["Success"] = "Category Updated";
                }

                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }


            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var CategoryFromDb = _unitOfWork.Category.GetT(x => x.ID == id);

            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var CategoryFromDb = _unitOfWork.Category.GetT(x => x.ID == id);

            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(CategoryFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Category Deleted";
            return RedirectToAction("Index");
        }
    }
}
