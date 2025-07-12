using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokéToolsThèque.Pokemons
{
    public class Pokemon
    {
        public int Dex { get; set; }
        public string? Name { get; set; }
        public string? Form { get; set; }
        public string? Type1 { get; set; }
        public string? Type2 { get; set; }
        public int Total { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpAtk { get; set; }
        public int SpDef { get; set; }
        public int Speed { get; set; }
        public int Gen { get; set; }

        public string? Nom { get; set; }
        public int? Ether { get; set; }
        public int? Psycox { get; set; }

        public string? IconUrl { get; set; }
        public string? PokeApiIdentifier { get; set; }

        public List<Move> Moves { get; set; }
        public List<Ability> Abilities { get; set; }
        public bool IsFullyEvolved { get; set; }
        public string Category { get; set; }
        public string SmogonTier { get; set; }
        public Tiers? Tiers { get; set; }


        //Export
        public string? SelectedItem { get; set; }
        public string? SelectedAbility { get; set; }
        public string? SelectedNature { get; set; }
        public Dictionary<string, int> EVs { get; set; } = new();
        public List<string> SelectedMoves { get; set; } = new();
    }
}
