namespace FurnitureStore.Client
{
    public class GlobalConfig
    {
        public const string BASE_ENDPOINT = "https://furniturestore-uit-y3s3.azurewebsites.net";
        public const string BASE_URL = $"{BASE_ENDPOINT}/api/";
        public const string PRODUCT_BASE_URL = $"{BASE_URL}Products";
        public const string CATEGORY_BASE_URL = $"{BASE_URL}Categories";
        public const string ORDER_BASE_URL = $"{BASE_URL}Orders";
        public const string STAFF_BASE_URL = $"{BASE_URL}Staffs";
    }
}
