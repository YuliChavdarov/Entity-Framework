using System;
using System.Collections.Generic;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentInputModel
    {
        public string Name { get; set; }
        public IEnumerable<CellInputModel> Cells { get; set; }
    }
}
