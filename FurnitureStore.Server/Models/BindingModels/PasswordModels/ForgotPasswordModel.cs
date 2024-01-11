namespace FurnitureStore.Server.Models.BindingModels.PasswordModels
{
    public class ForgotPasswordModel : BasePasswordModel
    {
        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}
