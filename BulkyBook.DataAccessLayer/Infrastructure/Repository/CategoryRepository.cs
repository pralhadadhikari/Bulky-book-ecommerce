using BulkyBook.DataAccessLayer.Infrastructure.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccessLayer.Infrastructure.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryDb = _context.Categories.FirstOrDefault(x => x.ID == category.ID);
            if (categoryDb != null)
            {
                categoryDb.Name = category.Name;
                categoryDb.DisplayOrder = category.DisplayOrder;

            }
        }


    }
}