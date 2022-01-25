using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Infrastructure;
using Shared.Persistence.Core.Features;
using Shared.Persistence.Mongo.Features;
using TheDungeonGuide.Persistence.Characters;

namespace TheDungeonGuide.Characters.Host;

[SuppressMessage("Design", "CC0091", MessageId = "Use static method")]
public class Startup
{
    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Environment   = environment;
        Configuration = configuration;
    }
    private IHostEnvironment Environment { get; }

    public IConfiguration Configuration { get; }
    /// <summary>
    ///     For integration testing set this to WebApplicationFactory Client
    /// </summary>
    public static HttpClient? UsersHttpClient { get; set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var identityUrl = UsersHttpClient is not null
                              ? UsersHttpClient.BaseAddress!.ToString()
                              : Configuration.GetValue<string>("IdentityUrl");
        services.AddControllers();

        services.AddAutoMapper(expression => { expression.AddProfile<PersistenceCharactersMapperProfile>(); });
        services.AddAuthentication(options =>
                                   {
                                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                       options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                                   }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                                                   {
                                                       options.Authority = identityUrl;
                                                       if (Environment.IsDevelopment())
                                                           options.RequireHttpsMetadata = false;
                                                       options.Audience  = "characters";
                                                       options.SaveToken = true;

                                                       options.TokenValidationParameters =
                                                           new TokenValidationParameters
                                                           {
                                                               NameClaimType = ClaimTypes.NameIdentifier,
                                                           };
                                                       if (UsersHttpClient is { } httpClient)
                                                           options.Backchannel = httpClient;
                                                   });
        services.AddSwaggerGen(c =>
                               {
                                   c.SwaggerDoc("v1", new OpenApiInfo
                                   {
                                       Title   = "Characters.Cube",
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
                                                       "characters.*", "All operations"
                                                   },
                                                   {
                                                       "characters.read", "Read operations"
                                                   },
                                                   {
                                                       "characters.write", "Write operations"
                                                   },
                                               },
                                           },
                                       },
                                   });
                                   c.OperationFilter<AuthorizeCheckOperationFilter>("characters");
                               });
        services.AddCors(options =>
                         {
                             options.AddPolicy("AnyOrigin", o =>
                                                            {
                                                                o.WithOrigins("https://localhost:5001",
                                                                              "https://localhost:5003",
                                                                              "https://localhost:5005")
                                                                 .AllowAnyOrigin()
                                                                 .AllowAnyHeader()
                                                                 .AllowAnyMethod();
                                                            });
                         });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.AddPersistenceCore();
        builder.AddPersistenceMongo();
        builder.AddCharactersMongoServices();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                             {
                                 c.SwaggerEndpoint("/swagger/v1/swagger.json", "Characters.Cube v1");
                                 c.OAuthClientId("characters");
                                 c.OAuthClientSecret("secret");
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