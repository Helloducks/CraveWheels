using CraveWheels.Data;
using CraveWheels.Models;
using Microsoft.AspNetCore.Mvc;


namespace CraveWheels.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var restaurants = _context.Restaurants.OrderBy(r => r.Name).ToList();

            return View(restaurants);
        }
        //GET: /shop/Restaurant
        //public IActionResult Restaurant(String Name)
        //{
        //    if (Name == null) { 
        //        return RedirectToAction("Index");
        //    }
        //    // use the product model to make a mock list of product to display
        //    var products = new List<Product>();
        //    for (var i=1;i<11;i++) { 
        //        products.Add(new Product { ProductID=i,Name = "Product" + i.ToString() ,Price=i});
        //    }
        //    ViewData["Name"] = Name;
        //    return View(products); 
        //}

        public IActionResult Restaurant(int id) {
            if (id == null) {
                return RedirectToAction("Index");
            }
            var products = _context.Products.Where(p => p.RestaurantId == id ).ToList();
            return View(products);
        
        }

        public IActionResult AddToCard([FromForm] int Pr)
    }
}
