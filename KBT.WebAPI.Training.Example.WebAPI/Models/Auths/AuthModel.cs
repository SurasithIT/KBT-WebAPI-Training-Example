using System;
using System.Text.Json.Serialization;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.Auths
{
	public class AuthModel
	{
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("accessTokenExpire")]
        public DateTime AccessTokenExpire { get; set; }
    }
}

