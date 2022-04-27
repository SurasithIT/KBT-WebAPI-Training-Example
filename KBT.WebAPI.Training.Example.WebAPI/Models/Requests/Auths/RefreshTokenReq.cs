using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.Requests.Auths
{
	public class RefreshTokenReq
	{
        [Required]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
        [Required]
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }
}

