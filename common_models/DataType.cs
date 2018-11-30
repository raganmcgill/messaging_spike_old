namespace common_models
{
    public class DataType
    {
        public string Type { get; set; }
        public int? MaximumLength { get; set; }
        public int? Precision { get; set; }
        public int? PrecisionRadix { get; set; }
        public int? NumericScale { get; set; }
        public int? DateTimePrecision { get; set; }
    }
}