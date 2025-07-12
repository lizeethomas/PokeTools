﻿using Microsoft.AspNetCore.Mvc;
using PokéTools.Services;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PokéTools.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ToolController : ControllerBase
    {
        private PokemonService _pokemonService;
        private TsvService _tsvService;

        public ToolController()
        {
            _pokemonService = new PokemonService();
            _tsvService = new TsvService();
        }


        [HttpGet("IsFullyEvolved/{identifier}")]
        public async Task<ActionResult<bool>> GetIsFullyEvolved(string identifier)
        {
            var result = await _tsvService.IsFullyEvolvedAsync(identifier);
            return Ok(result);
        }

        [HttpGet("updatetsv")]
        public async Task<ActionResult> Updatetsv()
        {
            await _tsvService.AddSmogonTierFromLocalFileAsync(
            "E:\\Bureau\\CODE\\PokéToolsProject\\PokéTools\\Data\\pokemons.tsv",
            "E:\\Bureau\\CODE\\PokéToolsProject\\PokéTools\\Data\\pokedex.tsv",
            "E:\\Bureau\\CODE\\PokéToolsProject\\PokéTools\\Data\\format-data.ts"
        );
            return Ok("");
        }

        [HttpGet("profile/{name}")]
        public IActionResult GetTiers(string name)
        {
            return Ok(ToolService.GetTiers(_pokemonService.Pokemons.First(p => p.Name == name || p.Nom == name)));
        }

        [HttpGet("bulk/{name}")]
        public IActionResult GetBulk(string name)
        {
            return Ok(ToolService.GetBulk(_pokemonService.Pokemons.First(p => p.Name == name || p.Nom == name)));
        }

        [HttpGet("power/{name}")]
        public IActionResult GetPower(string name)
        {
            return Ok(ToolService.GetPower(_pokemonService.Pokemons.First(p => p.Name == name || p.Nom == name)));
        }

        [HttpGet("bulk/top")]
        public IActionResult GetTopBulk()
        {
            var result = new Dictionary<string, double>();
            for (int i = 1; i < 1026; i++)
            {
                result.Add(_pokemonService.Pokemons.First(p => p.Dex == i).Nom ?? "", ToolService.GetBulk(_pokemonService.Pokemons.First(p => p.Dex == i)));
            }
            return Ok(result.OrderByDescending(r => r.Value).Take(50));
        }

        [HttpGet("power/top")]
        public IActionResult GetTopPower()
        {
            var result = new Dictionary<string, double>();
            for (int i = 1; i < 1026; i++)
            {
                result.Add(_pokemonService.Pokemons.First(p => p.Dex == i).Nom ?? "", ToolService.GetPower(_pokemonService.Pokemons.First(p => p.Dex == i)));
            }
            return Ok(result.OrderByDescending(r => r.Value).Take(50));
        }

        [HttpGet("bulk/flop")]
        public IActionResult GetFlopBulk()
        {
            var result = new Dictionary<string, double>();
            for (int i = 1; i < 1026; i++)
            {
                result.Add(_pokemonService.Pokemons.First(p => p.Dex == i).Nom ?? "", ToolService.GetBulk(_pokemonService.Pokemons.First(p => p.Dex == i)));
            }
            return Ok(result.OrderBy(r => r.Value).Take(50));
        }

        [HttpGet("power/flop")]
        public IActionResult GetFlopPower()
        {
            var result = new Dictionary<string, double>();
            for (int i = 1; i < 1026; i++)
            {
                result.Add(_pokemonService.Pokemons.First(p => p.Dex == i).Nom ?? "", ToolService.GetPower(_pokemonService.Pokemons.First(p => p.Dex == i)));
            }
            return Ok(result.OrderBy(r => r.Value).Take(50));
        }

        [HttpGet("ability/{name}")] 
        public async Task<IActionResult> GetAbilityDescription(string name)
        {
            await _tsvService.GetAbilityDescription(name);
            return Ok("done");
        }

        [HttpGet("writemoves")]
        public async Task<IActionResult> WriteMoves()
        {
            Console.WriteLine(_pokemonService.Pokemons.Count(p => p.SmogonTier == "Unknown"));
            return Ok("");
        }
    }
}
