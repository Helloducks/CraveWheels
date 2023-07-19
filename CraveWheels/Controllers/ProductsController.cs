using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CraveWheels.Data;
using CraveWheels.Models;
using Microsoft.AspNetCore.Authorization;

namespace CraveWheels.Controllers
{
    //this authorize annontaion is used to make every method in this controlller class to be only accessed by logged in user 
    //if this annontation was done above the 
    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Restaurant)
                .OrderBy(p => p.Restaurant.Name)
                .ThenBy(p => p.Name);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        //we are overriding the authorize annotation above the class with the below annotation
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Restaurant)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants.OrderBy(r => r.Name), "RestaurantId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Name,Price,RestaurantId")] Product product, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null) {
                    var fileName = UploadPhoto(Photo);
                    product.Photo = fileName;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "Name", product.RestaurantId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "RestaurantId", "Name", product.RestaurantId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Name,Price,RestaurantId")] Product product,IFormFile? Photo,string? CurrentPhoto)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Upload a photo must happen before we call Update and Save
                    if (Photo != null)
                    {
                        var fileName = UploadPhoto(Photo);
                        product.Photo = fileName;
                    }
                    else {
                        //this the hiddden input asked by the edit form
                        product.Photo = CurrentPhoto;
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants.OrderBy(r => r.Name), "RestaurantId", "Name", product.RestaurantId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Restaurant)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductID == id)).GetValueOrDefault();
        }
        private static string UploadPhoto(IFormFile Photo)
        {
            // get temp location of upload
            var filePath = Path.GetTempFileName();

            // use GUID class to generate unique name.  e.g. food.jpg => a348908sdaf-food.jpg
            var fileName = Guid.NewGuid().ToString() + "-" + Photo.FileName;

            // set destination path dynamically so it works on any server
            var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\products\\" + fileName;

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                Photo.CopyTo(stream);
            }

            return fileName;
        }
    }
}
