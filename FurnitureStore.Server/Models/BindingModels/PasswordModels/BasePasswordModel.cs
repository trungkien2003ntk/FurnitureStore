namespace FurnitureStore.Server.Models.BindingModels.PasswordModels
{
    public class BasePasswordModel
    {
        [JsonProperty("email")]
        public string? Email { get; set; }
    }
}
