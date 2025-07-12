using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokéToolsThèque.Pokemons
{
    public class Ability
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PokeApiIdentifier { get; set; }
        public bool IsHidden { get; set; }

    }
}
