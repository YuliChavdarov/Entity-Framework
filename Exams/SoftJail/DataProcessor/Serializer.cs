namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Linq;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(x => new PrisonerOutputModelJSON
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    TotalOfficerSalary = decimal.Parse(x.PrisonerOfficers.Sum(x => x.Officer.Salary).ToString("0.00")),
                    Officers = x.PrisonerOfficers.Select(x => new OfficerOutputModelJSON
                    {
                        FullName = x.Officer.FullName,
                        DepartmentName = x.Officer.Department.Name
                    })
                    .OrderBy(x => x.FullName)
                    .ToList()
                })
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToList();

            return JsonConvert.SerializeObject(prisoners, Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisoners = context.Prisoners
                .Where(x => prisonersNames.Contains(x.FullName))
                .Select(x => new PrisonerOutputModelXML
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd"),
                    Messages = x.Mails
                    .Select(x => new MessageOutputModelXML
                    {
                        Description = string.Join("",x.Description.Reverse())
                    })
                    .ToList()
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            return XmlConverter.Serialize(prisoners, "Prisoners");
        }
    }
}