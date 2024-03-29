﻿using System.ComponentModel.DataAnnotations;

namespace CraveWheels.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        //parent 
        public Product? Product { get; set; } = default;
    }
}
