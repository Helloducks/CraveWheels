using System.ComponentModel.DataAnnotations;

namespace CraveWheels.Models
{
    public class Product
    { //this comment here to check updates
        public int ProductID { get; set; }
        public string Name { get; set; }

        [Range (0.01, 10000)]
        public decimal Price { get; set; }
        //public decimal testvar { get; set; }

        //Parent refercne

        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; } = default;

        // child refernce 
        public List<CartItem>? CartItems { get; set; }
    }
}
