using FurnitureStore.Server.SeedData;
using FurnitureStore.Server.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddSingleton((provider) =>
{
    var endpointUri = configuration["CosmosDbSettings:EndpointUri"];
    var primaryKey = configuration["CosmosDbSettings:PrimaryKey"];
    var databaseName = configuration["CosmosDbSettings:DatabaseName"];

    var cosmosClientOptions = new CosmosClientOptions
    {
        ApplicationName = databaseName,  // ========================================= FIRST RUN - comment =========================================
        ConnectionMode = ConnectionMode.Gateway
    };

    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    return new CosmosClient(endpointUri, primaryKey, cosmosClientOptions); 
});

builder.Services.AddTransient<ICosmosDbService, CosmosDbService>(); // ========================================= FIRST RUN - comment =========================================
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


//await CreateCosmosDbAsync(app.Services); // ========================================= FIRST RUN - uncomment =========================================


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

// FIRST RUN - uncomment all below
/* 
async Task CreateCosmosDbAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var cosmosClient = scope.ServiceProvider.GetRequiredService<CosmosClient>();
    var databaseName = configuration["CosmosDbSettings:DatabaseName"];

    var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync("carts", "/customerId");
    await database.Database.CreateContainerIfNotExistsAsync("customers", "/customerId");
    await database.Database.CreateContainerIfNotExistsAsync("orders", "/customerId");
    await database.Database.CreateContainerIfNotExistsAsync("products", "/productId");
    await database.Database.CreateContainerIfNotExistsAsync("categories", "/parent");
    await database.Database.CreateContainerIfNotExistsAsync("staffs", "/staffId");

    CosmosDbService dbService = new CosmosDbService(cosmosClient, configuration);
    DataSeeder seeder = new DataSeeder(dbService);

    await seeder.SeedDataAsync();
}
*/