using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PokéToolsThèque.RandomBattle;

namespace PokeTools.Services
{
    public class RandomBattleService
    {
        private readonly List<RandomPokemon> _pokemons;

        public RandomBattleService()
        {
            var json = DownloadJsonAsync().GetAwaiter().GetResult();
            _pokemons = ParseJsonToList(json);
        }

        private async Task<string> DownloadJsonAsync()
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync("https://pkmn.github.io/randbats/data/gen9randombattle.json");
        }

        private List<RandomPokemon> ParseJsonToList(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var dict = JsonSerializer.Deserialize<Dictionary<string, Set>>(json, options)
                       ?? new Dictionary<string, Set>();

            var list = new List<RandomPokemon>();
            foreach (var kvp in dict)
            {
                list.Add(new RandomPokemon
                {
                    Name = kvp.Key,
                    Set = kvp.Value
                });
            }

            return list;
        }

        public RandomPokemon? GetPokemon(string name)
        {
            return _pokemons.Find(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public List<RandomPokemon> GetAllPokemons() => _pokemons;
    }

}


