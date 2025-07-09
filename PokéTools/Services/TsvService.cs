using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using PokéToolsThèque;

namespace PokéTools.Services
{
    public class TsvService
    {
        private readonly HttpClient _http;
        private readonly PokemonService _poke;
        private readonly MoveService _move;

        public TsvService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/")
            };
            _poke = new PokemonService();
            //_move = new MoveService();
        }

        public async Task<List<string>> VerifyAllIdentifiers()
        {
            var failedIdentifiers = new List<string>();
            int success = 0, total = 0;

            foreach (var p in _poke.Pokemons)
            {
                total++;
                try
                {
                    var result = await GetPokemonDataAsync(p.PokeApiIdentifier);
                    if (result != null)
                        success++;
                    else
                        failedIdentifiers.Add(p.PokeApiIdentifier);
                }
                catch (Exception ex)
                {
                    failedIdentifiers.Add(p.PokeApiIdentifier);
                    Console.WriteLine($"❌ Erreur pour {p.PokeApiIdentifier} : {ex.Message}");
                }
            }

            Console.WriteLine($"✅ {success} sur {total} Pokémon trouvés avec succès.");
            Console.WriteLine($"❌ {failedIdentifiers.Count} échecs.");

            return failedIdentifiers;
        }

        public async Task<string> GetAbilityDescription(string ability)
        {
            var page = await _http.GetFromJsonAsync<JsonElement>($"ability/{ability}");

            foreach (var entry in page.GetProperty("effect_entries").EnumerateArray())
            {
                if (entry.GetProperty("language").GetProperty("name").GetString() == "en")
                {
                    return entry.GetProperty("effect").GetString();
                }
            }

            return "No effect found";
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

                var move = _move.Moves.FirstOrDefault(m => m.Name.Equals(moveName, StringComparison.OrdinalIgnoreCase));

                if (move != null)
                {
                    learnableMoves.Add(move);
                }
            }

            return learnableMoves;
        }

        public async Task<string> GetPokemonCategoryAsync(string identifier)
        {
            var species = await _http.GetFromJsonAsync<JsonElement>($"pokemon-species/{identifier.ToLower()}");

            bool isLegendary = species.GetProperty("is_legendary").GetBoolean();
            bool isMythical = species.GetProperty("is_mythical").GetBoolean();
            bool isBaby = species.GetProperty("is_baby").GetBoolean();

            if (isMythical) return "Mythical";
            if (isLegendary) return "Legendary";
            if (isBaby) return "Baby";
            return "Normal";
        }

        public async Task AddCategoryToTsvAsync(string inputPath, string outputPath)
        {
            var lines = await File.ReadAllLinesAsync(inputPath);
            var output = new List<string>();

            // Ajouter le nom de la colonne Category à l'en-tête
            var header = lines[0] + "\tCategory";
            output.Add(header);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var columns = line.Split('\t');

                if (columns.Length < 18)
                {
                    output.Add(line + "\tUnknown");
                    continue;
                }

                var identifier = columns[17]; // colonne PokeApiIdentifier

                try
                {
                    string category = await GetPokemonCategoryAsync(identifier);
                    output.Add(line + $"\t{category}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur pour {identifier} : {ex.Message}");
                    output.Add(line + "\tUnknown");
                }
            }

            await File.WriteAllLinesAsync(outputPath, output);
        }

        public async Task UpdateTsvWithEvolutionDataAsync(string inputPath, string outputPath)
        {
            var lines = await File.ReadAllLinesAsync(inputPath);
            var output = new List<string>();

            var header = lines[0].Replace("EvolutionStage", "IsFullyEvolved");
            output.Add(header);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var columns = line.Split('\t');

                if (columns.Length < 18)
                {
                    output.Add(line);
                    continue;
                }

                var identifier = columns[17]; 

                try
                {
                    bool isFullyEvolved = await IsFullyEvolvedAsync(identifier);
                    columns[columns.Length - 1] = isFullyEvolved.ToString().ToLower();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur pour {identifier} : {ex.Message}");
                    columns[columns.Length - 1] = "false"; 
                }

                output.Add(string.Join('\t', columns));
            }

            await File.WriteAllLinesAsync(outputPath, output);
        }

        public async Task<bool> IsFullyEvolvedAsync(string identifier)
        {
            var pokemonData = await _http.GetFromJsonAsync<JsonElement>($"pokemon/{identifier.ToLower()}");
            var speciesUrl = pokemonData.GetProperty("species").GetProperty("url").GetString();
            var speciesName = pokemonData.GetProperty("species").GetProperty("name").GetString(); 

            var speciesData = await _http.GetFromJsonAsync<JsonElement>(speciesUrl);
            var evoUrl = speciesData.GetProperty("evolution_chain").GetProperty("url").GetString();

            var evoData = await _http.GetFromJsonAsync<JsonElement>(evoUrl);
            var chain = evoData.GetProperty("chain");

            if (chain.GetProperty("evolves_to").GetArrayLength() == 0 &&
                chain.GetProperty("species").GetProperty("name").GetString() == speciesName)
            {
                return true;
            }

            bool IsFullyEvolved(JsonElement node)
            {
                string name = node.GetProperty("species").GetProperty("name").GetString();
                var evolvesTo = node.GetProperty("evolves_to");

                if (name == speciesName)
                {
                    return evolvesTo.GetArrayLength() == 0;
                }

                foreach (var child in evolvesTo.EnumerateArray())
                {
                    if (IsFullyEvolved(child))
                        return true;
                }

                return false;
            }

            return IsFullyEvolved(chain);
        }


        public async Task<PokemonInfo> GetPokemonDataAsync(string identifier) 
        {
            var pokemonData = await _http.GetFromJsonAsync<JsonElement>($"pokemon/{identifier.ToLower()}");
            var speciesUrl = pokemonData.GetProperty("species").GetProperty("url").GetString();

            // Abilities
            var abilitiesList = new List<string>();
            string hiddenAbility = null;
            foreach (var ab in pokemonData.GetProperty("abilities").EnumerateArray())
            {
                var name = ab.GetProperty("ability").GetProperty("name").GetString();
                var url = ab.GetProperty("ability").GetProperty("url").GetString();
                bool isHidden = ab.GetProperty("is_hidden").GetBoolean();
                if (isHidden)
                    hiddenAbility = name;
                else
                    abilitiesList.Add(name);
            }

            // Get evolution chain
            var speciesData = await _http.GetFromJsonAsync<JsonElement>(speciesUrl);
            var evoUrl = speciesData.GetProperty("evolution_chain").GetProperty("url").GetString();
            var evoData = await _http.GetFromJsonAsync<JsonElement>(evoUrl);

            string targetName = pokemonData.GetProperty("name").GetString();

            int FindStage(JsonElement node, string name, int stage = 1)
            {
                if (node.GetProperty("species").GetProperty("name").GetString() == name)
                    return stage;

                foreach (var child in node.GetProperty("evolves_to").EnumerateArray())
                {
                    int found = FindStage(child, name, stage + 1);
                    if (found > 0)
                        return found;
                }

                return 0;
            }

            int stageNumber = FindStage(evoData.GetProperty("chain"), targetName);

            return new PokemonInfo
            {
                Name = targetName,
                Abilities = abilitiesList,
                HiddenAbility = hiddenAbility,
                EvolutionStage = stageNumber
            };
        }

        public async Task EnrichTsvWithAbilitiesAndEvoAsync()
        {
            string inputPath = "E:\\Bureau\\CODE\\PokéTools\\Data\\pokedex.tsv";
            string outputPath = "E:\\Bureau\\CODE\\PokéTools\\Data\\pokedex_updated.tsv";

            var lines = File.ReadAllLines(inputPath, Encoding.UTF8).ToList();
            if (lines.Count == 0) throw new Exception("Fichier vide");

            var headers = lines[0].Split('\t').ToList();

            headers.Add("Abilities");
            headers.Add("HiddenAbility");
            headers.Add("EvolutionStage");

            var outputLines = new List<string> { string.Join("\t", headers) };

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split('\t');
                Console.WriteLine(parts[0]);

                try
                {
                    var data = await GetPokemonDataAsync(parts[17]);

                    string abilities = data.Abilities != null ? string.Join(",", data.Abilities) : "";
                    string hiddenAbility = data.HiddenAbility ?? "";
                    string evoStage = data.EvolutionStage.ToString();

                    var enrichedLine = line + "\t" + abilities + "\t" + hiddenAbility + "\t" + evoStage;
                    outputLines.Add(enrichedLine);
                }
                catch
                {
                    var enrichedLine = line + "\t\t\t";
                    outputLines.Add(enrichedLine);
                }
            }

            File.WriteAllLines(outputPath, outputLines, Encoding.UTF8);
        }

        public async Task FetchAllItems()
        {
            var itemsUrl = "item?limit=10000";
            var heldItems = new List<Item>();

            try
            {
                var response = await _http.GetAsync(itemsUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Impossible de récupérer la liste des items.");
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                using var itemsDoc = JsonDocument.Parse(content);

                if (!itemsDoc.RootElement.TryGetProperty("results", out var results))
                {
                    Console.WriteLine("❌ Pas de propriété 'results' dans la réponse.");
                    return;
                }

                foreach (var element in results.EnumerateArray())
                {
                    var itemUrl = element.GetProperty("url").GetString();
                    if (string.IsNullOrEmpty(itemUrl)) continue;

                    var itemResponse = await _http.GetAsync(itemUrl);
                    if (!itemResponse.IsSuccessStatusCode) continue;

                    var itemJson = await itemResponse.Content.ReadAsStringAsync();
                    using var itemDoc = JsonDocument.Parse(itemJson);
                    var root = itemDoc.RootElement;

                    if (!root.TryGetProperty("attributes", out var attributes)) continue;

                    bool isHoldable = attributes.EnumerateArray()
                        .Any(attr => attr.TryGetProperty("name", out var attrName) &&
                                     attrName.GetString() == "holdable");

                    if (!isHoldable) continue;

                    var name = root.GetProperty("name").GetString();

                    string description = "";
                    if (root.TryGetProperty("flavor_text_entries", out var flavorEntries))
                    {
                        description = flavorEntries.EnumerateArray()
                            .Where(entry =>
                                entry.ValueKind == JsonValueKind.Object &&
                                entry.TryGetProperty("language", out var lang) &&
                                lang.ValueKind == JsonValueKind.Object &&
                                lang.TryGetProperty("name", out var langName) &&
                                langName.GetString() == "en")
                            .Select(entry =>
                                entry.TryGetProperty("text", out var textProp)
                                    ? textProp.GetString()
                                    : null)
                            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text))?
                            .Replace("\n", " ")
                            .Replace("\t", " ")
                            .Trim() ?? "";

                    }

                    heldItems.Add(new Item
                    {
                        Name = name,
                        Description = description
                    });

                    Console.WriteLine($"✔ {name}");

                    await Task.Delay(100); // Rate limit
                }

                // Écriture fichier TSV
                var outputPath = "E:\\Bureau\\CODE\\PokéTools\\Data\\items.tsv";
                using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);
                writer.WriteLine("item_name\titem_description");

                foreach (var item in heldItems)
                {
                    writer.WriteLine($"{item.Name}\t{item.Description}");
                }

                Console.WriteLine("✅ Fichier items.tsv généré avec {0} objets.", heldItems.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erreur dans FetchAllItems : " + ex.Message);
            }
        }


        public async Task FetchAllMovesAndExportAsync(string outputFilePath)
        {
            var httpClient = new HttpClient();
            var allMoveUrls = new HashSet<string>();

            foreach (var name in _poke.Pokemons.Select(p => p.PokeApiIdentifier))
            {
                var response = await httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}");
                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync();
                var pokemonData = JsonDocument.Parse(json);
                var moves = pokemonData.RootElement.GetProperty("moves");

                foreach (var move in moves.EnumerateArray())
                {
                    var moveUrl = move.GetProperty("move").GetProperty("url").GetString();
                    if (!string.IsNullOrEmpty(moveUrl))
                    {
                        allMoveUrls.Add(moveUrl);
                    }
                }
            }

            var moveInfos = new List<Move>();

            foreach (var url in allMoveUrls)
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync();
                var moveData = JsonDocument.Parse(json).RootElement;

                var name = moveData.GetProperty("name").GetString();
                var pp = moveData.TryGetProperty("pp", out var ppProp) ? ppProp.GetInt32() : 0;
                var power = moveData.TryGetProperty("power", out var powerProp) && powerProp.ValueKind != JsonValueKind.Null ? powerProp.GetInt32() : 0;
                var damageClass = moveData.GetProperty("damage_class").GetProperty("name").GetString();
                var type = moveData.GetProperty("type").GetProperty("name").GetString();

                var accuracy = moveData.TryGetProperty("accuracy", out var accProp) && accProp.ValueKind != JsonValueKind.Null
                ? accProp.GetInt32()
                : (int?)null;

                var flavorTexts = moveData.GetProperty("flavor_text_entries");
                string description = "";

                var englishEntry = flavorTexts.EnumerateArray()
                    .FirstOrDefault(e => e.TryGetProperty("language", out var lang) && lang.GetProperty("name").GetString() == "en");

                if (englishEntry.ValueKind != JsonValueKind.Undefined && englishEntry.TryGetProperty("flavor_text", out var descProp))
                {
                    description = descProp.GetString()
                        .Replace("\n", " ")
                        .Replace("\r", " ");
                }

                moveInfos.Add(new Move
                {
                    Name = name,
                    PP = pp,
                    Power = power,
                    Accuracy = accuracy,
                    DamageClass = damageClass,
                    Type = type,
                    Description = description
                });
            }

            var lines = new List<string> { "Name\tPP\tPower\tAccuracy\tDamageClass\tType\tDescription" };
            foreach (var move in moveInfos.OrderBy(m => m.Name).DistinctBy(m => m.Name))
            {
                lines.Add($"{move.Name}\t{move.PP}\t{move.Power}\t{(move.Accuracy?.ToString() ?? "")}\t{move.DamageClass}\t{move.Type}\t{move.Description}");
            }

            await File.WriteAllLinesAsync(outputFilePath, lines);
        }

        public async Task AddSmogonTierFromLocalFileAsync(string inputTsvPath, string outputTsvPath, string showdownFilePath)
        {
            var tiers = ParseLocalShowdownTiers(showdownFilePath);
            var lines = await File.ReadAllLinesAsync(inputTsvPath);
            var output = new List<string> { lines[0] + "\tSmogonTier" };

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var cols = line.Split('\t');
                if (cols.Length < 18)
                {
                    output.Add(line + "\tUnknown");
                    continue;
                }

                var pokeApiId = cols[17];
                var showdownId = pokeApiId.Replace("-", "").ToLowerInvariant();
                tiers.TryGetValue(showdownId, out var tier);
                output.Add(line + "\t" + (tier ?? "Unknown"));
            }

            await File.WriteAllLinesAsync(outputTsvPath, output);
        }


        public Dictionary<string, string> ParseLocalShowdownTiers(string filePath)
        {
            var dict = new Dictionary<string, string>();
            var text = File.ReadAllText(filePath);

            var regex = new Regex(@"(?<id>[a-z0-9]+)\s*:\s*\{\s*[^}]*?tier\s*:\s*""(?<tier>[^""]+)""", RegexOptions.Compiled);

            int count = 0;
            foreach (Match m in regex.Matches(text))
            {
                var id = m.Groups["id"].Value.Trim();
                var tier = m.Groups["tier"].Value.Trim();
                dict[id] = tier;
                count++;
            }

            Console.WriteLine($"✔ {count} Pokémon tiers extraits.");
            return dict;
        }


        private async Task<Dictionary<string, string>> FetchShowdownTiersAsync()
        {
            var url = "https://raw.githubusercontent.com/smogon/pokemon-showdown/master/data/formats-data.ts";
            var txt = await _http.GetStringAsync(url);
            var dict = new Dictionary<string, string>();
            var regex = new Regex(@"[""'](?<id>[^""']+)[""']\s*:\s*\{\s*tier\s*:\s*[""'](?<tier>[^""']+)[""']");
            foreach (Match m in regex.Matches(txt))
            {
                dict[m.Groups["id"].Value] = m.Groups["tier"].Value;
            }
            return dict;
        }

        private string ToShowdownId(string pokeApiIdentifier)
            => pokeApiIdentifier.Replace("-", "").ToLowerInvariant();

        public async Task AddSmogonTierToTsvAsync(string input, string output)
        {
            var tiers = await FetchShowdownTiersAsync();
            var lines = await File.ReadAllLinesAsync(input);
            var outLines = new List<string>();
            outLines.Add(lines[0] + "\tSmogonTier");
            foreach (var line in lines.Skip(1))
            {
                var cols = line.Split('\t');
                var id = cols[17];
                var sdId = ToShowdownId(id);
                tiers.TryGetValue(sdId, out var tier);
                outLines.Add(line + "\t" + (tier ?? "Unknown"));
            }
            await File.WriteAllLinesAsync(output, outLines);
        }
    }
}
