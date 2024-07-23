using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quickbooks
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}

	public class Employee
	{
		public int Id { get; set; }
		public string EmployeeName { get; set; }
		public string Contact { get; set; }
		public string Address { get; set; }
	}

	[ApiController]
	[Route("api/[controller]")]
	public class EmployeesController : ControllerBase
	{
		[HttpPost]
		public IActionResult Post([FromBody] List<Employee> employees)

		{
			Console.WriteLine("Received data:");
			foreach (var employee in employees)
			{
				Console.WriteLine($"Id: {employee.Id}, Name: {employee.EmployeeName}, Contact: {employee.Contact}, Address: {employee.Address}");
			}


			string connectionString = "Server=localhost\\SQLEXPRESS; Database=App2; Integrated Security=True;  TrustServerCertificate=True;";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				foreach (var employee in employees)
				{
					string query1 = "INSERT INTO Emp2 (Name, Mobile, Place) VALUES (@name, @contact, @address)";
					using (SqlCommand cmd = new SqlCommand(query1, conn))
					{
						cmd.Parameters.AddWithValue("@name", employee.EmployeeName);
						cmd.Parameters.AddWithValue("@contact", employee.Contact);
						cmd.Parameters.AddWithValue("@address", employee.Address);
						cmd.ExecuteNonQuery();
					}
					string query2 = "INSERT INTO Emp3 (EmpName, EmpAddress) VALUES (@name, @address)";
					using (SqlCommand cmd = new SqlCommand(query2, conn))
					{
						cmd.Parameters.AddWithValue("@name", employee.EmployeeName);
						cmd.Parameters.AddWithValue("@address", employee.Address);
						cmd.ExecuteNonQuery();
					}
				}
			}
			Console.WriteLine("Data inserted successfully.");


			return Ok();
		}
	}

	public class DataCollector
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}

