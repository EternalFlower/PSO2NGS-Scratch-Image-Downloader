using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PSO2_Scratch_Parser
{
    public class PrizeBoxItem
    {
        [JsonProperty("name(jp)")]
        public string Name_jp { get; set; }

        [JsonProperty("name(en)")]
        public string Name_en { get; set; }
        
        [JsonProperty("image_url")]
        public string Image_url { get; set; }

        [JsonProperty("genre(jp)")]
        public string Genre_jp { get; set; }

        [JsonProperty("genre(en)")]
        public string Genre_en { get; set; }

    }

    public class Prize
    {
        [JsonProperty("name(jp)")]
        public string Name_jp { get; set; }

        [JsonProperty("name(en)")]
        public string Name_en { get; set; }

        [JsonProperty("concept_art")]
        public string Concept_art { get; set; }

        [JsonProperty("image_url")]
        public string Image_url { get; set; }

        [JsonProperty("genre(jp)")]
        public string Genre_jp { get; set; }

        [JsonProperty("genre(en)")]
        public string Genre_en { get; set; }

        [JsonProperty("rate")]
        public string Rate { get; set; }

        [JsonProperty("contents")]
        public List<PrizeBoxItem> Contents { get; set; }
    }
}
