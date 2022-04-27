using System.Net;
using System.Text.Json.Serialization;
using KBT.WebAPI.Training.Example.WebAPI.Utils;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.ApiResponses;

public class ApiResponseEntity
{
    [JsonPropertyName("status")] public int Status { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }

    public ApiResponseEntity(int status, string message)
    {
        Status = status;
        Message = message;
    }

    public ApiResponseEntity() : this((int)HttpStatusCode.OK, CommonMessages.SERVICE_SUCCESS)
    {
    }
}