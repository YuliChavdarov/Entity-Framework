using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ConnectToMSSQL
{
    class Program
    {
        static void Main(string[] args)
        {

            string connectionString = "Server=.; Integrated Security = true; Database = SoftUni";

            List<Employee> employees = new List<Employee>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string scalarQuery = "SELECT COUNT(*) FROM Employees";
                string updateNonQuery = "UPDATE Employees SET Salary = Salary + 10 WHERE Salary < 40000";
                string tableQuery = "SELECT FirstName, LastName, Salary FROM Employees";

                SqlCommand scalarCommand = new SqlCommand(scalarQuery, connection);
                object scalarResult = scalarCommand.ExecuteScalar();

                SqlCommand nonQueryCommand = new SqlCommand(updateNonQuery, connection);
                int nonQueryResult = nonQueryCommand.ExecuteNonQuery();

                SqlCommand tableCommand = new SqlCommand(tableQuery, connection);

                using (SqlDataReader tableResult = tableCommand.ExecuteReader())
                {
                    while (tableResult.Read() == true)
                    {
                        Employee currentEmployee = new Employee();
                        currentEmployee.FirstName = tableResult["FirstName"] as string;
                        currentEmployee.LastName = tableResult["LastName"] as string;
                        currentEmployee.Salary = tableResult["Salary"] as decimal?;
                        employees.Add(currentEmployee);
                        Console.WriteLine($"{tableResult["FirstName"]} {tableResult["LastName"]}");
                    }
                }

                Console.WriteLine(scalarResult);
                Console.WriteLine(nonQueryResult);
            }
            Console.WriteLine("employees in List<Employee>: " + employees.Count);
        }

        public class Employee
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public decimal? Salary { get; set; }
        }
    }
}
