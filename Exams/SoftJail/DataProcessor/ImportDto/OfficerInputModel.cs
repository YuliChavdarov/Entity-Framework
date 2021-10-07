﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class OfficerInputModel
    {
        [XmlElement("Name")]
        public string FullName { get; set; }
        [XmlElement("Money")]
        public decimal Salary { get; set; }
        [XmlElement("Position")]
        public string Position { get; set; }
        [XmlElement("Weapon")]
        public string Weapon { get; set; }
        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }
        [XmlArray("Prisoners")]
        public List<OfficerPrisonerInputModel> OfficerPrisoners { get; set; }
    }
}