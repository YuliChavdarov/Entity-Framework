namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects
                .Include(x => x.Tasks)
                .ToList()
                .Where(x => x.Tasks.Any())
                .Select(x => new ProjectOutputModelXml
                {
                    ProjectName = x.Name,
                    HasEndDate = x.DueDate == null ? "No" : "Yes",
                    TaskCount = x.Tasks.Count,
                    Tasks = x.Tasks.Select(x => new TaskOutputModelXml
                    {
                        Name = x.Name,
                        Label = x.LabelType.ToString()
                    })
                    .OrderBy(x => x.Name)
                    .ToList()
                })
                .OrderByDescending(x => x.TaskCount)
                .ThenBy(x => x.ProjectName)
                .ToList();

            return XmlConverter.Serialize(projects, "Projects");
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Include(x => x.EmployeesTasks)
                .ThenInclude(x => x.Task)
                .ToList()
                .Where(x => x.EmployeesTasks.Any(x => x.Task.OpenDate >= date))
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .Where(x => x.Task.OpenDate >= date)
                    .OrderByDescending(x => x.Task.DueDate)
                    .ThenBy(x => x.Task.Name)
                    .Select(x => new
                    {
                        TaskName = x.Task.Name,
                        OpenDate = x.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = x.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = x.Task.LabelType.ToString(),
                        ExecutionType = x.Task.ExecutionType.ToString()
                    })
                    .ToList()
                })
                .OrderByDescending(x => x.Tasks.Count)
                .ThenBy(x => x.Username)
                .Take(10)
                .ToList();

            return JsonConvert.SerializeObject(employees, Formatting.Indented);
        }
    }
}