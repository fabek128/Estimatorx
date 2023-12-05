using System.Text.Json;
using System.Text.Json.Serialization;

using EstimatorX.Core.Changes;
using EstimatorX.Core.Options;
using EstimatorX.Shared;
using EstimatorX.Shared.Changes;
using EstimatorX.Shared.Extensions;
using EstimatorX.Shared.Models;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

using SendGrid.Extensions.DependencyInjection;

using Serilog;
using Serilog.Events;

namespace EstimatorX.Service;

public static class Program
{
    public static int Main(string[] args)
    {
        // azure home directory
        var homeDirectory = Environment.GetEnvironmentVariable("HOME") ?? ".";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u1}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: $"{homeDirectory}/LogFiles/boot.txt",
                rollingInterval: RollingInterval.Day,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u1}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 10
            )
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web host");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, services, configure) => configure
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
            );

            var services = builder.Services;
            var configuration = builder.Configuration;
            ConfigureServices(services, configuration);

            var app = builder.Build();
            ConfigureMiddleware(app);

            app.Run();


            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }


    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddProblemDetails();
        services.AddMemoryCache();
        services.AddCosmosRepository();

        services.AddEstimatorXCore();
        services.AddEstimatorXShared();
        services.AddEstimatorXService();

        services.AddSendGrid((serviceProvider, options) =>
        {
            var sendGridOptions = serviceProvider.GetRequiredService<IOptions<SendGridConfiguration>>();
            options.ApiKey = sendGridOptions.Value.ApiKey.HasValue() ? sendGridOptions.Value.ApiKey : "***";
        });

        services.AddAutoMapper(typeof(HostingConfiguration).Assembly, typeof(AssemblyMetadata).Assembly);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.TypeInfoResolverChain.Add(JsonContext.Default);

            });

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "EstimatorX", Version = "v1" }));

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml", "application/octet-stream" });
        });

        services.AddSignalR(hubOptions => hubOptions.EnableDetailedErrors = true);

        services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => apiBehaviorOptions.SuppressModelStateInvalidFilter = true);
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseHsts();
            app.UseResponseCompression();
        }

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EstimatorX v1"));

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<ChangeFeedHub>(ChangeFeedConstants.HubPath);

        app.MapFallbackToFile("index.html");
    }
}
