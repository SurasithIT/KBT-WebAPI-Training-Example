using System.Net;
using KBT.WebAPI.Training.Example.WebAPI.Models.ApiResponses;
using Microsoft.AspNetCore.Diagnostics;

namespace KBT.WebAPI.Training.Example.WebAPI.Utils.GlobalErrorHandling;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if(contextFeature != null)
                {
                    log4net.ILog logger = log4net.LogManager.GetLogger(contextFeature.Error.TargetSite.DeclaringType);
                    CommonUtility.GetLoggerThreadId();
                    
                    logger.Error(contextFeature.Error.Message, contextFeature.Error);
                    await context.Response.WriteAsJsonAsync(new ApiResponseEntity((int)HttpStatusCode.InternalServerError, CommonMessages.SERVICE_ERROR));
                }
            });
        });
    }
}