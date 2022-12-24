using BulkyBook.CommonHelper;
using BulkyBook.DataAccessLayer.Infrastructure.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CartVM itemList { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM VM = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new BulkyBook.Models.OrderHeader()
            };
            
            foreach (var item in VM.ListOfCart)
            {
                VM.OrderHeader.OrderTotal += (Int32.Parse(item.Product.Price) * (item.Count));
            }

            return View(VM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartVM VM = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new BulkyBook.Models.OrderHeader()
            };
            VM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == claims.Value);
            VM.OrderHeader.Name = VM.OrderHeader.ApplicationUser.Name;
            VM.OrderHeader.Phone = VM.OrderHeader.ApplicationUser.PhoneNumber;
            VM.OrderHeader.Address = VM.OrderHeader.ApplicationUser.Address;
            VM.OrderHeader.City = VM.OrderHeader.ApplicationUser.City;
            VM.OrderHeader.State = VM.OrderHeader.ApplicationUser.State;

            foreach (var item in VM.ListOfCart)
            {
                VM.OrderHeader.OrderTotal += (Int32.Parse(item.Product.Price) * (item.Count));
            }

            return View(VM);
        }
        [HttpPost]

        public IActionResult Summary(CartVM vm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm.ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product");
            vm.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            vm.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            vm.OrderHeader.PaymentStatus = PaymentStatus.StatusPending;
            vm.OrderHeader.DateOfOrder = DateTime.Now;
            vm.OrderHeader.ApplicationUserID = claims.Value;

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (Int32.Parse(item.Product.Price) * (item.Count));
            }

            _unitOfWork.OrderHeader.Add(vm.OrderHeader);
            _unitOfWork.Save();

            var domain = "https://localhost:7092/";
            var option = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var item in vm.ListOfCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = vm.OrderHeader.Id,
                    Count = item.Count,
                    Price = Int32.Parse(item.Product.Price)

                };



                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            //var service = new Stripe.Checkout.SessionService();
            //Session session = service.Create(option);
           // _unitOfWork.OrderHeader.PaymentStatus(vm.OrderHeader.Id, session.id, session.paymentIntentId);
            //_unitOfWork.Save();



            //_unitOfWork.Cart.DeleteRange(vm.ListOfCart);
            //  _unitOfWork.Save();

            return RedirectToAction("Index","Home");
        }


        



        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            _unitOfWork.Cart.IncrementCartItem(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            if (cart.Count <= 1)
            {
                _unitOfWork.Cart.Delete(cart);

            }
            else
            {
                _unitOfWork.Cart.DecrementCartItem(cart, 1);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            _unitOfWork.Cart.Delete(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        //public IActionResult ordersuccess(int id)
        //{
        //    var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id);
        //    var service = new SessionService();
        //    Session session = service.Get(orderHeader.SessionId);

        //    if(session.PaymentStatus.ToLower()=="paid")
        //    {
        //        _unitOfWork.OrderHeader.UpdateStatus(id, OrderStatus.StatusApproved);
        //    }
        //    List<Cart> cart = (List<Cart>)_unitOfWork.Cart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserID);
        //    _unitOfWork.Cart.DeleteRange(cart);
        //    _unitOfWork.Save();
                       

        //    return View(id);
        //}

    }

}
