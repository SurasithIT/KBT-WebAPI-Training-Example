using System;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.Employees
{
	public class EmployeeModel
	{
		[JsonPropertyName("employeeKey")]
		public int EmployeeKey { get; set; }

		[JsonPropertyName("firstName")]
		public string FirstName { get; set; }

		[JsonPropertyName("lastName")]
		public string LastName { get; set; }
	}
}

