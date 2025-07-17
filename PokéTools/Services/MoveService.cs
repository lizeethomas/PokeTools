using PokéToolsThèque.Pokemons;
using System.Text;
using System.Text.Json;

namespace PokéTools.Services
{
    public class MoveService
    {
        private readonly List<Move> _moves;
        public List<Move> Moves => _moves;

        private  readonly string _filePath;

        public MoveService()
        {
            string basePath = AppContext.BaseDirectory;
            _filePath = Path.Combine(basePath, "Data", "pokemons.tsv");
            _moves = LoadMovesFromFile(_filePath);
        }

        private static List<Move> LoadMovesFromFile(string filePath)
        {
            var moves = new List<Move>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var fields = line.Split('\t');

                if (fields.Length < 6)
                    continue;

                if (!int.TryParse(fields[1], out int pp)) pp = 0;
                if (!int.TryParse(fields[2], out int power)) power = 0;

                if (!int.TryParse(fields[3], out int accuracy))
                    accuracy = -1;

                moves.Add(new Move
                {
                    Name = FormatMoveName(fields[0]),
                    PP = pp,
                    Power = power,
                    Accuracy = accuracy >= 0 ? accuracy : null,
                    DamageClass = fields[4],
                    Type = fields[5],
                    Description = fields[6], 
                    PokeApiIdentifier = fields[0]
                });
            }

            return moves;
        }

        private static string FormatMoveName(string move)
        {
            if (string.IsNullOrWhiteSpace(move)) return "";
            return string.Join(" ", move.Split('-')
                .Select(word => char.ToUpper(word[0]) + word[1..]));
        }

        public async Task<List<Move>> GetLearnableMovesAsync(string pokeApiIdentifier)
        {
            var learnableMoves = new List<Move>();

            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{pokeApiIdentifier.ToLower()}");
            if (!response.IsSuccessStatusCode) return learnableMoves;

            var json = await response.Content.ReadAsStringAsync();
            var pokemonData = JsonDocument.Parse(json);

            if (!pokemonData.RootElement.TryGetProperty("moves", out var movesElement))
                return learnableMoves;

            foreach (var moveEntry in movesElement.EnumerateArray())
            {
                var moveName = moveEntry.GetProperty("move").GetProperty("name").GetString();
                if (string.IsNullOrEmpty(moveName)) continue;

                var move = _moves.FirstOrDefault(m => m.PokeApiIdentifier.Equals(moveName, StringComparison.OrdinalIgnoreCase));
                if (move != null)
                {
                    learnableMoves.Add(move);
                }
            }

            return learnableMoves;
        }


    }
}
