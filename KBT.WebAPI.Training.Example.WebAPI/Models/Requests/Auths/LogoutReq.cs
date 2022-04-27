using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.Requests.Auths
{
	public class LogoutReq
	{
		[Required]
		[JsonPropertyName("userKey")]
		public int UserKey { get; set; }
	}
}

