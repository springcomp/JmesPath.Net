using DevLab.JmesPath;
using DevLab.JmesPath.Functions;

[assembly: FunctionsStartup(typeof(Startup))]

public class Startup : FunctionsStartup
{
	public override void Configure(IFunctionsHostBuilder builder)
	{
		ConfigureServices(builder.Services);
	}

	private void ConfigureServices(IServiceCollection services)
	{
		// retrieve IConfiguration instance if necessary
		// to configure the Function App

		var configuration = services.GetConfiguration();

		var jmesPath = new JmesPath();
		jmesPath.FunctionRepository
			.Register<FromItemsFunction>()
			.Register<ItemsFunction>()
			.Register<LetFunction>()
			.Register<ZipFunction>()
			;

		services.AddSingleton(jmesPath);
	}
}

public static class IServiceCollectionConfigurationExtensions
{
	public static IConfiguration? GetConfiguration(this IServiceCollection services)
	{
		return services.BuildServiceProvider().GetService<IConfiguration>();
	}
}