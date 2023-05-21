using CraveWheels.Models;
using Microsoft.AspNetCore.Mvc;


namespace CraveWheels.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        //GET: /shop/Restaurant
        public IActionResult Restaurant(String Name)
        {
            if (Name == null) { 
                return RedirectToAction("Index");
            }
            // use the product model to make a mock list of product to display
            var products = new List<Product>();
            for (var i=1;i<11;i++) { 
                products.Add(new Product { ProductID=i,Name = "Product" + i.ToString() ,Price=i});
            }
            ViewData["Name"] = Name;
            return View(products); 
        }
    }
}
