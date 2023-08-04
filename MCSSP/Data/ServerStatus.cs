using System.Text.Json.Serialization;

namespace MCSSP.Data;

public struct ServerStatus {
    [JsonPropertyName("version")]
    public Version Ver;
    [JsonPropertyName("players")]
    public Players PlayerList;

    public struct Version {
        [JsonPropertyName("name")]
        public string Name {get; set;} = "1.20";
        [JsonPropertyName("protocol")]
        public int Protocol {get;set;} = 763;
        public Version() {}
    }

    public struct Players {
        [JsonPropertyName("max")]
        public int Max;
        [JsonPropertyName("online")]
        public int Online;
        [JsonPropertyName("sample")]
        public Sample[]? Samples;

        public struct Sample {
            [JsonPropertyName("name")]
            public string Name;
            [JsonPropertyName("id")]
            public string Id;
        }
    }
}