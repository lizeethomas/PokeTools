using Microsoft.AspNetCore.Mvc;
using PokéTools.Services;
using PokéToolsThèque;

namespace PokéTools.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeController : ControllerBase
    {
        private readonly TypeService _typeService;
        private readonly PokemonService _pokemonService;

        public TypeController(TypeService typeService, PokemonService pokemonService)
        {
            _typeService = typeService;
            _pokemonService = pokemonService;
        }

        [HttpGet("defensiveById/{id}")]
        public IActionResult GetDefensiveProfileByDex(string id)
        {
            var pokemon = _pokemonService.Pokemons.First(p => p.PokeApiIdentifier == id);
            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé pour le numéro #{pokemon.Dex}");
            var results = _typeService.GetDefensiveProfile(pokemon.Type1, pokemon.Type2);
            return Ok(results);
        }

        [HttpGet("defensiveByDex/{dex}")]
        public IActionResult GetDefensiveProfileByDex(int dex)
        {
            var pokemon = _pokemonService.Pokemons.First(p => p.Dex == dex);
            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé pour le numéro #{dex}");
            var results = _typeService.GetDefensiveProfile(pokemon.Type1, pokemon.Type2);
            return Ok(results);
        }

        [HttpGet("offensiveByDex/{dex}")]
        public IActionResult GetOffensiveProfileByDex(int dex)
        {
            var pokemon = _pokemonService.Pokemons.First(p => p.Dex == dex);
            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé pour le numéro #{dex}");
            var results = _typeService.GetOffensiveCoverage(pokemon.Type1, pokemon.Type2);
            return Ok(results);
        }

        [HttpGet("defensiveByName/{name}")]
        public IActionResult GetDefensiveProfileByName(string name)
        {
            var pokemon = _pokemonService.Pokemons
                .First(p => p.Name?.ToLowerInvariant() == name.ToLowerInvariant() || p.Nom?.ToLowerInvariant() == name.ToLowerInvariant());
            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé portant le nom '{name}'");
            var results = _typeService.GetDefensiveProfile(pokemon.Type1, pokemon.Type2);
            return Ok(results);
        }

        [HttpGet("offensiveByName/{name}")]
        public IActionResult GetOffensiveProfileByName(string name)
        {
            var pokemon = _pokemonService.Pokemons
                .First(p => p.Name?.ToLowerInvariant() == name.ToLowerInvariant() || p.Nom?.ToLowerInvariant() == name.ToLowerInvariant());
            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé portant le nom '{name}'");
            var results = _typeService.GetOffensiveCoverage(pokemon.Type1, pokemon.Type2);
            return Ok(results);
        }

        [HttpGet("defense/{dex}")]
        public IActionResult GetDefensiveCoverage(int dex)
        {
            var pokemon = _pokemonService.Pokemons.First(p => p.Dex == dex);
            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé pour le numéro #{dex}");

            var resistances = pokemon.Type2 is null
                ? _typeService.GetAllResistances(pokemon.Type1)
                : _typeService.GetCombinedResistances(pokemon.Type1, pokemon.Type2);

            var result = new
            {
                Pokemon = pokemon.Name,
                Types = new[] { pokemon.Type1, pokemon.Type2 },
                Weaknesses = resistances.Where(kvp => kvp.Value > 1.0).OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Resistances = resistances.Where(kvp => kvp.Value > 0 && kvp.Value < 1.0).OrderBy(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Immunities = resistances.Where(kvp => kvp.Value == 0.0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            return Ok(result);
        }

        [HttpGet("offense/{dex}")]
        public IActionResult GetOffensiveCoverage(int dex)
        {
            var pokemon = _pokemonService.Pokemons.First(p => p.Dex == dex);
            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé pour le numéro #{dex}");

            var offensive = pokemon.Type2 is null
                ? _typeService.GetAllEffectiveness(pokemon.Type1)
                : _typeService.GetCombinedOffensiveCoverage(pokemon.Type1, pokemon.Type2);

            var result = new
            {
                Pokemon = pokemon.Name,
                Types = new[] { pokemon.Type1, pokemon.Type2 },
                SuperEffective = offensive.Where(kvp => kvp.Value > 1.0).OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                NotEffective = offensive.Where(kvp => kvp.Value > 0 && kvp.Value < 1.0).OrderBy(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                Ineffective = offensive.Where(kvp => kvp.Value == 0.0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            return Ok(result);
        }
    }
}
