using System;
using System.Collections.Generic;
using GraphQL.Server;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Validation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MusicFeed.PublicApiService.GraphQL;
using MusicFeed.PublicApiService.Interfaces;
using MusicFeed.PublicApiService.Internal;
using MusicFeed.PublicApiService.Settings;
using MusicFeed.UpdatesService.Client;

namespace MusicFeed.PublicApiService
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		private readonly IWebHostEnvironment environment;

		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<AppSettings>(configuration);

			AddSecurityServices(services);

			services.AddScoped<ApiSchema>();
			services.AddScoped<ApiQuery>();

			services.AddGraphQL()
				.AddSystemTextJson()
				.AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = environment.IsDevelopment())
				.AddDataLoader()
				.AddGraphTypes(ServiceLifetime.Scoped)
				.AddUserContextBuilder(context => new Dictionary<string, object>
				{
					{ "user", context.User },
				});

			services.AddHealthChecks();

			services.AddScoped<INewReleasesProvider, NewReleasesProvider>();

			services.AddUpdatesServiceClient(o =>
			{
				var settings = configuration.Get<AppSettings>();
				if (settings.Services.UpdatesServiceAddress == null)
				{
					throw new InvalidOperationException("The address of Updates service is not configured");
				}

				o.Address = settings.Services.UpdatesServiceAddress;
			});

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		}

		private void AddSecurityServices(IServiceCollection services)
		{
			services
				.AddAuthentication("Bearer")
				.AddJwtBearer("Bearer", options =>
				{
					var settings = configuration.Get<AppSettings>();

					var identityServiceAddress = settings.Services.IdentityServiceAddress;
					if (settings.Services.IdentityServiceAddress == null)
					{
						throw new InvalidOperationException("The address of Identity service is not configured");
					}

					options.RequireHttpsMetadata = false;
					options.Authority = identityServiceAddress.OriginalString;
					options.Audience = "musicfeed-api";

					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidateActor = true,
						ValidateTokenReplay = true,

						ValidTypes = new[] { "at+jwt" },
					};
				});

			services
				.AddTransient<IValidationRule, AuthorizationValidationRule>()
				.AddAuthorization(options =>
				{
					options.AddPolicy("HasMusicFeedApiScope", policy =>
					{
						policy.RequireAuthenticatedUser();
						policy.RequireClaim("scope", "musicfeed-api");
					});
				});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// We make service available at sub-path /api as a workaround for missing URL rewrite in ALB controller.
			// This could be removed when URL rewrite is supported by ALB controller.
			// Tracking issue: https://github.com/kubernetes-sigs/aws-load-balancer-controller/issues/1571
			// The service is still available at root path / - https://stackoverflow.com/questions/53429942/
			app.UsePathBase("/api");

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseGraphQL<ApiSchema>();
			app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
			{
				GraphQLEndPoint = "/api/graphql",
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
				{
					Predicate = check => check.Tags.Contains("ready"),
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
				});

				endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
				{
					Predicate = check => check.Tags.Contains("live"),
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
				});
			});
		}
	}
}
