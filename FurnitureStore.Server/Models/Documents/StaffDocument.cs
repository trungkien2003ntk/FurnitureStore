﻿namespace FurnitureStore.Server.Models.Documents;

public class StaffDocument
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("staffId")]
    public string StaffId { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }
}
