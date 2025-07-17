using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PetApp.Gateway.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Swagger;

namespace PetApp.Gateway.Configs;

public class ConfigureSwaggerOptions(
    IOptionsMonitor<ReverseProxyDocumentFilterConfig> reverseProxyDocumentFilterConfigOptions)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly ReverseProxyDocumentFilterConfig _reverseProxyDocumentFilterConfig =
        reverseProxyDocumentFilterConfigOptions.CurrentValue;

    public void Configure(SwaggerGenOptions options)
    {
        var filterDescriptors = new List<FilterDescriptor>();

        options.ConfigureSwaggerDocs(_reverseProxyDocumentFilterConfig);

        filterDescriptors.Add(new FilterDescriptor
        {
            Type = typeof(ReverseProxyDocumentFilter),
            Arguments = Array.Empty<object>()
        });

        options.DocumentFilterDescriptors = filterDescriptors;
    }
}