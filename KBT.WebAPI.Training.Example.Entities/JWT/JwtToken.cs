using System;
namespace KBT.WebAPI.Training.Example.Entities.JWT
{
	public class JwtToken
	{
        public int Key { get; set; }
        public int UserKey { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string AccessToken { get; set; }
    }
}

