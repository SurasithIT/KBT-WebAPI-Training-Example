using System;
using System.Security.Claims;
using KBT.WebAPI.Training.Example.Entities.JWT;

namespace KBT.WebAPI.Training.Example.Services.Interfaces
{
	public interface IAuthService
	{
		List<Claim> GetPrincipalFromToken(string AccessToken, out string? userKey);

		JwtToken GetRefreshToken(string userCode, string refreshToken);

		bool GenerateToken(int userKey, List<Claim> claims, bool deleteOldToken, string refreshTokenOld, out string accessToken, out string refreshToken, out DateTime accessTokenExpire);

		int? GetUserKey();

		int? GetUserKeyFromHeader();
	}
}

