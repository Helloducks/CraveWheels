using CraveWheels.Data;
using CraveWheels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult AddToCart([FromForm] int ProductId, [FromForm] int Quantity)
        {
            // retrieve Id to identify the current user session
            var customerId = GetCustomerId();
        // retrieve price from db
        var price = _context.Products.Find(ProductId).Price;
        // create and save cart object
        var cart = new CartItem()
        {
            ProductId = ProductId,
            Quantity = Quantity,
            Price = price,
            CustomerId = customerId
        };
        // save changes to the db
        _context.CartItems.Add(cart);
            _context.SaveChanges();
            // redirect to /Shop/Cart
            return Redirect("Cart");
    }

    // GET: /Shop/Cart
    public IActionResult Cart()
    {
        // get customerid
        var customerId = GetCustomerId();
        // get cart items as a list
        var cartItems = _context.CartItems//SELECT 
                .Include(c => c.Product)//SUPER IMPRORTANT 
            // method chaining
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.Product.Name)
            .ToList();
            // return list to view
            // TODO: calculate total amount of cart and return to view
            // SELECT SUM(c.Price) FROM CartItems c
            var total = cartItems.Sum(c => c.Price).ToString("C");
            ViewBag.TotalAmount = total;
            return View(cartItems);
    }
        public IActionResult RemoveFromCart(int id)
        {
            // find cartitem 
            var cartItem = _context.CartItems.Find(id);
            // remove from db
            _context.CartItems.Remove(cartItem);
            // save changes
            _context.SaveChanges();
            // redirect back to cart page
            return RedirectToAction("Cart");
        }

        // TODO: Protected Checkout > only authenticated users can complete a purchase

        // Helper Method
        // Retrieves or generates ID to identify user
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }


        private string GetCustomerId()
        {
            // variable to store the ID temporarily
            string customerId = string.Empty;
            // check the session object for a customer id
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId")))
            {
                // if user is authenticated, use email
                if (User.Identity.IsAuthenticated)
                {
                    customerId = User.Identity.Name;
                }
                // else use a GUID
                else
                {
                    customerId = Guid.NewGuid().ToString();
                }
                // update session value
                HttpContext.Session.SetString("CustomerId", customerId);
            }
            // return whatever value is in the session object
            return HttpContext.Session.GetString("CustomerId");
        }
        }
}
