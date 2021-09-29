using CarDealer.DTO.InputModels;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO
{
    [XmlType("Car")]
    public class CarInputModel
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }
        [XmlArray("parts")]
        public List<PartIdInputModel> PartsId { get; set; } = new List<PartIdInputModel>();
    }
}
