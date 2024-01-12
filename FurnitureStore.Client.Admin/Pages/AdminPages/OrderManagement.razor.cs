namespace FurnitureStore.Client.Admin.Pages.AdminPages
{
    public partial class OrderManagement
    {
        private bool isHiddenPopup { get; set; } = true;

        public void ShowPopup()
        {
            isHiddenPopup = false;
        }

        public void CloseOrderDetailPopup()
        {
            isHiddenPopup = true;
        }

        public void CancelOrderDetailPopup()
        {
            isHiddenPopup = true;
        }
    }
}
