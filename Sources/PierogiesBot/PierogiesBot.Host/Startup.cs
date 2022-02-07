using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PierogiesBot.Discord.Infrastructure;
using PierogiesBot.Discord.Infrastructure.Features.DiscordHost;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Persistence.BotReactRules.Features;
using PierogiesBot.Persistence.BotResponseRules.Features;
using PierogiesBot.Persistence.GuildSettings.Features;
using Shared.Infrastructure;
using Shared.Persistence.Core.Features;
using Shared.Persistence.Mongo.Features;
using StartupBase = Shared.Infrastructure.StartupBase;

namespace PierogiesBot.Host;

[SuppressMessage("Design", "CC0091", MessageId = "Use static method")]
public class Startup : StartupBase
{
    private readonly string _validIssuer;

    public Startup(IConfiguration configuration, IHostEnvironment environment) : base(configuration, environment)
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var identityUrl = IdentityHttpClient is not null
                              ? IdentityHttpClient.BaseAddress!.ToString()
                              : Configuration.GetValue<string>("IdentityUrl");
        services.AddControllers();
        services.AddOptions()
                .Configure<MongoSettings>(Configuration.GetSection("MongoSettings"));
        
        services.AddDiscord(Configuration.GetSection(DiscordSettings.SectionName));

        services.AddAutoMapper(expression =>
        {
            expression.AddProfile<PersistenceBotCrontabRulesMapperProfile>();
            expression.AddProfile<PersistenceBotReactRulesMapperProfile>();
            expression.AddProfile<PersistenceBotMessageSubscriptionsMapperProfile>();
            expression.AddProfile<PersistenceBotResponseRulesMapperProfile>();
            expression.AddProfile<PersistenceGuildSettingsMapperProfile>();
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = identityUrl;
            if (Environment.IsDevelopment())
                options.RequireHttpsMetadata = false;
            options.Audience  = "pierogiesbot";
            options.SaveToken = true;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                };
            if (IdentityHttpClient is { } httpClient)
                options.Backchannel = httpClient;
        });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title   = "PierogiesBot",
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
                                "pierogiesbot.*", "All operations"
                            },
                            {
                                "pierogiesbot.read", "Read operations"
                            },
                            {
                                "pierogiesbot.write", "Write operations"
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
                o.WithOrigins("https://identity.pierogiesbot.tk", "https://api.pierogiesbot.tk")
                 .AllowAnyHeader()
                 .AllowAnyMethod();
            });
        });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        if(Configuration["IsDiscordEnabled"] == bool.TrueString)
            builder.AddDiscordServices();
        builder.AddPersistenceCore();
        builder.AddPersistenceMongo();
        builder.AddBotCrontabRulesMongoServices();
        builder.AddBotReactRulesMongoServices();
        builder.AddBotMessageSubscriptionsMongoServices();
        builder.AddBotResponseRulesMongoServices();
        builder.AddGuildSettingsMongoServices();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PierogiesBot v1");
                c.OAuthClientId("pierogiesbot");
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