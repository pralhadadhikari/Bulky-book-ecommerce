using BulkyBook.CommonHelper;
using BulkyBook.DataAccessLayer.Infrastructure.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        
        private IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult AllOrders(string status)
        {
            IEnumerable<OrderHeader> orderHeader;
           

            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                orderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderHeader = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserID == claims.Value);
            }

            switch (status)
            {
                case "pending":
                    orderHeader = orderHeader.Where(x => x.PaymentStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    orderHeader = orderHeader.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved);
                    break;

                default:
                    break;
            }

           
           
            return Json(new { data = orderHeader });
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OrderDetails(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id,
                includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.Id == id,
                includeProperties: "Product")
            };


            return View(orderVM);
        }


    }


}
