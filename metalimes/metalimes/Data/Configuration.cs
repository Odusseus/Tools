namespace metalimes.Data
{
    public class Configuration
    {
        public int Id { get; set; }
        public ConfigKey Key { get; set; }

        // optional typed values; only one is expected to be used depending on the key
        public string? StringValue { get; set; }
        public int? IntegerValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        // Value type indicator: "String", "Integer", or "DateTime"
        public string ValueType { get; set; } = "String";

        public DateTime CreatedAt { get; set; }
    }
}
