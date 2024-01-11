namespace FurnitureStore.Server.Exceptions
{
    public class InvalidSortByPropertyException(string propertyName) : Exception($"The property '{propertyName}' does not exist on the product object.") { }
}
