namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class CategoryManagement
    {
        private bool isHiddenPopup { get; set; } = true;
        public void AddCategoryLV1()
        {
            isHiddenPopup = false;
        }
        public void AddCategoryLV2()
        {
            isHiddenPopup = false;
        }
        public void AddCategoryLV3()
        {
            isHiddenPopup = false;
        }
        public void CloseAddUpdateCategoryPopup()
        {
            isHiddenPopup = true;
        }
        public void CancelAddUpdateCategoryPopup()
        {
            isHiddenPopup = true;
        }
    }
}
