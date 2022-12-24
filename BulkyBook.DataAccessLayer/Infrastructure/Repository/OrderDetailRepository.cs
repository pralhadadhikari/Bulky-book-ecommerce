using BulkyBook.DataAccessLayer.Infrastructure.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccessLayer.Infrastructure.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);

            //var categoryDb = _context.Categories.FirstOrDefault(x => x.ID == category.ID);
            //if(categoryDb != null)
            //{
            //    categoryDb.Name = category.Name;
            //    categoryDb.DisplayOrder = category.DisplayOrder;

            //}
        }
    }
}
