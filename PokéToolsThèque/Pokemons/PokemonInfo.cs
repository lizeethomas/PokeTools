using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokéToolsThèque.Pokemons
{
    public class PokemonInfo
    {
        public string Name { get; set; }
        public List<string> Abilities { get; set; }
        public string HiddenAbility { get; set; }
        public int EvolutionStage { get; set; }
    }
}
