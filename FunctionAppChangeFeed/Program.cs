using FunctionAppChangeFeed.IRepositories;
using FunctionAppChangeFeed.Repositories;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton((s) => {

            CosmosClientOptions options = new()
            {
                ApplicationName = context.Configuration.GetValue<string>("DatabaseName"),
                ConnectionMode = ConnectionMode.Gateway
            };

            return new CosmosClient(context.Configuration.GetValue<string>("CosmosDbConnectionString"), options);
        });
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
    })
    .Build();

host.Run();
