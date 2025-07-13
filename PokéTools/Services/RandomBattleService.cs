using PokéToolsThèque.RandomBattle;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PokéToolsThèque.Pokemons;
using PokéTools.Services;

namespace PokeTools.Services
{
    public class RandomBattleService
    {
        private readonly List<RandomPokemon> _randomPokemons;
        private readonly List<Pokemon> _pokemons;

        public RandomBattleService()
        {
            var json = DownloadJsonAsync().GetAwaiter().GetResult();
            _randomPokemons = ParseJsonToList(json);
            _pokemons = new PokemonService().Pokemons.ToList();
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
            return _randomPokemons.Find(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public List<RandomPokemon> GetAllPokemons() => _randomPokemons;

        public async Task AppendRandomSetColumnToTsv(string inputPath, string outputPath)
        {
            var json = await new HttpClient().GetStringAsync("https://pkmn.github.io/randbats/data/gen9randombattle.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json, options);
            var randomNames = new HashSet<string>(dict.Keys, StringComparer.OrdinalIgnoreCase);

            // 2. Lire le fichier TSV ligne par ligne
            var lines = File.ReadAllLines(inputPath);
            if (lines.Length == 0) throw new Exception("Fichier vide.");

            var header = lines[0] + "\tRandomSet";
            var outputLines = new List<string> { header };

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var fields = line.Split('\t');

                if (fields.Length < 18)
                {
                    outputLines.Add(line + "\tfalse");
                    continue;
                }

                var name = fields[1]; 
                var hasRandomSet = !string.IsNullOrWhiteSpace(name) &&
                                   randomNames.Contains(name.Trim());

                outputLines.Add(line + "\t" + hasRandomSet.ToString().ToLower());
            }

            File.WriteAllLines(outputPath, outputLines, Encoding.UTF8);

            Console.WriteLine($"✅ Fichier enrichi généré : {outputPath}");
        }
    }

}


