﻿using Newtonsoft.Json;

namespace FurnitureStore.Server.Models.Documents
{
    public class PurchaseHistory
    {
        [JsonProperty("orderID")]
        public string OrderID { get; set; }

        [JsonProperty("createAt")]
        public string CreateAt { get; set; }

        [JsonProperty("totalAmount")]
        public string TotalAmount { get; set; }
    }

    internal class Customer
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("customerID")]
        public string CustomerID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; } 

        [JsonProperty("totalPay")]
        public int TotalPay { get; set; }

        [JsonProperty("purchaseHistory")]
        public List<PurchaseHistory> PurchaseHistory { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("passwordTemp")]
        public string PasswordTemp { get; set; }

        [JsonProperty("hashedAndSaltedPassword")]
        public string HashedAndSaltedPassword { get; set; }
    }
}