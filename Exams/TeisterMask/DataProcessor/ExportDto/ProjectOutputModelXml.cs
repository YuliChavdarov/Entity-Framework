using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ProjectOutputModelXml
    {
        [XmlAttribute("TasksCount")]
        public int TaskCount { get; set; }
        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }
        [XmlElement("HasEndDate")]
        public string HasEndDate { get; set; }
        [XmlArray("Tasks")]
        public List<TaskOutputModelXml> Tasks { get; set; }
    }
}
