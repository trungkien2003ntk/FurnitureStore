using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using System.Text;

namespace FurnitureStore.Server.Utils;

public class CosmosDbUtils
{
    public static async Task<IEnumerable<TDocument>> GetDocumentsByQueryDefinition<TDocument>(Container container, QueryDefinition queryDefinition)
    {
        var results = new List<TDocument>();

        using var feed = container.GetItemQueryIterator<TDocument>(queryDefinition);

        double requestCharge = 0d;

        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync();
            requestCharge += response.RequestCharge;

            results.AddRange(response);
        }

        //LogRequestCharged(requestCharge);

        return results;
    }

    public static async Task<TDocument?> GetDocumentByQueryDefinition<TDocument>(Container container, QueryDefinition queryDefinition)
    {
        using var feed = container.GetItemQueryIterator<TDocument>(
            queryDefinition: queryDefinition
        );

        FeedResponse<TDocument> response;

        if (feed.HasMoreResults)
        {
            response = await feed.ReadNextAsync();
            return response.FirstOrDefault();
        }

        //LogRequestCharged(response.RequestCharge);

        return default;
    }

    public static void CheckForNullToThrowException<TDocument>(TDocument document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));
    }



    public static QueryDefinition BuildQuery(QueryParameters queryParams, string defaultSelect = "SELECT *", bool isRemovableDocument = true)
    {
        var query = new StringBuilder($"{defaultSelect} FROM c WHERE ISDEFINED(c.id) ");

        AppendDeleteFilter(query, isRemovableDocument);
        AppendQueryParameters(query, queryParams);

        return BuildQueryDef(query);
    }

    public static QueryDefinition BuildQuery(QueryParameters queryParams, ProductFilterModel filter, string defaultSelect = "SELECT *", bool isRemovableDocument = true)
    {
        var query = new StringBuilder($"{defaultSelect} FROM c WHERE ISDEFINED(c.id) ");

        //AppendProductFilter(query, filter);
        AppendDeleteFilter(query, isRemovableDocument);

        QueryParameters queryParamsToGetAll = new()
        {
            PageNumber = 1,
            PageSize = -1,
            OrderBy = queryParams.OrderBy,
            SortBy = queryParams.SortBy
        };

        AppendQueryParameters(query, queryParamsToGetAll);

        QueryDefinition queryDef = BuildQueryDef(query);

        return queryDef;
    }

    public static QueryDefinition BuildQuery(QueryParameters queryParams, CategoryFilterModel filter, string defaultSelect = "SELECT *", bool isRemovableDocument = true)
    {
        var query = new StringBuilder($"{defaultSelect} FROM c WHERE ISDEFINED(c.id) ");

        AppendCategoryFilter(query, filter);
        AppendDeleteFilter(query, isRemovableDocument);
        AppendQueryParameters(query, queryParams);

        QueryDefinition queryDef = BuildQueryDef(query);

        return queryDef;
    }


    private static void AppendProductFilter(StringBuilder query, ProductFilterModel filter)
    {
        if (!VariableHelpers.IsNull(filter.CategoryIds))
        {
            var categoryIds = string.Join(", ", filter.CategoryIds!.Select(id => $"\"{id}\""));
            query.Append($" and c.categoryId IN ({categoryIds})");
        }

        if (!VariableHelpers.IsNull(filter.VariationId))
        {
            query.Append($" AND (NOT IS_NULL(c.variationDetail.id) AND STRINGEQUALS(c.variationDetail.id, '{filter.VariationId}'))");
        }
        else
        {
            // if variationDetails.id is null, we'll include it in the result, otherwise, just include the one with min SalePrice
            query.Append(" AND (c.variationDetails.id = null OR c.salePrice = (SELECT VALUE MIN(p.salePrice) FROM p WHERE p.variationDetails.id = c.variationDetails.id))");
        }

        //AppendIsActiveFilter(query, filter.IsActive);
    }

    private static void AppendCategoryFilter(StringBuilder query, CategoryFilterModel filter)
    {
        if (filter.ParentId != null)
        {
            query.Append($" AND STRINGEQUALS(c.parentId, '{filter.ParentId}')");
        }

        if (filter.Level != null)
        {
            query.Append($" AND c.level = {filter.Level}");
        }
    }


    private static void AppendDeleteFilter(StringBuilder query, bool isRemovableDocument)
    {
        if (isRemovableDocument)
        {
            query.Append(" AND c.isDeleted = false");
        }
    }

    private static void AppendQueryParameters(StringBuilder query, QueryParameters queryParameters)
    {
        if (!string.IsNullOrEmpty(queryParameters.SortBy) && !string.IsNullOrEmpty(queryParameters.OrderBy))
        {
            query.Append($" ORDER BY c.{queryParameters.SortBy} {queryParameters.OrderBy}");
        }

        if (queryParameters.PageSize != -1)
        {
            query.Append($" OFFSET {(queryParameters.PageNumber - 1) * queryParameters.PageSize} LIMIT {queryParameters.PageSize}");
        }
    }

    private static QueryDefinition BuildQueryDef(StringBuilder query)
    {
        // avoid sql injection
        query = query.Replace(";", "");
        return new QueryDefinition(query.ToString());
    }


    private static void AppendIsActiveFilter(StringBuilder query, bool? isActive)
    {
        if (!VariableHelpers.IsNull(isActive))
        {
            query.Append($" and c.isActive = {isActive.ToString()!.ToLower()}");
        }
    }
}
