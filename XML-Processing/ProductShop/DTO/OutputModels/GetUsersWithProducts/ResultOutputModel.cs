using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.OutputModels.GetUsersWithProducts
{
    [XmlType("Users")]
    public class ResultOutputModel
    {
        [XmlElement("count")]
        public int UsersCount { get; set; }
        [XmlArray("users")]
        public List<UserOutputModel> Users { get; set; }
    }
}
