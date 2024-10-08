﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ProductShop.Models
{
    public class CategoryProduct
    {
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
