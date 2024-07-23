using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeLibrary; 
namespace Adapter
{
	class Middleware
	{
		static async Task Main(string[] args)
		{
			string connectionString = "Server=localhost\\SQLEXPRESS; Database=App1; Integrated Security=True; TrustServerCertificate=True;";
			string app2ApiUrl = "http://localhost:5000/api/employees";

			EmployeeService employeeService = new EmployeeService();

			List<Employee> employees = employeeService.FetchUnprocessedDataFromApp1(connectionString);

			if (employees.Count > 0)
			{
				await employeeService.SendDataToApp2(employees, app2ApiUrl);

				employeeService.MarkRecordsAsProcessed(connectionString, employees);
			}
		}
	}
}
