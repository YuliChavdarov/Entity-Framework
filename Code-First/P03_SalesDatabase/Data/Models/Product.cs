﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_SalesDatabase.Data.Models
{
    public class Product
    {
        public Product()
        {
            Sales = new HashSet<Sale>();
        }
        public int ProductId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
