using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokéToolsThèque.Pokemons
{
    public class Move
    {
        public string Name { get; set; }
        public int PP { get; set; }
        public int Power { get; set; }
        public int? Accuracy { get; set; }
        public string DamageClass { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string PokeApiIdentifier { get; set; }
    }
}
