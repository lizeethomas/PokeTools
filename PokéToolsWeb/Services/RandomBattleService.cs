using PokéToolsThèque.Pokemons;
using PokéToolsThèque.RandomBattle;
using System.Text.Json;

namespace PokéToolsWeb.Services
{
    public class RandomBattleService
    {
        public List<RandomPokemon> _randomPokemons { get; set; } = new List<RandomPokemon>();

        private async Task<string> DownloadJsonAsync()
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync("https://pkmn.github.io/randbats/data/gen9randombattle.json");
        }

        public async Task<List<RandomPokemon>> LoadSets()
        {
            string json = await DownloadJsonAsync();
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

        /*
        public RandomPokemon? GetPokemon(string name)
        {
            return _randomPokemons.Find(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public List<RandomPokemon> GetAllPokemons() => _randomPokemons;
        */
    }
}