﻿namespace FurnitureStore.Shared;

public class ResponseToGetId
{
    [JsonProperty("id")]
    public string Id { get; set; }
}
