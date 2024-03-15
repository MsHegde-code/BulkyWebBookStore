using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {

        //this creates the admin user and roles for the website at the time of deployment
        void Initialize();
    }
}
