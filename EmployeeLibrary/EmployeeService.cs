using System.Text;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;


namespace EmployeeLibrary
{
	public class EmployeeService
	{
		public List<Employee> FetchUnprocessedDataFromApp1(string connectionString)
		{
			List<Employee> employees = new List<Employee>();

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				string query = "SELECT Id, EmployeeName, Contact, Address FROM Employees WHERE Processed = 0";
				using (SqlCommand cmd = new SqlCommand(query, conn))
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						employees.Add(new Employee
						{
							Id = reader.GetInt32(0),
							EmployeeName = reader.GetString(1),
							Contact = reader.GetString(2),
							Address = reader.GetString(3)
						});
					}
				}
			}

			return employees;
		}

		public async Task SendDataToApp2(List<Employee> employees, string apiUrl)
		{
			using (HttpClient client = new HttpClient())
			{
				var json = JsonConvert.SerializeObject(employees);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(apiUrl, content);
				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine("Data sent successfully.");
				}
				else
				{
					Console.WriteLine($"Error sending data: {response.ReasonPhrase}");
				}
			}
		}

		public void MarkRecordsAsProcessed(string connectionString, List<Employee> employees)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				foreach (var employee in employees)
				{
					string query = "UPDATE Employees SET Processed = 1 WHERE Id = @id";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@id", employee.Id);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}
	}

	public class Employee
	{
		public int Id { get; set; }
		public string EmployeeName { get; set; }
		public string Contact { get; set; }
		public string Address { get; set; }
	}
}
