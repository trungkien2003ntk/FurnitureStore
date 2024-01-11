namespace FurnitureStore.Server.Models.BindingModels;

public class FileModel
{
    public IFormFile ImageFiles { get; set; }

    internal string Url { get; set; }
    //public string? Base64 { get; set; }
}
