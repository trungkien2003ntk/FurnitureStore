using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.BindingModels;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Text;

namespace FurnitureStore.Server.Utils
{
    public class AzureSearchUtils
    {
        public static SearchOptions BuildOptions(QueryParameters queryParameters, CategoryFilterModel filter)
        {
            var options = BuildBaseOptions(queryParameters);

            var filterQuery = BuildFilter(filter);
            options.Filter = filterQuery;

            return options;
        }

        public static SearchOptions BuildOptions(QueryParameters queryParameters, ProductFilterModel filter)
        {
            var options = BuildBaseOptions(queryParameters);

            var filterQuery = BuildFilter(filter);
            options.Filter = filterQuery;

            return options;
        }

        private static SearchOptions BuildBaseOptions(QueryParameters queryParameters)
        {
            var options = new SearchOptions
            {
                QueryType = SearchQueryType.Semantic,
                SemanticSearch = new SemanticSearchOptions()
                {
                    QueryCaption = new QueryCaption(QueryCaptionType.Extractive),
                    QueryAnswer = new QueryAnswer(QueryAnswerType.Extractive)
                    {
                        Count = 3
                    },
                    SemanticConfigurationName = "productByName"
                },
                IncludeTotalCount = true
            };

            //if (queryParameters.PageSize != -1)
            //{
            //    options.Size = queryParameters.PageSize;
            //    options.Skip = (queryParameters.PageNumber - 1) * queryParameters.PageSize;
            //}

            //options.OrderBy.Add($"{queryParameters.SortBy} {queryParameters.OrderBy}");

            return options;
        }


        private static string BuildFilter(CategoryFilterModel filter)
        {
            var query = new StringBuilder("ttl eq -1");

            query.Append(" and isDeleted eq false ");

            AppendFilter(query, filter);

            return query.ToString();
        }

        private static string BuildFilter(ProductFilterModel filter)
        {
            var query = new StringBuilder("ttl eq -1");

            query.Append(" and isDeleted eq false ");

            AppendFilter(query, filter);


            return query.ToString();
        }

        private static void AppendFilter(StringBuilder query, CategoryFilterModel filter)
        {

        }

        private static void AppendFilter(StringBuilder query, ProductFilterModel filter)

        {
            //if (!VariableHelpers.IsNull(filter.CategoryIds))
            //{
            //    var categoryIds = string.Join(", ", filter.CategoryIds!.Select(id => $"{id}"));
            //    query.Append($" and search.in(categoryId, '{categoryIds}')");
            //}


            //if (!VariableHelpers.IsNull(filter.PriceRangeStrings))
            //{
            //    var priceRangeConditions = filter.PriceRanges!
            //        .Select(range => $"(salePrice ge {range.MinPrice} and salePrice le {range.MaxPrice})");

            //    var priceRangeQuery = string.Join(" or ", priceRangeConditions);
            //    query.Append($" and ({priceRangeQuery})");
            //}

            AppendIsActiveFilter(query, filter.IsActive);
        }



        private static void AppendIsActiveFilter(StringBuilder query, bool? isActive)
        {
            if (!VariableHelpers.IsNull(isActive))
            {
                query.Append($" and isActive eq {isActive.ToString()!.ToLower()}");
            }
        }
    }
}
