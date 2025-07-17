using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PetApp.Common.Filters;

namespace PetApp.Common.Extensions;

public static class EndpointBuilderExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        where TRequest : class
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}