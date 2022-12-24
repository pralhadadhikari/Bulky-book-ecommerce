using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccessLayer.Infrastructure.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }

        IApplicationUserRepository ApplicationUser { get; }

        ICartRepository Cart { get; }

        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }


        void Save();
    }
}
