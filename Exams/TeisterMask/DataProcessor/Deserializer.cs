namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var projectDtos = XmlConverter.Deserializer<ProjectInputModelXml>(xmlString, "Projects");

            foreach (var projectDto in projectDtos)
            {
                DateTime projectOpenDate;

                if (IsValid(projectDto) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy",
                  CultureInfo.InvariantCulture, DateTimeStyles.None, out projectOpenDate) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (string.IsNullOrEmpty(projectDto.DueDate) == false
                    && DateTime.TryParseExact(projectDto.DueDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projectToAdd = new Project
                {
                    Name = projectDto.Name,
                    OpenDate = projectOpenDate,
                    DueDate = string.IsNullOrEmpty(projectDto.DueDate) ? null : (DateTime?)DateTime.ParseExact(projectDto.DueDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture),
                    Tasks = new List<Task>()
                };

                foreach (var taskDto in projectDto.Tasks)
                {
                    DateTime taskOpenDate;
                    DateTime taskDueDate;

                    if (DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOpenDate) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out taskDueDate) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (IsValid(taskDto) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (taskOpenDate < projectToAdd.OpenDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (projectToAdd.DueDate.HasValue && taskDueDate > projectToAdd.DueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var taskToAdd = new Task
                    {
                        Name = taskDto.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType
                    };

                    projectToAdd.Tasks.Add(taskToAdd);
                }

                context.Projects.Add(projectToAdd);
                sb.AppendLine($"Successfully imported project - {projectToAdd.Name} with {projectToAdd.Tasks.Count} tasks.");
            }

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var employeeDtos = JsonConvert.DeserializeObject<List<EmployeeInputModelJson>>(jsonString);

            foreach (var employeeDto in employeeDtos)
            {
                if(IsValid(employeeDto) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var employeeToAdd = new Employee
                {
                    Username = employeeDto.Username,
                    Phone = employeeDto.Phone,
                    Email = employeeDto.Email,
                    EmployeesTasks = new List<EmployeeTask>()
                };

                foreach (var taskId in employeeDto.Tasks.Distinct())
                {
                    if(context.Tasks.Any(x => x.Id == taskId) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employeeToAdd.EmployeesTasks.Add(new EmployeeTask { TaskId = taskId });
                }

                sb.AppendLine($"Successfully imported employee - {employeeToAdd.Username} with {employeeToAdd.EmployeesTasks.Count} tasks.");
                context.Employees.Add(employeeToAdd);
            }

            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}