﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.WebAPI.Models.ApiResponses;
using KBT.WebAPI.Training.Example.WebAPI.Models.Requests.Users;
using KBT.WebAPI.Training.Example.WebAPI.Models.Users;
using KBT.WebAPI.Training.Example.WebAPI.Services.Interfaces;
using KBT.WebAPI.Training.Example.WebAPI.Utils;
using KBT.WebAPI.Training.Example.WebAPI.Utils.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KBT.WebAPI.Training.Example.WebAPI.Controllers
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(DemoDbContext demoDbContext, IMapper mapper, IUserService userService)
        {
            _demoDbContext = demoDbContext;
            _mapper = mapper;
            _userService = userService;
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
                IEnumerable<User> _users = _demoDbContext.Users;
                List<UserModel> users = _mapper.Map<IEnumerable<UserModel>>(_users).ToList();
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
                User _user = _demoDbContext.Users.Where(user => user.UserKey == userKey).FirstOrDefault();
                UserModel user = _mapper.Map<UserModel>(_user);
                
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


                User newUser = _mapper.Map<User>(userReq);
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
        
        [HttpGet("FromDatabaseFactory")]
        [SwaggerOperation(summary: "Get all users from database factory")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ApiResponseWithDataEntity<List<UserModel>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public IActionResult GetUsersFromDatabaseFactory()
        {
            object result = new object();

                List<User> _users = _userService.GetUsers().ToList();
                List<UserModel> users = _mapper.Map<List<User>, List<UserModel>>(_users);

                return Ok(new ApiResponseWithDataEntity<List<UserModel>>(users));
        }
    }
}

