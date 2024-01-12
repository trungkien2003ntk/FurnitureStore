using System.Text;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class Login
    {
        protected override async Task OnInitializedAsync()
        {
            string hashPassword = EncryptData("123456789abc");
            await base.OnInitializedAsync();
        }
        public static string EncryptData(string data)
        {
            try
            {
                byte[] encDataByte = Encoding.UTF8.GetBytes(data!);
                string encodeData = Convert.ToBase64String(encDataByte);
                return encodeData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode " + ex.Message);
            }
        }
    }

}
