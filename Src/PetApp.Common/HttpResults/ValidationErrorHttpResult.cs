using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;

namespace PetApp.Common.HttpResults;

public class ValidationErrorHttpResult : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IContentTypeHttpResult, IValueHttpResult, IValueHttpResult<HttpValidationProblemDetails>
{
    private readonly ValidationProblem problem;

    public ValidationErrorHttpResult(string errorMessage)
    {
        problem = TypedResults.ValidationProblem
        (
            errors: new Dictionary<string, string[]>(),
            detail: errorMessage
        );
    }

    public int? StatusCode => problem.StatusCode;
    public string? ContentType => problem.ContentType;
    public object? Value => problem.ProblemDetails;
    HttpValidationProblemDetails? IValueHttpResult<HttpValidationProblemDetails>.Value => problem.ProblemDetails;

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status400BadRequest, typeof(HttpValidationProblemDetails), ["application/problem+json"]));
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await problem.ExecuteAsync(httpContext);
    }
}