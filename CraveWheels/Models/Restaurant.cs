using System.ComponentModel.DataAnnotations;

namespace CraveWheels.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }

        [Required]
        public string Name { get; set; }
        //child referece to prodcuts (1 Restuaurant >>many Products)
        public List<Product>? Products { get; set; } = default;
        public List<Order>? Orders { get; set; } = default;
    }
}
