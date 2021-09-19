using SoftUni.Data;
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

            Console.WriteLine(GetEmployeesFullInformation(db));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(x => x.EmployeeId)
                .Select(x => new { x.FirstName, x.MiddleName, x.LastName, x.JobTitle, x.Salary });

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
                .OrderBy(x => x.EmployeeId)
                .Select(x => new { x.FirstName, x.MiddleName, x.LastName, x.JobTitle, x.Salary });

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.MiddleName} {employee.LastName} {employee.JobTitle} {employee.Salary:0.00}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}