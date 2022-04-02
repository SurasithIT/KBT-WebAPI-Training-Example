using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.Models.Employees;
using KBT.WebAPI.Training.Example.Models.Requests.Employees;
using KBT.WebAPI.Training.Example.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KBT.WebAPI.Training.Example.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EmployeesController));
        private readonly DemoDbContext _demoDbContext;

        public EmployeesController(DemoDbContext demoDbContext)
        {
            _demoDbContext = demoDbContext;
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get all employees")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<EmployeeModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetEmployees()
        {
            object result = new object();
            try
            {
                List<EmployeeModel> employees = _demoDbContext.Employees.Select(emp => new EmployeeModel
                {
                    EmployeeKey = emp.EmployeeKey,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                }).ToList();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
                    data = employees
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

        [HttpGet("{employeeKey}")]
        [SwaggerOperation(summary: "Get employee by employeeKey")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EmployeeModel))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetEmployeeByKey(int employeeKey)
        {
            object result = new object();
            try
            {
                EmployeeModel? employee = _demoDbContext.Employees
                                                        .Where(emp => emp.EmployeeKey == employeeKey)
                                                        .Select(emp => new EmployeeModel
                                                        {
                                                            EmployeeKey = emp.EmployeeKey,
                                                            FirstName = emp.FirstName,
                                                            LastName = emp.LastName,
                                                        })
                                                        .FirstOrDefault();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
                    data = employee
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
        [SwaggerOperation(summary: "Create Employee")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.Continue)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult CreateEmployee(EmployeeReq employeeReq)
        {
            object result = new object();
            try
            {
                Employee? duplicateEmp = _demoDbContext.Employees.FirstOrDefault(emp => emp.FirstName.Trim().ToLower() == employeeReq.FirstName.Trim().ToLower()
                                                                                        && emp.LastName.Trim().ToLower() == employeeReq.LastName.Trim().ToLower());
                if (duplicateEmp != null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.DUPLICATED_NAME,
                    };
                    return Ok(result);
                }

                Employee newEmployee = new Employee()
                {
                    FirstName = employeeReq.FirstName,
                    LastName = employeeReq.LastName,
                };
                _demoDbContext.Employees.Add(newEmployee);
                _demoDbContext.SaveChanges();

                result = new
                {
                    status = (int)HttpStatusCode.OK,
                    message = CommonMessages.SERVICE_SUCCESS,
                    data = newEmployee
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

        [HttpPut("{employeeKey}")]
        [SwaggerOperation(summary: "Update Employee")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.Continue)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult UpdateEmployee(int employeeKey, EmployeeReq employeeReq)
        {
            object result = new object();
            try
            {
                Employee? employee = _demoDbContext.Employees.FirstOrDefault(emp => emp.EmployeeKey == employeeKey);
                if (employee == null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.NotFound,
                        message = CommonMessages.NOT_FOUND_DATA,
                    };
                    return Ok(result);
                }

                Employee? duplicateEmp = _demoDbContext.Employees.FirstOrDefault(emp => emp.FirstName.Trim().ToLower() == employeeReq.FirstName.Trim().ToLower()
                                                                                        && emp.LastName.Trim().ToLower() == employeeReq.LastName.Trim().ToLower()
                                                                                        && emp.EmployeeKey != employeeKey);
                if (duplicateEmp != null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.Continue,
                        message = CommonMessages.DUPLICATED_NAME,
                    };
                    return Ok(result);
                }

                employee.FirstName = employeeReq.FirstName;
                employee.LastName = employeeReq.LastName;

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

        [HttpDelete("{employeeKey}")]
        [SwaggerOperation(summary: "Delete Employee by employeeKey")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public IActionResult DeleteEmployee(int employeeKey)
        {
            object result = new object();
            try
            {
                Employee? employee = _demoDbContext.Employees.FirstOrDefault(emp => emp.EmployeeKey == employeeKey);
                if (employee == null)
                {
                    result = new
                    {
                        status = (int)HttpStatusCode.NotFound,
                        message = CommonMessages.NOT_FOUND_DATA,
                    };
                    return Ok(result);
                }

                _demoDbContext.Employees.Remove(employee);
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

