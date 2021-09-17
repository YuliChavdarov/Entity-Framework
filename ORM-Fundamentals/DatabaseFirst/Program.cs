using DatabaseFirst.Models;
using System;
using System.Linq;

namespace DatabaseFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext db = new SoftUniContext();

            var employees = db.Employees.Where(x => x.FirstName.StartsWith("A"))
                .OrderByDescending(x => x.Salary)
                .Select(x => new { x.FirstName, x.LastName, x.Salary })
                .ToList();

            foreach(var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName} {employee.Salary}");
            }
        }
    }
}
