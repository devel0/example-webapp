namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Enable swagger openapi v3 with xml comments and jwt bearer auth token button.
    /// </summary>    
    public static void AddSwaggerGenCustom(this IServiceCollection serviceCollection) => serviceCollection
        .AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v3", new OpenApiInfo { Title = "MyAPI", Version = "v3" });

            // requires <GenerateDocumentationFile> on csproj
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

            options.AddEnumsWithValuesFixFilters(o =>
            {
                // add schema filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema
                o.ApplySchemaFilter = true;

                // alias for replacing 'x-enumNames' in swagger document
                o.XEnumNamesAlias = "x-enum-varnames";

                // alias for replacing 'x-enumDescriptions' in swagger document
                o.XEnumDescriptionsAlias = "x-enum-descriptions";

                // add parameter filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema parameters
                o.ApplyParameterFilter = true;

                // add document filter to fix enums displaying in swagger document
                o.ApplyDocumentFilter = true;

                // add descriptions from DescriptionAttribute or xml-comments to fix enums (add 'x-enumDescriptions' or its alias from XEnumDescriptionsAlias for schema extensions) for applied filters
                o.IncludeDescriptions = true;

                // add remarks for descriptions from xml-comments
                o.IncludeXEnumRemarks = true;

                // get descriptions from DescriptionAttribute then from xml-comments
                o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;

                // new line for enum values descriptions
                // o.NewLine = Environment.NewLine;
                o.NewLine = "\n";
            });

            options.AddSecurityDefinition(WEB_Bearer, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = WEB_Bearer
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = WEB_Bearer
                        }
                    },
                    new string[]{}
                }
            });

            // Use method name as operationId
            options.CustomOperationIds(apiDesc =>
            {
                return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
            });
        });

    /// <summary>
    /// Configure swagger theme.
    /// </summary>
    public static (string swaggerUIPath, string swaggerEndpointPath)? ConfigSwagger(this WebApplication webApplication)
    {
        if (webApplication.Environment.IsDevelopment())
        {
            var logger = webApplication.Logger;

            var swaggerEndpointPath = SWAGGER_ENDPOINT_PATH;

            webApplication.UseSwagger();
            webApplication.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerEndpointPath, "ExampleWebAppAPI");
                c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
            });
            webApplication.MapGet("/swagger-ui/SwaggerDark.css", async (CancellationToken cancellationToken) =>
            {
                var css = await File.ReadAllBytesAsync(SWAGGER_CSS_PATH, cancellationToken);
                return Results.File(css, "text/css");
            }).ExcludeFromDescription();

            return (
                swaggerUIPath: SWAGGER_UI_PATH,
                swaggerEndpointPath
            );
        }

        return null;
    }

}