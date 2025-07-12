using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PokéToolsThèque.RandomBattle
{
    public class SubSet
    {
        [JsonPropertyName("abilities")]
        public List<string>? Abilities { get; set; }

        [JsonPropertyName("items")]
        public List<string>? Items { get; set; }

        [JsonPropertyName("teraTypes")]
        public List<string>? TeraTypes { get; set; }

        [JsonPropertyName("moves")]
        public List<string>? Moves { get; set; }

        [JsonPropertyName("evs")]
        public Dictionary<string, int>? EVs { get; set; }

        [JsonPropertyName("ivs")]
        public Dictionary<string, int>? IVs { get; set; }
    }
}
