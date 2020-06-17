using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace processpubsubapi
{
    public partial class Greater100
    {
        [JsonProperty("Items")]
        public long Items { get; set; }

        [JsonProperty("TotalCost")]
        public double TotalCost { get; set; }

        [JsonProperty("SalesNumber")]
        public string SalesNumber { get; set; }

        [JsonProperty("SalesDate")]
        public string SalesDate { get; set; }

        [JsonProperty("Store")]
        public string Store { get; set; }

        [JsonProperty("ReceiptImage")]
        public string ReceiptImage { get; set; }
    }

    public partial class Less100
    {
        [JsonProperty("Items")]
        public long Items { get; set; }

        [JsonProperty("TotalCost")]
        public double TotalCost { get; set; }

        [JsonProperty("SalesNumber")]
        public string SalesNumber { get; set; }

        [JsonProperty("SalesDate")]
        public string SalesDate { get; set; }

        [JsonProperty("Store")]
        public string Store { get; set; }
    }
}