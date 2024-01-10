namespace FurnitureStore.Client
{
    public class GlobalConfig
    {
        public const string BASE_ENDPOINT = "localhost:7007";
        public const string BASE_URL = $"https://{BASE_ENDPOINT}/api/";
        public const string PRODUCT_BASE_URL = $"{BASE_URL}Products/";
        public const string CATEGORY_BASE_URL = $"{BASE_URL}Categories/";
    }
}
