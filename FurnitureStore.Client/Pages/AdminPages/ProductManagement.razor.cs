using Microsoft.AspNetCore.Components.Web;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class ProductManagement
    {
        private bool isHidden { get; set; } = true;

        private void ShowAddUpdateProductPopup(MouseEventArgs e)
        {
            isHidden = false;
        }

        private void CloseAddUpdatePopup(MouseEventArgs e)
        {
            isHidden = true;
        }
        private void CancelAddUpdatePopup(MouseEventArgs e)
        {
            isHidden = true;
        }
    }
}
