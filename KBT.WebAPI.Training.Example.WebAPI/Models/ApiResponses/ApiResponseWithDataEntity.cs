using System.Net;
using System.Text.Json.Serialization;
using KBT.WebAPI.Training.Example.WebAPI.Utils;

namespace KBT.WebAPI.Training.Example.WebAPI.Models.ApiResponses;

public class ApiResponseWithDataEntity<T>
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("data")]
    public T Data { get; set; }

    public ApiResponseWithDataEntity(int status, string message, T data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public ApiResponseWithDataEntity(T data) : this((int)HttpStatusCode.OK, CommonMessages.SERVICE_SUCCESS, data)
    {
        Data = data;
    }
}