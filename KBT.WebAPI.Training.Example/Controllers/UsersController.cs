using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.Models.Requests.Users;
using KBT.WebAPI.Training.Example.Models.Users;
using KBT.WebAPI.Training.Example.Utils;
using KBT.WebAPI.Training.Example.Utils.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KBT.WebAPI.Training.Example.Controllers
{
    [Authorize]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiDocument("User", "User", true)]
    public class UsersController : Controller
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(UsersController));
        private readonly DemoDbContext _demoDbContext;

        public UsersController(DemoDbContext demoDbContext)
        {
            _demoDbContext = demoDbContext;
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get all users")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<UserModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetUsers()
        {
            object result = new object();
            try
            {
                List<UserModel> users = _demoDbContext.Users.Select(user => new UserModel
                {
                    UserKey = user.UserKey,
                    UserName = user.UserName,
                    IsActive = user.IsActive,
                    EmployeeKey = (int)user.EmployeeKey
                }).ToList();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
                    data = users
                };
               return Ok(result);
            }
            catch(Exception ex)
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

        [HttpGet("{userKey}")]
        [SwaggerOperation(summary: "Get user by userKey")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(UserModel))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetUserByKey(int userKey)
        {
            object result = new object();
            try
            {
                UserModel? user = _demoDbContext.Users
                                    .Where(user => user.UserKey == userKey)
                                    .Select(user => new UserModel
                                    {
                                        UserKey = user.UserKey,
                                        UserName = user.UserName,
                                        IsActive = user.IsActive,
                                        EmployeeKey = (int)user.EmployeeKey
                                    })
                                    .FirstOrDefault();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
                    data = user
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

        [HttpPost]
        [SwaggerOperation(summary: "Create User")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.Continue)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public IActionResult CreateUser(UserReq userReq)
        {
            object result = new object();
            try
            {
                User? duplicateUser = _demoDbContext.Users.FirstOrDefault(user => user.UserName.Trim().ToLower() == userReq.UserName.Trim().ToLower());
                if(duplicateUser != null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.DUPLICATED_USERNAME,
                    };
                    return Ok(result);
                }

                User newUser = new User()
                {
                    UserName = userReq.UserName,
                    Password = userReq.Password,
                    IsActive = userReq.IsActive ?? false,
                    EmployeeKey = userReq.EmployeeKey
                };
                _demoDbContext.Users.Add(newUser);
                _demoDbContext.SaveChanges();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
                    data = newUser
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

        [HttpPut("{userKey}")]
        [SwaggerOperation(summary: "Update User")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.Continue)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult UpdateUser(int userKey, UserReq userReq)
        {
            object result = new object();
            try
            {
                User? user = _demoDbContext.Users.FirstOrDefault(user => user.UserKey == userKey);
                if (user == null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.NotFound,
                        message = CommonMessages.NOT_FOUND_DATA,
                    };
                    return Ok(result);
                }

                User? duplicateUser = _demoDbContext.Users.FirstOrDefault(user => user.UserName.Trim().ToLower() == userReq.UserName.Trim().ToLower()
                                                                                && user.UserKey != userKey);
                if (duplicateUser != null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.DUPLICATED_USERNAME,
                    };
                    return Ok(result);
                }

                user.UserName = userReq.UserName;
                user.Password = userReq.Password;
                user.IsActive = userReq.IsActive ?? false;
                user.EmployeeKey = userReq.EmployeeKey;

                _demoDbContext.SaveChanges();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
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

        [HttpDelete("{userKey}")]
        [SwaggerOperation(summary: "Delete user by userKey")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult DeleteUser(int userKey)
        {
            object result = new object();
            try
            {
                User? user = _demoDbContext.Users.FirstOrDefault(user => user.UserKey == userKey);
                if (user == null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.NotFound,
                        message = CommonMessages.NOT_FOUND_DATA,
                    };
                    return Ok(result);
                }

                _demoDbContext.Users.Remove(user);
                _demoDbContext.SaveChanges();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
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

