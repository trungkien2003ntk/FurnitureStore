using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class ProductManagement
    {
        private bool isHidden { get; set; } = true;
        private bool isHiddenVariant { get; set; } = true;
        private bool isChecked { get; set; } = false;

        private void ShowAddUpdateProductPopup()
        {
            isHidden = false;
        }

        private void CloseAddUpdatePopup()
        {
            isHidden = true;
        }
        private void CancelAddUpdatePopup()
        {
            isHidden = true;
        }
        private void VariantToggleCheckbox(ChangeEventArgs e)
        {
            isChecked = (bool)e.Value;
            if (isChecked)
            {
                isHiddenVariant = false;
            }
            else
            {
                isHiddenVariant = true;
            }
        }
    }
}
