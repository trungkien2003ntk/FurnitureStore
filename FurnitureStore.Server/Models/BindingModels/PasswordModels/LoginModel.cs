namespace FurnitureStore.Server.Models.BindingModels.PasswordModels
{
    public class LoginModel : BasePasswordModel
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
