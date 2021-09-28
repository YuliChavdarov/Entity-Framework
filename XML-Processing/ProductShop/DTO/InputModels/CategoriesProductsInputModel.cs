using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO
{
    [XmlType("CategoryProduct")]
    public class CategoriesProductsInputModel
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
    }
}
