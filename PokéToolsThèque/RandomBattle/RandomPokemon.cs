using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokéToolsThèque.RandomBattle
{
    public class RandomPokemon
    {
        public string Name { get; set; } = string.Empty;
        public Set Set { get; set; } = new();
    }
}
