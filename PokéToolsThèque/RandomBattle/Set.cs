using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PokéToolsThèque.RandomBattle
{
    public class Set
    {
        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("abilities")]
        public List<string>? Abilities { get; set; }

        [JsonPropertyName("items")]
        public List<string>? Items { get; set; }

        [JsonPropertyName("roles")]
        public Dictionary<string, SubSet>? Roles { get; set; }
    }
}
