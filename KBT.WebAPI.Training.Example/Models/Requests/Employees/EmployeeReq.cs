using System;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.Models.Requests.Employees
{
	public class EmployeeReq
	{
		[JsonPropertyName("firstName")]
		public string FirstName { get; set; }

		[JsonPropertyName("lastName")]
		public string LastName { get; set; }
	}
}

