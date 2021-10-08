using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Task")]
    public class TaskOutputModelXml
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Label")]
        public string Label { get; set; }
    }
}