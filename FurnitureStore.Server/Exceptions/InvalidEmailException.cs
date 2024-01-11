namespace FurnitureStore.Server.Exceptions
{
    public class InvalidEmailException(string? message = "Invalid Email") : Exception(message) { }
}
