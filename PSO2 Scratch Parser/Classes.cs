using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PSO2_Scratch_Parser
{
    public class Prize
    {
        public string number { get; set; }

        [JsonProperty("new")]
        public string new_status { get; set; }

        public string name { get; set; }

        public string type_name { get; set; }

        public string percent { get; set; }

        public string explanation { get; set; }

        public string other { get; set; }

        public string supplement { get; set; }

        public string voiceid { get; set; }

        public string type { get; set; }

        public string illust { get; set; }

        public string style { get; set; }

        public string copyright { get; set; }

        public string pickup { get; set; }

        public string ss { get; set; }

        public string scratchname { get; set; }

        public string name_en { get; set; }

        public string type_name_en { get; set; }

        public string explanation_en { get; set; }

        public string supplement_en { get; set; }
    }

    public class BonusPrize
    {
        public string number { get; set; }

        [JsonProperty("new")]
        public string new_status { get; set; }

        public string name { get; set; }

        public string type_name { get; set; }

        public string num02 { get; set; }

        public string num03 { get; set; }

        public string trade { get; set; }

        public string explanation { get; set; }

        public string other { get; set; }

        public string supplement { get; set; }

        public string voiceid { get; set; }

        public string type { get; set; }

        public string illust { get; set; }

        public string style { get; set; }

        public string copyright { get; set; }

        public string pickup { get; set; }

        public string ss { get; set; }

        public string scratchname { get; set; }

        public string cycle { get; set; }

        public string name_en { get; set; }

        public string type_name_en { get; set; }

        public string explanation_en { get; set; }

        public string supplement_en { get; set; }
    }
}
