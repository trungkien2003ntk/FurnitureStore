using System.Diagnostics.CodeAnalysis;

namespace FurnitureStore.Shared.Additional;

public class Contact
{
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}
