using PokéToolsThèque;
using PokéTools.Tools;
using System.Text;

namespace PokéTools.Services
{
    public class AbilityService
    {
        private readonly List<Ability> _abilities;
        public List<Ability> Abilities => _abilities;

        private static readonly string _filePath = "E:\\Bureau\\CODE\\PokéToolsProject\\PokéTools\\Data\\abilities.tsv";

        public AbilityService()
        {
            _abilities = LoadAbilitiesFromFile(_filePath);
        }

        private static List<Ability> LoadAbilitiesFromFile(string filePath)
        {
            var abilities = new List<Ability>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var fields = line.Split('\t');

                if (fields.Length < 2)
                    continue;

                abilities.Add(new Ability
                {
                    Name = FormatAbilityName(fields[0]),
                    Description = fields[1],
                    PokeApiIdentifier = fields[0],
                    IsHidden = false
                });
            }

            return abilities;
        }

        private static string FormatAbilityName(string ability)
        {
            if (string.IsNullOrWhiteSpace(ability)) return "";
            return string.Join(" ", ability.Split('-')
                .Select(word => char.ToUpper(word[0]) + word[1..]));
        }
    }
}
