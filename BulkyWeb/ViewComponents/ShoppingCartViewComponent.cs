using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
        //this view component is used to get the shopping cart item number/count from db


        private readonly IUnitOfWork _UnitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        //handles the retrieving functionality

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null) { 
                
                //check if the current session has the cart value, else get form db
                if(HttpContext.Session.GetInt32(SD.SessionCart) == null) 
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        _UnitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == claim.Value).Count());
                    //return back to view
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                //session cart exists, return the existing value
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                //no users
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
//the view component 'View' will be in shared folder named 'Component' ->
    //inside this folder create another folder of The viewComponent name (here, ShoppingCart)
        //then create a view named -> (Default.cshtml)
