namespace FurnitureStore.Server.Utils;

public class IdUtils
{
    public static string IncreaseId(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            string prefix = id[..4];
            string numericPart = id[4..];

            if (int.TryParse(numericPart, out int numericValue))
            {
                numericValue++;

                string newId = $"{prefix}{numericValue:D5}";
                return newId;
            }
        }

        return id;
    }
}
