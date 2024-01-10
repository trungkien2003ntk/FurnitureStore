﻿namespace FurnitureStore.Shared.DTOs;

public class StaffDTO
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("staffId")]
    public string? StaffId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("contact")]
    public Contact Contact { get; set; } = new();

    [JsonProperty("profileImage")]
    public string? ProfileImage { get; set; }

    [JsonProperty("role")]
    public string? Role { get; set; } = "admin";

    [JsonProperty("address")]
    public string? Address { get; set; }

    [JsonProperty("note")]
    public string? Note { get; set; }

    [JsonProperty("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    [JsonProperty("hashedAndSaltedPassword")]
    public string? HashedAndSaltedPassword { get; set; }

    [JsonProperty("defaultPassword")]
    public string? DefaultPassword { get; set; }

    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    [JsonProperty("isRemovable")]
    public bool? IsRemovable { get; set; }

    [JsonProperty("isDeleted")]
    public bool? IsDeleted { get; set; }

    [JsonProperty("ttl")]
    public int? TTL { get; set; }
}
