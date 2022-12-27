using Newtonsoft.Json;

namespace WPFPrismTemplate.Base.Models
{
    public class NvMapObjectEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        public string label { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string short_label { get; set; }

        public string start { get; set; }

        public int length { get; set; } = 2;
        public string encoding { get; set; } = "int";

        public bool packed { get; set; } = true;

        public int? min { get; set; }

        public int? max { get; set; }

        public int? @default { get; set; }

        public int? multiple_of { get; set; }

        public string suffix { get; set; }

        public string mask { get; set; }

        /// <summary>
        /// CSV - Convert to array
        /// </summary>
        public string values { get; set; }

        public int? scale { get; set; }

        public int? offset { get; set; }

        public string units { get; set; }

        public string special_values { get; set; }
    }
}
