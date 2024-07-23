using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

namespace Application1
{
	class DataStore
	{
		static void Main(string[] args)
		{
			string connectionString = "Server=localhost\\SQLEXPRESS; Database=App1; Integrated Security=True; TrustServerCertificate=True;";

			List<XElement> employeeElements = new List<XElement>();

			bool continueEntering = true;
			while (continueEntering)
			{
				Console.WriteLine("Enter Employee Name:");
				string name = Console.ReadLine();

				Console.WriteLine("Enter Employee Contact:");
				string contact = Console.ReadLine();

				Console.WriteLine("Enter Employee Address:");
				string address = Console.ReadLine();

				// Create XML element for the current employee
				XElement employeeElement = new XElement("Employee",
					new XElement("Name", name),
					new XElement("Contact", contact),
					new XElement("Address", address)
				);

				employeeElements.Add(employeeElement);

				Console.WriteLine("Do you want to enter another employee? (yes/no)");
				string response = Console.ReadLine();
				if (response?.ToLower() != "yes")
				{
					continueEntering = false;
				}
			}

			XDocument xdoc = new XDocument(
				new XElement("Employees",
					employeeElements
				)
			);

			Console.WriteLine("Generated XML Data:");
			Console.WriteLine(xdoc);

			// Insert XML data into the database
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				foreach (var employee in xdoc.Descendants("Employee"))
				{
					string query = "INSERT INTO Employees (EmployeeName, Contact, Address) VALUES (@name, @contact, @address)";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@name", employee.Element("Name").Value);
						cmd.Parameters.AddWithValue("@contact", employee.Element("Contact").Value);
						cmd.Parameters.AddWithValue("@address", employee.Element("Address").Value);
						cmd.ExecuteNonQuery();
					}
				}
			}

			Console.WriteLine("Data inserted successfully.");
		}
	}
}
