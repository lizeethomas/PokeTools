using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokéToolsThèque.Pokemons;

namespace PokéToolsThèque.Misc
{
    public class OffensiveCoverage
    {
        public List<Pokemon> Ineffective { get; set; } = new();
        public List<Pokemon> NotVeryEffective { get; set; } = new();
        public List<Pokemon> Neutral { get; set; } = new();
        public List<Pokemon> SuperEffective { get; set; } = new();
    }
}
