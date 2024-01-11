using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CosmosChangeFeedFunction
{
    public class CFCategoryFunction
    {
        private readonly ILogger _logger;

        public CFCategoryFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CFCategoryFunction>();
        }

        [Function("CFCategoryFunction")]
        public void Run([CosmosDBTrigger(
            databaseName: "FurnitureStoreDb",
            containerName: "categories",
            Connection = "CosmosDbConnectionString",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<MyDocument> input)
        {
            if (input != null && input.Count > 0)
            {
                _logger.LogInformation("Documents modified: " + input.Count);
                _logger.LogInformation("First document Id: " + input[0].id);
            }
        }
    }

    public class MyDocument
    {
        public string id { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public bool Boolean { get; set; }
    }
}
