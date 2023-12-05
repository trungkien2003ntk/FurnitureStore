using FurnitureStore.Server.SeedData;
using FurnitureStore.Server.Services;
using Microsoft.Azure.Cosmos;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var configuration = builder.Configuration;

// Add services to the container.

// CosmosClient dependency injection
builder.Services.AddSingleton((provider) =>
{
    var endpointUri = configuration["CosmosDbSettings:EndpointUri"];
    var primaryKey = configuration["CosmosDbSettings:PrimaryKey"];
    var databaseName = configuration["CosmosDbSettings:DatabaseName"];

    var cosmosClientOptions = new CosmosClientOptions
    {
        ApplicationName = databaseName,
        ConnectionMode = ConnectionMode.Gateway
    };

    LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    return new CosmosClient(endpointUri, primaryKey, cosmosClientOptions); 
});

// Create database
builder.Services.AddSingleton<ICosmosDbService>(provider =>
{
    var cosmosClient = provider.GetRequiredService<CosmosClient>();
    var databaseName = configuration["CosmosDbSettings:DatabaseName"];

    cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);

    return new CosmosDbService(cosmosClient, configuration);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data if database is successfully created
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var cosmosClient = scope.ServiceProvider.GetRequiredService<CosmosClient>();
    // Call this here to initialize database.
    var cosmosDbService = scope.ServiceProvider.GetRequiredService<ICosmosDbService>();
    var database = cosmosClient.GetDatabase(configuration["CosmosDbSettings:DatabaseName"]);
    var seeder = new DataSeeder(cosmosDbService);

    if (database != null)
    {
        // If the containers has been initialized, do not seed data.
        bool dataHaveNotBeenSeeded = 
            (await database.CreateContainerIfNotExistsAsync("carts", "/customerId")).StatusCode == HttpStatusCode.Created &&
            (await database.CreateContainerIfNotExistsAsync("customers", "/customerId")).StatusCode == HttpStatusCode.Created &&
            (await database.CreateContainerIfNotExistsAsync("orders", "/customerId")).StatusCode == HttpStatusCode.Created &&
            (await database.CreateContainerIfNotExistsAsync("categories", "/parent")).StatusCode == HttpStatusCode.Created &&
            (await database.CreateContainerIfNotExistsAsync("staffs", "/staffId")).StatusCode == HttpStatusCode.Created &&
            (await database.CreateContainerIfNotExistsAsync("products", "/productId")).StatusCode == HttpStatusCode.Created;

        if (dataHaveNotBeenSeeded)
        {
            await seeder.SeedDataAsync();
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();