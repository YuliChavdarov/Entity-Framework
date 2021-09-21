﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_SalesDatabase.Data.Models
{
    public class Customer
    {
        public Customer()
        {
            Sales = new HashSet<Sale>();
        }
        public int CustomerId { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [MaxLength(80)]
        [Column(TypeName = "VARCHAR(80)")]
        public string Email { get; set; }
        public string CreditCardNumber { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
