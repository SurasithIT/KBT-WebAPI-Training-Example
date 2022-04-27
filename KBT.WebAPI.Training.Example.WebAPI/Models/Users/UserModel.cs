using System;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.Users
{
	public class UserModel
	{
        [JsonPropertyName("userKey")]
        public int UserKey { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("employeeKey")]
        public int? EmployeeKey { get; set; }
    }
}

