using BulkyBook.DataAccessLayer.Infrastructure.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccessLayer.Infrastructure.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void PaymentStatus(int Id, string SessionId, string PaymentIntentId)
        {
            var orderHeader=_context.OrderHeaders.FirstOrDefault(x=>x.Id==Id);
            //orderHeader.PaymentIntentId=PaymentIntentId;
            orderHeader.DateOfPayment=DateTime.Now;
            orderHeader.SessionId=SessionId;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
            //var categoryDb = _context.Categories.FirstOrDefault(x => x.ID == orderHeader.ID);
            //if(categoryDb != null)
            //{
            //    categoryDb.Name = orderHeader.Name;
            //    categoryDb.DisplayOrder = orderHeader.DisplayOrder;

            //}
        }

        public void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null)
        {
            var order = _context.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            if(order != null)
            {
                order.OrderStatus = orderStatus;
            }
            if(paymentStatus != null)
            {
                order.PaymentStatus=paymentStatus;
            }
        }
    }
}
