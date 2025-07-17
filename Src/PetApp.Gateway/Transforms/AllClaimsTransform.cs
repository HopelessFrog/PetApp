using System.Text.Json;
using PetApp.Common.Constants;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace PetApp.Gateway.Transforms;

public class AllClaimsTransform : ITransformProvider
{
    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(transformContext =>
        {
            var user = transformContext.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);
                var json = JsonSerializer.Serialize(claims);

                transformContext.ProxyRequest.Headers.Remove(ForwardedHeadersHttp.Claims);
                transformContext.ProxyRequest.Headers.Add(ForwardedHeadersHttp.Claims, json);
            }

            return ValueTask.CompletedTask;
        });
    }
}