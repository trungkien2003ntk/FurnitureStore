using Azure.Storage.Blobs;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Repositories;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Server.SeedData;
using FurnitureStore.Server.Services;
using FurnitureStore.Server.Utils;
using FurnitureStore.Server.Validators.BindingModels;
using Microsoft.AspNetCore.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
var configuration = builder.Configuration;

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


builder.Services.AddScoped(_ => {
    //var accountUri = new Uri(!);
    return new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
});


builder.Services.AddTransient<AzureSearchServiceFactory>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() // or AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});

// add custom services
builder.Services.AddTransient<IFileService, FileService>();

// add repositories
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IStaffRepository, StaffRepository>();

// add validators
builder.Services.AddTransient<IValidator<QueryParameters>, QueryParametersValidator>();


builder.Services.AddTransient<DataSeeder>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Seed data if database is successfully created
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    // Create instance of CosmosClient
    var cosmosClient = scope.ServiceProvider.GetRequiredService<CosmosClient>();

    // Get the Database Name
    var databaseName = cosmosClient.ClientOptions.ApplicationName;


    //Create the database with autoscale enabled
    var response = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);

    // Logging
    if (response.StatusCode == HttpStatusCode.Created)
        app.Logger.LogInformation($"Database {databaseName} created");
    else
        app.Logger.LogInformation($"Database {databaseName} had already created before");

    // Get the database
    var database = cosmosClient.GetDatabase(databaseName);


    if (database != null)
    {
        bool emptyContainerCreated = await EnsureContainersAreCreatedAsync(database);

        if (emptyContainerCreated)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

            await seeder.SeedDataAsync();
        }
    }
}



app.UseSwagger();
app.UseSwaggerUI();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // using static System.Net.Mime.MediaTypeNames;
            context.Response.ContentType = Text.Plain;

            await context.Response.WriteAsync("An exception was thrown.");

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
            {
                await context.Response.WriteAsync(" The file was not found.");
            }

            if (exceptionHandlerPathFeature?.Path == "/")
            {
                await context.Response.WriteAsync(" Page: Home.");
            }
        });
    });

    app.UseHsts();
}

app.UseCors();

app.UseCors("corspolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


async Task<bool> EnsureContainersAreCreatedAsync(Database database)
{
    var containersToCreate = new[]
    {
        ("orders", "/yearMonth"),
        ("categories", "/parentPath"),
        ("staffs", "/staffId"),
        ("products", "/sku"),
        ("variations", "/variationId")
    };

    foreach (var (containerName, partitionKeyPath) in containersToCreate)
    {
        var statusCode = await GetContainerCreationStatusCode(database, containerName, partitionKeyPath);

        if (statusCode != HttpStatusCode.Created)
        {
            return false;
        }
    }

    return true;
}

async Task<HttpStatusCode> GetContainerCreationStatusCode(Database database, string containerName, string partitionKeyPath)
{
    ContainerProperties properties = new()
    {
        Id = containerName,
        PartitionKeyPath = partitionKeyPath,
        // Expire all documents after 90 days
        DefaultTimeToLive = -1
    };

    var response = await database.CreateContainerIfNotExistsAsync(properties);

    if (response.StatusCode == HttpStatusCode.Created)
        app.Logger.LogInformation($"Container {containerName} created");
    else
        app.Logger.LogInformation($"Container {containerName} had already created before");

    return response.StatusCode;
}