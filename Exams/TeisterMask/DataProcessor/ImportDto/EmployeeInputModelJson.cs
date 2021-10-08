﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class EmployeeInputModelJson
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression("[a-zA-Z0-9]*")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("[0-9]{3}-[0-9]{3}-[0-9]{4}")]
        public string Phone { get; set; }
        public IEnumerable<int> Tasks { get; set; }
    }
}
