using Microsoft.AspNetCore.Mvc;
using PokéToolsThèque;
using PokéTools.Services;

namespace PokéTools.Controllers
{
    [ApiController]
    [Route("api/")]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonService _pokemonService;
        private readonly AbilityService _abilityService;
        private readonly MoveService _moveService;
        private readonly ItemService _itemService;

        public PokemonController()
        {
            _pokemonService = new PokemonService();
            _abilityService = new AbilityService();
            _moveService = new MoveService();
            _itemService = new ItemService();
        }

        [HttpGet("Pokemon/{dex}")]
        public ActionResult<Pokemon> GetPokemon(int dex)
        {
            var pokemon = _pokemonService.Pokemons.FirstOrDefault(p => p.Dex == dex);

            if (pokemon == null)
                return NotFound($"Aucun Pokémon trouvé avec le numéro Dex {dex}");

            return Ok(pokemon);
        }

        [HttpGet("Pokemons")]
        public ActionResult<List<Pokemon>> GetPokemons()
        {
            var pokemons = _pokemonService.Pokemons;
            return Ok(pokemons);
        }

        [HttpGet("Abilities")]
        public ActionResult<List<Ability>> GetAbilities()
        {
            var abilities = _abilityService.Abilities;
            return Ok(abilities);
        }

        [HttpGet("Moves")]
        public ActionResult<List<Ability>> GetMoves()
        {
            var moves = _moveService.Moves;
            return Ok(moves);
        }

        [HttpGet("Items")]
        public ActionResult<List<Item>> GetItems()
        {
            var items = _itemService.Items
                .Where(i => i.Category == "Berries" || i.Category == "Hold items");
            return Ok(items);
        }

        [HttpGet("Pokemon/{name}/Moves")]
        public async Task<IActionResult> GetMovesFromDex(string name)
        {
            var moves = await _moveService.GetLearnableMovesAsync(name);
            return Ok(moves);
        }
    }
}
