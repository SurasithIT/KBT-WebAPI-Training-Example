using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KBT.WebAPI.Training.Example.Entities.JWT;
using KBT.WebAPI.Training.Example.WebAPI.Services.Interfaces;
using KBT.WebAPI.Training.Example.WebAPI.Utils;
using Microsoft.IdentityModel.Tokens;

namespace KBT.WebAPI.Training.Example.WebAPI.Services
{
    public class AuthService : IAuthService
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AuthService));
        private readonly IConfiguration _configuration;
        private JwtDbContext _jwtDbContext;
        private readonly IHttpContextAccessor _context;

        public AuthService(IConfiguration configuration, JwtDbContext jwtDbContext, IHttpContextAccessor context)
        {
            _configuration = configuration;
            _jwtDbContext = jwtDbContext;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            CommonUtility.GetLoggerThreadId();
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string GenerateAccessToken(IEnumerable<Claim> Claims, out string RefreshToken, out DateTime TokenExpire)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var Secret = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

                var ClaimsDictionary = new Dictionary<string, object>();

                foreach (var claim in Claims)
                {
                    ClaimsDictionary.Add(claim.Type, claim.Value);
                }
                DateTime NotBefore = DateTime.Now;
                double expired = 0;
                expired = Convert.ToDouble(_configuration["JWT:Expire"]);
                TokenExpire = NotBefore.AddSeconds(expired);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(Claims),
                    Claims = ClaimsDictionary,
                    NotBefore = NotBefore,
                    Expires = TokenExpire,
                    SigningCredentials =
                        new SigningCredentials(
                            new SymmetricSecurityKey(Secret),
                            SecurityAlgorithms.HmacSha256Signature)
                };

                var Token = tokenHandler.CreateToken(tokenDescriptor);
                RefreshToken = GenerateRefreshToken();
                string AccessToken = tokenHandler.WriteToken(Token);

                return AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Claim> GetPrincipalFromToken(string AccessToken, out string? userKey)
        {
            try
            {
                var Secret = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

                var TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Secret)
                };

                var TokenHandler = new JwtSecurityTokenHandler();
                SecurityToken SecurityToken;
                var Principal = TokenHandler.ValidateToken(AccessToken, TokenValidationParameters, out SecurityToken);
                var JWTSecurityToken = SecurityToken as JwtSecurityToken;
                if (JWTSecurityToken == null || !JWTSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    userKey = null;
                    return null;
                }

                userKey = ((ClaimsIdentity)Principal.Identity).Claims
                            .Where(c => c.Type == ClaimTypes.NameIdentifier)
                            .Select(c => c.Value).FirstOrDefault();

                return ((ClaimsIdentity)Principal.Identity).Claims.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                userKey = null;
                return null;
            }
        }

        public JwtToken GetRefreshToken(int userKey, string refreshToken)
        {
            var result = new object();
            try
            {
                JwtToken jwtToken = _jwtDbContext.JwtToken
                    .Where(t => t.UserKey == userKey && t.RefreshToken == refreshToken)
                    .FirstOrDefault();

                //if (DateTime.Now > JwtToken.Expiration)
                //{
                //    return null;
                //}

                return jwtToken;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return null;
            }
        }

        public bool GenerateToken(int userKey, List<Claim> claims, bool deleteOldToken, string refreshTokenOld, out string accessToken, out string refreshToken, out DateTime accessTokenExpire)
        {
            try
            {
                if (deleteOldToken)
                {
                    List<JwtToken> jwtTokenOld = _jwtDbContext.JwtToken
                        .Where(t => t.UserKey == userKey)
                        .ToList();
                    if (jwtTokenOld != null && jwtTokenOld.Count > 0)
                    {
                        _jwtDbContext.JwtToken.RemoveRange(jwtTokenOld);
                        _jwtDbContext.SaveChanges();
                    }
                }


                accessToken = GenerateAccessToken(claims, out refreshToken, out accessTokenExpire);

                JwtToken jwtToken = new JwtToken()
                {
                    UserKey = userKey,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Expiration = accessTokenExpire
                };

                _jwtDbContext.JwtToken.Add(jwtToken);
                _jwtDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                accessToken = null;
                refreshToken = null;
                accessTokenExpire = DateTime.MinValue;

                return false;
            }
        }

        public int? GetUserKey()
        {
            try
            {
                var userKey = _context.HttpContext.User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value;

                if( int.TryParse(userKey, out int result)){
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return null;
                //throw ex;
            }
        }

        public int? GetUserKeyFromHeader()
        {
            try
            {
                var AuthHeaderVal = AuthenticationHeaderValue.Parse(_context.HttpContext.Request.Headers["Authorization"]);
                var principal = GetPrincipalFromToken(AuthHeaderVal.Parameter, out string? userKey);
                if (int.TryParse(userKey, out int result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return null;
                //throw ex;
            }
        }
    }
}

