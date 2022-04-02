using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.Models.Requests.Auths
{
	public class LoginReq
	{
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }

    }
}

