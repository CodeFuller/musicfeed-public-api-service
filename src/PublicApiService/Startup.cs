using System;
using System.Collections.Generic;
using System.Threading;
using GraphQL.Server;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Server.Ui.Playground;
using GraphQL.Validation;
using HealthChecks.UI.Client;
using idunno.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublicApiService.GraphQL;
using PublicApiService.Interfaces;
using PublicApiService.Internal;
using PublicApiService.Settings;
using UpdatesService.Client;

namespace PublicApiService
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
					throw new InvalidOperationException("The address of UpdatesService is not configured");
				}

				o.Address = settings.Services.UpdatesServiceAddress;
			});

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		}

		private static void AddSecurityServices(IServiceCollection services)
		{
			services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
				.AddBasic(options =>
				{
					options.Events = new BasicAuthenticationEvents
					{
						OnValidateCredentials = context =>
						{
							var userChecker = context.HttpContext.RequestServices.GetRequiredService<IUserChecker>();
							return userChecker.CheckUser(context, CancellationToken.None);
						},
					};
				});

			services.AddTransient<IValidationRule, AuthorizationValidationRule>()
				.AddAuthorization(options =>
				{
					options.AddPolicy("IsAuthenticated", p => p.RequireAuthenticatedUser());
				});

			services.AddSingleton<IUserChecker, UserChecker>();
			services.AddSingleton<IUserAuthenticator, UserAuthenticator>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseGraphQL<ApiSchema>();
			app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());

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
