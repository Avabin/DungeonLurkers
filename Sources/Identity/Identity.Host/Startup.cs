using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AspNetCore.Identity.Mongo;
using Autofac;
using Identity.Infrastructure.Features.DevUser;
using Identity.Infrastructure.Features.IdentityServer;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Shared.Infrastructure;
using Shared.Persistence.Core.Features;
using Shared.Persistence.Identity;
using Shared.Persistence.Identity.Features.Roles;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features;

namespace Identity.Host;

[SuppressMessage("Design", "CC0091", MessageId = "Use static method")]
public class Startup
{
    private readonly string _validIssuer;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration = configuration;
        Environment   = environment;
        _validIssuer  = Configuration["JWT:ValidIssuer"] ?? "https://localhost";
    }

    public IConfiguration   Configuration { get; }
    public IHostEnvironment Environment   { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        if (Environment.IsDevelopment()) services.AddHostedService<InsertDevUserBackgroundService>();
        services.AddControllers();
        services.AddOptions()
                .Configure<MongoSettings>(Configuration.GetSection("MongoSettings"));
        services.AddAutoMapper(
            expression =>
            {
                expression.AddProfile<UsersMapperProfile>();
                expression.AddProfile<RolesMapperProfile>();
            });

        services
           .AddIdentityMongoDbProvider<UserDocument, RoleDocument, string>(
                options =>
                {
                    options.Password.RequiredLength = 8;
                },
                options =>
                {
                    options.ConnectionString =
                        Configuration
                           .GetConnectionString("MongoDb");
                })
           .AddDefaultTokenProviders()
           .AddMongoDbStores<UserDocument, RoleDocument, string>(
                options =>
                {
                    options.ConnectionString =
                        Configuration
                           .GetConnectionString("MongoDb");
                });

        services
           .AddIdentityServer()
           .AddAspNetIdentity<UserDocument>()
           .AddDeveloperSigningCredential()
           .AddInMemoryIdentityResources(new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            })
           .AddInMemoryApiResources(new List<ApiResource>
            {
                new()
                {
                    Name = IdentityServerConstants.LocalApi.ScopeName, Scopes = new List<string>
                        { IdentityServerConstants.LocalApi.ScopeName, "users.*", "users.read", "users.write" },
                },
                new()
                {
                    Name = "sessions", Scopes = new List<string> { "sessions.read", "sessions.write", "sessions.*" },
                },
                new()
                {
                    Name = "characters",
                    Scopes = new List<string> { "characters.read", "characters.write", "characters.*" },
                },
                new()
                {
                    Name = "pierogiesbot",
                    Scopes = new List<string> { "pierogiesbot.read", "pierogiesbot.write", "pierogiesbot.*" },
                },
            })
           .AddInMemoryApiScopes(new List<ApiScope>
            {
                new(IdentityServerConstants.LocalApi.ScopeName,
                    new List<string> { "user", "player", "gm", "admin" }),
                new("users.read", new[] { "user" }),
                new("users.write", new[] { "admin" }),
                new("users.*", new[] { "admin" }),
                new() { Name = "sessions.read", UserClaims = new List<string> { "player", "gm" } },
                new() { Name = "sessions.write", UserClaims = new List<string> { "gm" } },
                new() { Name = "sessions.*", UserClaims = new List<string> { "gm" } },
                new()
                {
                    Name = "characters.read", UserClaims = new List<string> { "user", "player", "gm", "admin" },
                },
                new() { Name = "characters.write", UserClaims = new List<string> { "gm", "admin" } },
                new() { Name = "characters.*", UserClaims = new List<string> { "admin" } },
                new()
                {
                    Name = "pierogiesbot.read", UserClaims = new List<string> { "user", "admin" },
                },
                new() { Name = "pierogiesbot.write", UserClaims = new List<string> { "admin" } },
                new() { Name = "pierogiesbot.*", UserClaims = new List<string> { "admin" } },
            })
           .AddInMemoryClients(new List<Client>
            {
                new()
                {
                    ClientId          = "default",
                    ClientName        = "Default",
                    AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                    ClientSecrets     = { new Secret("secret".Sha256()) },
                    AllowedScopes     = { IdentityServerConstants.LocalApi.ScopeName },
                },
                new()
                {
                    ClientId   = "sessions",
                    ClientName = "Sessions client",
                    AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes =
                    {
                        "sessions.read", "sessions.write", "sessions.*", "user.read",
                        IdentityServerConstants.LocalApi.ScopeName,
                    },
                },
                new()
                {
                    ClientId   = "characters",
                    ClientName = "Characters client",
                    AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes =
                    {
                        "characters.read", "characters.write", "characters.*", "user.read",
                        IdentityServerConstants.LocalApi.ScopeName,
                    },
                },
                new()
                {
                    ClientId   = "pierogiesbot",
                    ClientName = "PierogiesBot client",
                    AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes =
                    {
                        "pierogiesbot.read", "pierogiesbot.write", "pierogiesbot.*", "user.read",
                        IdentityServerConstants.LocalApi.ScopeName,
                    },
                },
            })
           .AddPersistedGrantStore<MongoPersistedGrantStore>()
           .AddProfileService<JwtProfileService>();

        services.AddLocalApiAuthentication();
        services.AddSwaggerGen(
            c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title   = "Identity",
                    Version = "v1",
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                c.IncludeXmlComments(xmlFilePath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.OAuth2,
                    // OpenIdConnectUrl = new Uri($"{_validIssuer}/.well-known/openid-configuration"),
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{_validIssuer}/connect/authorize"),
                            TokenUrl         = new Uri($"{_validIssuer}/connect/token"),
                            Scopes =
                            {
                                {
                                    IdentityServerConstants.LocalApi.ScopeName, "Identity API scope"
                                },
                            },
                        },
                    },
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>(IdentityServerConstants.LocalApi
                                                                    .ScopeName);
            });

        services.AddCors(
            options =>
            {
                options.AddPolicy(
                    "AnyOrigin",
                    o =>
                    {
                        o.WithOrigins("https://localhost:5001",
                                      "https://localhost:5003",
                                      "https://localhost:5005",
                                      "https://localhost:5007")
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
        builder.AddIdentityMongoServices();
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity v1");
                    c.OAuthClientId("default");
                    c.OAuthClientSecret("secret");
                });
        }

        app.UseHttpsRedirection();

        app.UseCors("AnyOrigin");
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}