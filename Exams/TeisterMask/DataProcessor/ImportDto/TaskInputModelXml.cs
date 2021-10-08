using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class TaskInputModelXml
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; }
        [XmlElement("OpenDate")]
        [Required]
        public string OpenDate { get; set; }
        [XmlElement("DueDate")]
        [Required]
        public string DueDate { get; set; }
        [XmlElement("ExecutionType")]
        [Required]
        [Range(0,3)]
        public int ExecutionType { get; set; }
        [XmlElement("LabelType")]
        [Required]
        [Range(0,4)]
        public int LabelType { get; set; }
    }
}