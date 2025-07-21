using PokéToolsThèque.Pokemons;
using System.Text;

namespace PokéToolsWeb.Services
{
    public static class ToolService
    {

        private static TypeService typeService = new TypeService();

        public static Tiers GetTiers(Pokemon pokemon)
        {
            Tiers tiers = new Tiers()
            {
                Bulk = GetBulkTier(GetBulk(pokemon)),
                PhysicalBulk = GetBulkTier(GetPhysicalBulk(pokemon)),
                SpecialBulk = GetBulkTier(GetSpecialBulk(pokemon)),
                PhysicalPower = GetPowerTier(GetPhysicalPower(pokemon)),
                SpecialPower = GetPowerTier(GetSpecialPower(pokemon)),
                SpeedTier = GetSpeedTier(pokemon.Speed)
            };
            return tiers;
        }


        public static double GetBulk(Pokemon pkmn)
        {
            var defensiveProfile = typeService.GetDefensiveProfile(pkmn.Type1, pkmn.Type2);
            double avgMultiplier = defensiveProfile.Values.Average();
            double harmonicMean = 2.0 * pkmn.Defense * pkmn.SpDef / (pkmn.Defense + pkmn.SpDef);
            return pkmn.HP * (1.0 / avgMultiplier) * harmonicMean;
        }

        public static double GetPhysicalBulk(Pokemon pkmn)
        {
            var defensiveProfile = typeService.GetDefensiveProfile(pkmn.Type1, pkmn.Type2);
            double avgMultiplier = defensiveProfile.Values.Average();
            return pkmn.HP * (1.0 / avgMultiplier) * pkmn.Defense;
        }

        public static double GetSpecialBulk(Pokemon pkmn)
        {
            var defensiveProfile = typeService.GetDefensiveProfile(pkmn.Type1, pkmn.Type2);
            double avgMultiplier = defensiveProfile.Values.Average();
            return pkmn.HP * (1.0 / avgMultiplier) * pkmn.SpDef;
        }

        public static double GetPower(Pokemon pkmn)
        {
            var offensiveCoverage = typeService.GetOffensiveCoverage(pkmn.Type1, pkmn.Type2);
            double avgMultiplier = offensiveCoverage.Values.Average();
            double attPower = pkmn.Attack / 120.0;
            double speAttPower = pkmn.SpAtk / 100.0;
            return avgMultiplier * Math.Max(attPower, speAttPower);
        }

        public static double GetPhysicalPower(Pokemon pkmn)
        {
            var offensiveCoverage = typeService.GetOffensiveCoverage(pkmn.Type1, pkmn.Type2);
            double avgMultiplier = offensiveCoverage.Values.Average();
            double attPower = pkmn.Attack / 120.0;
            return avgMultiplier * attPower;
        }

        public static double GetSpecialPower(Pokemon pkmn)
        {
            var offensiveCoverage = typeService.GetOffensiveCoverage(pkmn.Type1, pkmn.Type2);
            double avgMultiplier = offensiveCoverage.Values.Average();
            double attPower = pkmn.SpAtk / 100.0;
            return avgMultiplier * attPower;
        }

        private static char GetBulkTier(double bulk)
        {
            return bulk switch
            {
                >= 11000 => 'S',
                >= 9000 => 'A',
                >= 7000 => 'B',
                >= 5000 => 'C',
                >= 3000 => 'D',
                >= 1000 => 'E',
                _ => 'F'
            };
        }

        private static char GetPowerTier(double power)
        {
            return power switch
            {
                >= 1.7 => 'S',
                >= 1.4 => 'A',
                >= 1.1 => 'B',
                >= 0.8 => 'C',
                >= 0.5 => 'D',
                >= 0.2 => 'E',
                _ => 'F'
            };
        }

        private static char GetSpeedTier(double speed)
        {
            return speed switch
            {
                >= 120 => 'S',
                >= 110 => 'A',
                >= 100 => 'B',
                >= 80 => 'C',
                >= 60 => 'D',
                >= 40 => 'E',
                _ => 'F'
            };
        }

        public static string BuildIdentifier(string name, string form)
        {
            string Normalize(string s) =>
                s.ToLowerInvariant()
                 .Replace("♀", "-f")
                 .Replace("♂", "-m")
                 .Replace(" ", "-")
                 .Replace("’", "")
                 .Replace("'", "");

            var baseName = Normalize(name);

            if (string.IsNullOrWhiteSpace(form))
                return baseName;

            var formSlug = Normalize(form);

            var knownForms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "alolan", "alola" },
            { "galarian", "galar" },
            { "hisuian", "hisui" },
            { "paldean", "paldea" },
            { "mega", "mega" },
            { "gigantamax", "gmax" },
            { "attack", "attack" },
            { "defense", "defense" },
            { "speed", "speed" },
            { "origin", "origin" },
            { "sky", "sky" },
            { "crowned sword", "crowned-sword" },
            { "crowned shield", "crowned-shield" },
            { "blue-striped", "blue-striped" },
            { "white-striped", "white-striped" },
            { "school", "school" },
            { "solo", "solo" },
            { "dusk", "dusk" },
            { "dawn", "dawn" },
            { "midnight", "midnight" },
            { "midday", "midday" },
            { "rapid-strike", "rapid-strike" },
            { "single-strike", "single-strike" },
            { "ice-rider", "ice-rider" },
            { "shadow-rider", "shadow-rider" },
            { "resolute", "resolute" },
            { "pirouette", "pirouette" }
        };

            foreach (var kvp in knownForms)
            {
                if (formSlug.Contains(kvp.Key.ToLowerInvariant()))
                    return $"{baseName}-{kvp.Value}";
            }

            return $"{baseName}-{formSlug}";
        }
    }
}
