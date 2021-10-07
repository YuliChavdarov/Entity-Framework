namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var departmentDTOs = JsonConvert.DeserializeObject<IEnumerable<DepartmentInputModel>>(jsonString);

            foreach (var department in departmentDTOs)
            {
                var deptToAdd = new Department
                {
                    Name = department.Name,
                    Cells = department.Cells
                    .Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    })
                    .ToList()
                };

                if (IsValid(deptToAdd) == false || deptToAdd.Cells.Count() < 1 || deptToAdd.Cells.Any(x => IsValid(x) == false))
                {
                    sb.AppendLine("Invalid Data");
                }
                else
                {
                    context.Departments.Add(deptToAdd);
                    sb.AppendLine($"Imported {department.Name} with {department.Cells.Count()} cells");
                }
            }

            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var prisonerDTOs = JsonConvert.DeserializeObject<IEnumerable<PrisonerInputModel>>(jsonString);

            foreach (var prisoner in prisonerDTOs)
            {
                var prisonerToAdd = AutoMapper.Mapper.Map<PrisonerInputModel, Prisoner>(prisoner);

                if (IsValid(prisonerToAdd) == false || prisonerToAdd.Mails.Any(x => IsValid(x) == false))
                {
                    sb.AppendLine("Invalid Data");
                }
                else
                {
                    context.Prisoners.Add(prisonerToAdd);
                    sb.AppendLine($"Imported {prisonerToAdd.FullName} {prisonerToAdd.Age} years old");
                }
            }

            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var officerDTOs = XmlConverter.Deserializer<OfficerInputModel>(xmlString, "Officers");

            foreach (var officer in officerDTOs)
            {
                try
                {
                    var officerToAdd = AutoMapper.Mapper.Map<Officer>(officer);
                    if (IsValid(officerToAdd) == false)
                    {
                        sb.AppendLine("Invalid Data");
                    }
                    else
                    {
                        context.Officers.Add(officerToAdd);
                        sb.AppendLine($"Imported {officerToAdd.FullName} ({officerToAdd.OfficerPrisoners.Count} prisoners)");
                    }
                }
                catch
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
            }

            context.SaveChanges();
            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}