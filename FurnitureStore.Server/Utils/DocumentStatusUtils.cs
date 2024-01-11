namespace FurnitureStore.Server.Utils
{
    public class DocumentStatusUtils
    {
        public static string GetInventoryStatus(int currentStock, int? minStock)
        {
            minStock ??= 0;

            if (currentStock >= minStock)
            {
                return "In Stock";
            }

            return "Out Of Stock";
        }

        public static string GetPaymentStatus(int targetAmount, int paidAmount)
        {
            if (targetAmount > paidAmount)
            {
                return "unpaid";
            }

            return "paid";
        }

        public static string GetPaymentStatus(int remainAmount)
        {
            if (remainAmount > 0)
            {
                return "unpaid";
            }

            return "paid";
        }
    }
}
