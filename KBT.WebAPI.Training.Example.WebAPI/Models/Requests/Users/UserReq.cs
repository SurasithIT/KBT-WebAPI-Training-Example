using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.Requests.Users
{
    public class UserReq
    {
        [Required]
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("employeeKey")]
        public int? EmployeeKey { get; set; }
    }
}

