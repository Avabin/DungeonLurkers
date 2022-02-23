using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Infrastructure;
using Shared.MessageBroker.Core;
using Shared.Persistence.Core.Features;
using Shared.Persistence.Mongo.Features;
using TheDungeonGuide.Persistence.Sessions;

namespace TheDungeonGuide.Sessions.Host;

[SuppressMessage("Design", "CC0091", MessageId = "Use static method")]
public class Startup
{
    private readonly string _clientSecret;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration = configuration;
        Environment   = environment;
        
        _clientSecret = Configuration["JWT:Secret"] ?? "secret";
    }

    /// <summary>
    ///     For integration testing set this to WebApplicationFactory Client
    /// </summary>
    public static HttpClient? IdentityHttpClient { get; set; }

    public IConfiguration   Configuration { get; }
    public IHostEnvironment Environment   { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var identityUrl = IdentityHttpClient is not null
                              ? IdentityHttpClient.BaseAddress!.ToString()
                              : Configuration.GetValue<string>("IdentityUrl");
        services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = identityUrl;
            if (Environment.IsDevelopment())
                options.RequireHttpsMetadata = false;
            options.Audience  = "sessions";
            options.SaveToken = true;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                };
            if (IdentityHttpClient is not null)
                options.Backchannel = IdentityHttpClient;
        });
        services.AddAutoMapper(expression => { expression.AddProfile<PersistenceSessionsMapperProfile>(); });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "Sessions.Cube",
                Version = "v1",
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{identityUrl}connect/authorize"),
                        TokenUrl         = new Uri($"{identityUrl}connect/token"),
                        Scopes =
                        {
                            {
                                "sessions.*", "All operations"
                            },
                            {
                                "sessions.read", "Read operations"
                            },
                            {
                                "sessions.write", "Write operations"
                            },
                        },
                    },
                },
            });
            c.OperationFilter<AuthorizeCheckOperationFilter>("sessions");
        });
        services.AddCors(options =>
        {
            options.AddPolicy("AnyOrigin", o =>
            {
                o.WithOrigins("https://identity.pierogiesbot.tk",
                              "https://identity.avabin.tk",
                              "https://avabin.tk",
                              "https://api.pierogiesbot.tk",
                              "https://pierogiesbot.avabin.tk",
                              "https://sessions.tdg.avabin.tk",
                              "https://characters.tdg.avabin.tk",
                              "https://localhost:5001",
                              "https://localhost", 
                              "https://app.localhost",
                              "https://localhost:5003",
                              "https://localhost:5005",
                              "https://localhost:5007")
                 .AllowAnyHeader()
                 .AllowAnyMethod();
            });
        });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.AddPersistenceCore();
        builder.AddPersistenceMongo();
        builder.AddSessionsMongoServices();
        
        builder.RegisterType<DocumentMessageBroker>().AsImplementedInterfaces().SingleInstance();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var pathBase = Configuration["PathBase"];
        if (!string.IsNullOrWhiteSpace(pathBase)) app.UsePathBase(pathBase);
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{pathBase ?? ""}/swagger/v1/swagger.json", "Sessions API v1");
                c.OAuthClientId("sessions");
                c.OAuthClientSecret(_clientSecret);
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AnyOrigin");

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}