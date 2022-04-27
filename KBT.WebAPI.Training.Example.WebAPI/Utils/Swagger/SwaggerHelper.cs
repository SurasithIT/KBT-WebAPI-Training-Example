using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace KBT.WebAPI.Training.Example.WebAPI.Utils.Swagger
{
	public class SwaggerHelper
	{
        private static bool IsGenerateAuthorize = false;
        public static string ApiName;
        public static void ConfigureSwaggerGen(SwaggerGenOptions swaggerGenOptions)
        {
            //swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo());
            var webApiAssembly = Assembly.GetEntryAssembly();
            AddSwaggerDocPerVersion(swaggerGenOptions, webApiAssembly);
            ApplyDocInclusions(swaggerGenOptions);
            swaggerGenOptions.EnableAnnotations();
            //IncludeXmlComments(swaggerGenOptions);
        }

        public static void ConfigureSwagger(SwaggerOptions swaggerOptions)
        { }

        public static void ConfigureSwaggerUI(SwaggerUIOptions swaggerUIOptions)
        {
            var webApiAssembly = Assembly.GetEntryAssembly();
            var ApiDocuments = GetApiDocuments(webApiAssembly);
            foreach (var ApiDocument in ApiDocuments)
            {
                swaggerUIOptions.SwaggerEndpoint($"./swagger/{ApiDocument.DocumentName}/swagger.json",
                    ApiDocument.TitleName + " " + ApiDocument.Version);
            }
            swaggerUIOptions.RoutePrefix = string.Empty;
            swaggerUIOptions.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
        }

        private static void AddSwaggerDocPerVersion(SwaggerGenOptions swaggerGenOptions, Assembly webApiAssembly)
        {
            var ApiDocuments = GetApiDocuments(webApiAssembly);
            foreach (var ApiDocument in ApiDocuments)
            {
                swaggerGenOptions.SwaggerDoc($"{ApiDocument.DocumentName}",
                    new OpenApiInfo
                    {
                        Title = ApiName + " " + ApiDocument.TitleName + " API",
                        Version = $"{ApiDocument.Version}",
                        Description = ""
                    });
                //swaggerGenOptions.OperationFilter<AuthorizationOperationFilter>();
                if (ApiDocument.Authorize && !IsGenerateAuthorize)
                {
                    IsGenerateAuthorize = true;
                    swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Bearer {access token}",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>(){}
                        }
                    });
                }
            }
        }

        private static void ApplyDocInclusions(SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.DocInclusionPredicate((DocName, apiDesc) =>
            {
                if (apiDesc.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers
                    .ControllerActionDescriptor controller)
                {
                    string DocumentName = controller.ControllerTypeInfo.GetCustomAttributes()
                        .OfType<ApiDocumentAttribute>().Select(attr => attr.DocumentName).FirstOrDefault();
                    return DocName.Equals(DocumentName);
                }
                else
                {
                    return false;
                }
            });
        }

        private static IEnumerable<ApiDocumentAttribute> GetApiDocuments(Assembly webApiAssembly)
        {
            var ApiGroups = webApiAssembly.DefinedTypes
                .Where(x => x.GetCustomAttributes<ApiDocumentAttribute>().Any())
                .Select(y => y.GetCustomAttribute<ApiDocumentAttribute>())
                .Distinct()
                .OrderBy(x => x.DocumentName).ThenBy(x => x.Version);

            return ApiGroups;
        }
    }
}

