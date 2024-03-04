using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    //inheriting the base (common )properties of IRepository
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
