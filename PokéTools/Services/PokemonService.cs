
using System.Text;
using PokéToolsThèque.Pokemons;

namespace PokéTools.Services
{
    public class PokemonService
    {
        private readonly List<Pokemon> _pokemons;
        public List<Pokemon> Pokemons => _pokemons;

        private readonly string _filePath = "E:\\Bureau\\CODE\\PokéToolsProject\\PokéTools\\Data\\pokemons.tsv";
        private readonly AbilityService _abilityService;

        public PokemonService()
        {
            _abilityService = new AbilityService();
            _pokemons = LoadPokemonsFromFile(_filePath);
            _pokemons.ForEach(p => {
                p.Tiers = ToolService.GetTiers(p);
            });
        }

        public Pokemon GetRandomPokemon()
        {
            Random random = new Random();
            int dex = random.Next(1, _pokemons.Count);
            return _pokemons.FirstOrDefault(p => p.Dex == dex) ?? new Pokemon();
        }

        public List<Pokemon> GetRandomTeamWithBudget(int statBudget)
        {
            var rnd = new Random();

            int maxRetries = 50;
            while (maxRetries-- > 0)
            {
                var candidates = _pokemons
                    .Where(p => p.IsFullyEvolved)
                    .OrderBy(_ => rnd.Next()) 
                    .ToList();

                var team = new List<Pokemon>();
                int currentTotal = 0;

                foreach (var p in candidates)
                {
                    if (team.Count >= 6)
                        break;

                    if (!team.Contains(p) && currentTotal + p.Total <= statBudget)
                    {
                        team.Add(p);
                        currentTotal += p.Total;
                    }
                }

                if (team.Count == 6)
                    return team;
            }

            return new List<Pokemon>();
        }


        private List<Pokemon> LoadPokemonsFromFile(string filePath)
        {
            var pokemons = new List<Pokemon>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            var allAbilities = _abilityService.Abilities;

            if (lines.Length <= 1)
                return pokemons;

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var fields = line.Split('\t');

                List<Ability> abilities = fields[18]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => allAbilities.FirstOrDefault(a => a.PokeApiIdentifier.Equals(id, StringComparison.OrdinalIgnoreCase)))
                    .Where(a => a != null)
                    .ToList()!;

                var hiddenAbility = !string.IsNullOrWhiteSpace(fields[19])
                    ? allAbilities.FirstOrDefault(a => a.PokeApiIdentifier.Equals(fields[19], StringComparison.OrdinalIgnoreCase))
                    : null;

                if (hiddenAbility != null)
                {
                    abilities.Add(new Ability
                    {
                        Name = hiddenAbility.Name,
                        Description = hiddenAbility.Description,
                        PokeApiIdentifier = hiddenAbility.PokeApiIdentifier,
                        IsHidden = true
                    });
                }

                pokemons.Add(new Pokemon
                {
                    Dex = ParseInt(fields[0]),
                    Name = fields[1],
                    Nom = fields[2],
                    Form = fields[3],
                    Type1 = fields[4],
                    Type2 = string.IsNullOrWhiteSpace(fields[5]) ? null : fields[5],
                    Total = ParseInt(fields[6]),
                    HP = ParseInt(fields[7]),
                    Attack = ParseInt(fields[8]),
                    Defense = ParseInt(fields[9]),
                    SpAtk = ParseInt(fields[10]),
                    SpDef = ParseInt(fields[11]),
                    Speed = ParseInt(fields[12]),
                    Gen = ParseInt(fields[13]),
                    Ether = ParseNullableInt(fields[14]),
                    Psycox = ParseNullableInt(fields[15]),
                    IconUrl = fields[16],
                    PokeApiIdentifier = fields[17],
                    IsFullyEvolved = Convert.ToBoolean(fields[20]),
                    Category = fields[21],
                    SmogonTier = fields[22],
                    RandomSet = Convert.ToBoolean(fields[23]),
                    Abilities = abilities
                });
            }

            return pokemons;
        }

        private static int ParseInt(string s)
        {
            return int.TryParse(s, out int result) ? result : 0;
        }

        private static int? ParseNullableInt(string s)
        {
            return int.TryParse(s, out int result) ? result : (int?)null;
        }
    }
}
