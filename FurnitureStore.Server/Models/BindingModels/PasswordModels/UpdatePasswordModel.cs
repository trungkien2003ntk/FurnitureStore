namespace FurnitureStore.Server.Models.BindingModels.PasswordModels
{
    public class UpdatePasswordModel : ForgotPasswordModel
    {
        [JsonProperty("oldPassword")]
        public string OldPassword { get; set; }
    }
}
