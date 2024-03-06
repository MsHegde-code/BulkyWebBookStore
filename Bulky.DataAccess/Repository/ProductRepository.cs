using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private readonly ApplicationDbContext _db;

        //remember to extend the constructor to its base class, as we are using AppliDbContext in Repository.cs also
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            _db.Products.Update(product); //can also be used or we may be more specific about the update condition
            //var ObjFromDb = _db.Products.FirstOrDefault(u=>u.Id == product.Id);
            //if (ObjFromDb != null)
            //{
            //    ObjFromDb.Title = product.Title;
            //    ObjFromDb.Description = product.Description;
            //    ObjFromDb.CategoryId = product.CategoryId;
            //    ObjFromDb.ListPrice = product.ListPrice;
            //    ObjFromDb.Price = product.Price;
            //    ObjFromDb.Price100 = product.Price100;
            //    ObjFromDb.Price50 = product.Price50;
            //    ObjFromDb.Author = product.Author;
            //    ObjFromDb.ISBN = product.ISBN;

            //    //like update the image url if its not null
            //    if (product.ImageUrl != null)
            //    {
            //        ObjFromDb.ImageUrl = ObjFromDb.ImageUrl;
            //    }
            //}
        }
    }
}
