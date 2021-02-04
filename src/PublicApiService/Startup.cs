using System;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
			services.AddScoped<ApiSchema>();
			services.AddScoped<ApiQuery>();

			services.AddScoped<INewReleasesProvider, NewReleasesProvider>();
			services.AddScoped<IDiagnosticsProvider, DiagnosticsProvider>();

			services.AddGraphQL()
				.AddSystemTextJson()
				.AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = environment.IsDevelopment())
				.AddDataLoader()
				.AddGraphTypes(ServiceLifetime.Scoped);

			services.Configure<AppSettings>(configuration);

			services.AddUpdatesServiceClient(o =>
			{
				var settings = configuration.Get<AppSettings>();
				if (settings.Services.UpdatesServiceAddress == null)
				{
					throw new InvalidOperationException("The address of UpdatesService is not configured");
				}

				o.Address = settings.Services.UpdatesServiceAddress;
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseGraphQL<ApiSchema>();
			app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
		}
	}
}
