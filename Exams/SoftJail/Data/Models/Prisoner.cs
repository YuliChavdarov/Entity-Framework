using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class Prisoner
    {
        public Prisoner()
        {
            Mails = new HashSet<Mail>();
            PrisonerOfficers = new HashSet<OfficerPrisoner>();
        }
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FullName { get; set; }
        [Required]
        [RegularExpression("The [A-Z][a-z]*")]
        public string Nickname { get; set; }
        [Range(18, 65)]
        public int Age { get; set; }
        [Required]
        public DateTime IncarcerationDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }
        [ForeignKey("Cell")]
        public int? CellId { get; set; }
        public Cell Cell { get; set; }
        public virtual ICollection<Mail> Mails { get; set; }
        public virtual ICollection<OfficerPrisoner> PrisonerOfficers { get; set; }
    }
}