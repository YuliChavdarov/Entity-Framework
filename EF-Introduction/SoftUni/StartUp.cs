using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext db = new SoftUniContext();

            Console.WriteLine(GetEmployeesInPeriod(db));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(x => x.EmployeeId)
                .Select(x => new { x.FirstName, x.MiddleName, x.LastName, x.JobTitle, x.Salary })
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:0.00}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(x => x.FirstName)
                .Select(x => new { x.FirstName, x.Salary })
                .Where(x => x.Salary > 50000.00M)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:0.00}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new { x.FirstName, x.LastName, DepartmentName = x.Department.Name, x.Salary })
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:0.00}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            nakov.Address = new Address() { AddressText = "Vitoshka 15", TownId = 4 };

            context.SaveChanges();

            var addresses = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => x.Address.AddressText)
                .ToList();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.EmployeesProjects.Any(x => x.Project.StartDate.Year >= 2001 && x.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(x => new {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(x => new {
                        Name = x.Project.Name,
                        StartDate = x.Project.StartDate,
                        EndDate = x.Project.EndDate
                    })
                })
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach(var project in employee.Projects)
                {
                    string endDate = project.EndDate != null
                        ? project.EndDate?.ToString("M/d/yyyy h:mm:ss tt")
                        : "not finished";

                    sb.AppendLine($"--{project.Name} - {project.StartDate.ToString("M/d/yyyy h:mm:ss tt")} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}