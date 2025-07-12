using PokéTools.Services;
using PokéToolsThèque;

public class OffensiveAnalyzer
{
    private readonly TypeService _typeService;
    private readonly PokemonService _pokemonService;

    public OffensiveAnalyzer()
    {
        _typeService = new TypeService();
        _pokemonService = new PokemonService();
    }

    private double ApplyTalentOverride(Pokemon target, string attackingType, double effectiveness)
    {
        var talents = target.Abilities.Select(a => a.PokeApiIdentifier).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if ((attackingType == "Ground") && (talents.Contains("levitate") || talents.Contains("earth-eater")))
            return 0.0;

        if ((attackingType == "Water") && (talents.Contains("dry-skin") || talents.Contains("water-absorb") || talents.Contains("storm-drain")))
            return 0.0;

        if ((attackingType == "Grass") && talents.Contains("sap-sipper"))
            return 0.0;

        if ((attackingType == "Electric") && (talents.Contains("lightning-rod") || talents.Contains("volt-absorb") || talents.Contains("motor-drive")))
            return 0.0;

        if (talents.Contains("wonder-guard") && effectiveness <= 1.0)
            return 0.0;

        return effectiveness;
    }

    public OffensiveCoverage EvaluateOffensiveCoverage(List<string> teamPokeApiIdentifiers)
    {
        var team = _pokemonService.Pokemons
            .Where(p => teamPokeApiIdentifiers.Contains(p.PokeApiIdentifier, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var result = new OffensiveCoverage();

        foreach (var target in _pokemonService.Pokemons)
        {
            double maxEffectiveness = 0.0;

            foreach (var attacker in team)
            {
                double effType1 = _typeService.GetEffectiveness(attacker.Type1, target.Type1, target.Type2);
                effType1 = ApplyTalentOverride(target, attacker.Type1, effType1);
                maxEffectiveness = Math.Max(maxEffectiveness, effType1);

                if (!string.IsNullOrEmpty(attacker.Type2))
                {
                    double effType2 = _typeService.GetEffectiveness(attacker.Type2, target.Type1, target.Type2);
                    effType2 = ApplyTalentOverride(target, attacker.Type2, effType2);
                    maxEffectiveness = Math.Max(maxEffectiveness, effType2);
                }
            }

            if (maxEffectiveness == 0.0)
                result.Ineffective.Add(target);
            else if (maxEffectiveness < 1.0)
                result.NotVeryEffective.Add(target);
            else if (maxEffectiveness == 1.0)
                result.Neutral.Add(target);
            else
                result.SuperEffective.Add(target);
        }

        return result;
    }

}
