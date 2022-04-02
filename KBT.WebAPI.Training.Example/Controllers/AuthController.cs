using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.Entities.JWT;
using KBT.WebAPI.Training.Example.Models.Auths;
using KBT.WebAPI.Training.Example.Models.Requests.Auths;
using KBT.WebAPI.Training.Example.Services.Interfaces;
using KBT.WebAPI.Training.Example.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KBT.WebAPI.Training.Example.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AuthController));

        private readonly IConfiguration _configuration;
        private readonly JwtDbContext _jwtDbContext;
        private readonly DemoDbContext _demoDbContext;
        private IAuthService _authService;
        private IHttpContextAccessor _httpContextAccessor;

        public AuthController(
            IConfiguration configuration,
            JwtDbContext jwtDbContext,
            DemoDbContext demoDbContext,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _configuration = configuration;
            _jwtDbContext = jwtDbContext;
            _demoDbContext = demoDbContext;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            CommonUtility.GetLoggerThreadId();
        }


        [HttpPost("login")]
        [SwaggerOperation(summary: "Login")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthModel))]
        [SwaggerResponse((int)HttpStatusCode.Continue)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult Login([FromBody] LoginReq req)
        {
            var result = new object();
            try
            {
                User? user = _demoDbContext.Users.FirstOrDefault(user => user.UserName.Trim().ToLower() == req.Username.Trim().ToLower()
                                                                        && user.Password == req.Password);

                if(user == null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.SECURITY_INCORRECT_AUTH,
                    };
                    return Ok(result);
                }

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserKey.ToString()));
                claims.Add(new Claim(ClaimTypes.GivenName, user.UserName.ToString()));

                string accessToken = "";
                string refreshToken = "";
                DateTime accessTokenExpire;
                bool isSuccess = _authService.GenerateToken(user.UserKey, claims, true, null, out accessToken, out refreshToken, out accessTokenExpire);
                if (isSuccess)
                {
                    logger.Debug($"========== {req.Username} Login Success ==========");

                    result = new
                    {
                        status = (int)HttpStatusCode.OK,
                        message = CommonMessages.SERVICE_SUCCESS,
                        data = new AuthModel
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                            AccessTokenExpire = accessTokenExpire,
                        }
                    };
                    return Ok(result);
                }
                else
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.SECURITY_CANNOT_AUTH_ERROR,
                    };
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                result = new
                {
                    status = (int)HttpStatusCode.InternalServerError,
                    message = CommonMessages.SERVICE_ERROR,
                };
                return Ok(result);
            }
        }

        [HttpPost("refresh")]
        [SwaggerOperation(summary: "refresh token")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthModel))]
        [SwaggerResponse((int)HttpStatusCode.Continue)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult Refresh([FromBody] RefreshTokenReq req)
        {
            var result = new object();
            try
            {
                _authService.GetPrincipalFromToken(req.AccessToken, out string? userKeyStr);
                if(!(Int32.TryParse(userKeyStr, out int userKey)))
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.SECURITY_CANNOT_AUTH_ERROR,
                    };
                    return Ok(result);
                }

                JwtToken JwtToken = _authService.GetRefreshToken((int)userKey, req.RefreshToken);
                if (JwtToken == null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.SECURITY_CANNOT_AUTH_ERROR,
                    };
                    return Ok(result);
                }

                User? user = _demoDbContext.Users.FirstOrDefault(user => user.UserKey == userKey);
                if (user == null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.SECURITY_CANNOT_AUTH_ERROR,
                    };
                    return Ok(result);
                }

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserKey.ToString()));
                claims.Add(new Claim(ClaimTypes.GivenName, user.UserName.ToString()));

                _authService.GenerateToken((int)userKey, claims, false, req.RefreshToken, out string AccessToken, out string RefreshToken, out DateTime AccessTokenExpire);

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
                    data = new AuthModel
                    {
                        AccessToken = AccessToken,
                        RefreshToken = RefreshToken,
                        AccessTokenExpire = AccessTokenExpire
                    }
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                result = new
                {
                    status = (int)HttpStatusCode.InternalServerError,
                    message = CommonMessages.SERVICE_ERROR,
                };
                return Ok(result);
            }
        }

        [HttpPost("logout")]
        [SwaggerOperation(summary: "Logout")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult Logout([FromBody] LogoutReq req)
        {
            var result = new object();
            try
            {
                List<JwtToken> jwtToken = _jwtDbContext.JwtToken.Where(t => t.UserKey == req.UserKey).ToList();
                if (jwtToken != null)
                {
                    _jwtDbContext.JwtToken.RemoveRange(jwtToken);
                    _jwtDbContext.SaveChanges();
                }

                logger.Debug($"{req.UserKey} Logout Success!");
                
                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS
                };
                return Ok(result);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                result = new
                {
                    status = (int)HttpStatusCode.InternalServerError,
                    message = CommonMessages.SERVICE_ERROR,
                };
                return Ok(result);
            }
        }
    }
}

