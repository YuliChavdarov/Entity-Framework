using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.OutputModels.GetUsersWithProducts
{
    [XmlType("SoldProducts")]
    public class SoldProductsModel
    {
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("products")]
        public List<ProductOutputModel> Products{ get; set; }
    }
}
