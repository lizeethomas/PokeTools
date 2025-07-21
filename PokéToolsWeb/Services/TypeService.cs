namespace PokéToolsWeb.Services
{
    public class TypeService
    {

        private readonly Dictionary<string, Dictionary<string, double>> _typeChart;

        public enum OffensiveEffectivenessCategory
        {
            Ineffective,
            NotVeryEffective,
            Neutral,
            SuperEffective
        }

        public class OffensiveAnalysisResult
        {
            public int Ineffective { get; set; }
            public int NotVeryEffective { get; set; }
            public int Neutral { get; set; }
            public int SuperEffective { get; set; }
        }

        public TypeService()
        {
            _typeChart = new Dictionary<string, Dictionary<string, double>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Normal"] = new Dictionary<string, double>
                {
                    ["Rock"] = 0.5,
                    ["Ghost"] = 0.0,
                    ["Steel"] = 0.5
                },
                ["Fire"] = new Dictionary<string, double>
                {
                    ["Grass"] = 2.0,
                    ["Ice"] = 2.0,
                    ["Bug"] = 2.0,
                    ["Steel"] = 2.0,
                    ["Fire"] = 0.5,
                    ["Water"] = 0.5,
                    ["Rock"] = 0.5,
                    ["Dragon"] = 0.5
                },
                ["Water"] = new Dictionary<string, double>
                {
                    ["Fire"] = 2.0,
                    ["Ground"] = 2.0,
                    ["Rock"] = 2.0,
                    ["Water"] = 0.5,
                    ["Grass"] = 0.5,
                    ["Dragon"] = 0.5
                },
                ["Electric"] = new Dictionary<string, double>
                {
                    ["Water"] = 2.0,
                    ["Flying"] = 2.0,
                    ["Electric"] = 0.5,
                    ["Grass"] = 0.5,
                    ["Dragon"] = 0.5,
                    ["Ground"] = 0.0
                },
                ["Grass"] = new Dictionary<string, double>
                {
                    ["Water"] = 2.0,
                    ["Ground"] = 2.0,
                    ["Rock"] = 2.0,
                    ["Fire"] = 0.5,
                    ["Grass"] = 0.5,
                    ["Poison"] = 0.5,
                    ["Flying"] = 0.5,
                    ["Bug"] = 0.5,
                    ["Dragon"] = 0.5,
                    ["Steel"] = 0.5
                },
                ["Ice"] = new Dictionary<string, double>
                {
                    ["Grass"] = 2.0,
                    ["Ground"] = 2.0,
                    ["Flying"] = 2.0,
                    ["Dragon"] = 2.0,
                    ["Fire"] = 0.5,
                    ["Water"] = 0.5,
                    ["Ice"] = 0.5,
                    ["Steel"] = 0.5
                },
                ["Fighting"] = new Dictionary<string, double>
                {
                    ["Normal"] = 2.0,
                    ["Ice"] = 2.0,
                    ["Rock"] = 2.0,
                    ["Dark"] = 2.0,
                    ["Steel"] = 2.0,
                    ["Poison"] = 0.5,
                    ["Flying"] = 0.5,
                    ["Psychic"] = 0.5,
                    ["Bug"] = 0.5,
                    ["Fairy"] = 0.5,
                    ["Ghost"] = 0.0
                },
                ["Poison"] = new Dictionary<string, double>
                {
                    ["Grass"] = 2.0,
                    ["Fairy"] = 2.0,
                    ["Poison"] = 0.5,
                    ["Ground"] = 0.5,
                    ["Rock"] = 0.5,
                    ["Ghost"] = 0.5,
                    ["Steel"] = 0.0
                },
                ["Ground"] = new Dictionary<string, double>
                {
                    ["Fire"] = 2.0,
                    ["Electric"] = 2.0,
                    ["Poison"] = 2.0,
                    ["Rock"] = 2.0,
                    ["Steel"] = 2.0,
                    ["Grass"] = 0.5,
                    ["Bug"] = 0.5,
                    ["Flying"] = 0.0
                },
                ["Flying"] = new Dictionary<string, double>
                {
                    ["Grass"] = 2.0,
                    ["Fighting"] = 2.0,
                    ["Bug"] = 2.0,
                    ["Electric"] = 0.5,
                    ["Rock"] = 0.5,
                    ["Steel"] = 0.5
                },
                ["Psychic"] = new Dictionary<string, double>
                {
                    ["Fighting"] = 2.0,
                    ["Poison"] = 2.0,
                    ["Psychic"] = 0.5,
                    ["Steel"] = 0.5,
                    ["Dark"] = 0.0
                },
                ["Bug"] = new Dictionary<string, double>
                {
                    ["Grass"] = 2.0,
                    ["Psychic"] = 2.0,
                    ["Dark"] = 2.0,
                    ["Fire"] = 0.5,
                    ["Fighting"] = 0.5,
                    ["Poison"] = 0.5,
                    ["Flying"] = 0.5,
                    ["Ghost"] = 0.5,
                    ["Steel"] = 0.5,
                    ["Fairy"] = 0.5
                },
                ["Rock"] = new Dictionary<string, double>
                {
                    ["Fire"] = 2.0,
                    ["Ice"] = 2.0,
                    ["Flying"] = 2.0,
                    ["Bug"] = 2.0,
                    ["Fighting"] = 0.5,
                    ["Ground"] = 0.5,
                    ["Steel"] = 0.5
                },
                ["Ghost"] = new Dictionary<string, double>
                {
                    ["Psychic"] = 2.0,
                    ["Ghost"] = 2.0,
                    ["Dark"] = 0.5,
                    ["Normal"] = 0.0
                },
                ["Dragon"] = new Dictionary<string, double>
                {
                    ["Dragon"] = 2.0,
                    ["Steel"] = 0.5,
                    ["Fairy"] = 0.0
                },
                ["Dark"] = new Dictionary<string, double>
                {
                    ["Psychic"] = 2.0,
                    ["Ghost"] = 2.0,
                    ["Fighting"] = 0.5,
                    ["Dark"] = 0.5,
                    ["Fairy"] = 0.5
                },
                ["Steel"] = new Dictionary<string, double>
                {
                    ["Ice"] = 2.0,
                    ["Rock"] = 2.0,
                    ["Fairy"] = 2.0,
                    ["Fire"] = 0.5,
                    ["Water"] = 0.5,
                    ["Electric"] = 0.5,
                    ["Steel"] = 0.5
                },
                ["Fairy"] = new Dictionary<string, double>
                {
                    ["Fighting"] = 2.0,
                    ["Dragon"] = 2.0,
                    ["Dark"] = 2.0,
                    ["Fire"] = 0.5,
                    ["Poison"] = 0.5,
                    ["Steel"] = 0.5
                }
            };
        }

        public double GetEffectiveness(string attackingType, params string[] defendingTypes)
        {
            double effectiveness = 1.0;

            foreach (var defType in defendingTypes.Where(t => !string.IsNullOrEmpty(t)))
            {
                if (_typeChart.TryGetValue(attackingType, out var innerDict) &&
                    innerDict.TryGetValue(defType, out var multiplier))
                {
                    effectiveness *= multiplier;
                }
                else
                {
                    effectiveness *= 1.0;
                }
            }

            return effectiveness;
        }


        public double GetResistance(string defendingType, params string[] attackingTypes)
        {
            double resistance = 1.0;

            foreach (var atkType in attackingTypes)
            {
                if (_typeChart.TryGetValue(atkType, out var innerDict) &&
                    innerDict.TryGetValue(defendingType, out var multiplier))
                {
                    resistance *= multiplier;
                }
                else
                {
                    resistance *= 1.0;
                }
            }

            return resistance;
        }

        public Dictionary<string, double> GetAllResistances(string defendingType)
        {
            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var atkType in _typeChart.Keys)
            {
                if (_typeChart[atkType].TryGetValue(defendingType, out double multiplier))
                {
                    result[atkType] = multiplier;
                }
                else
                {
                    result[atkType] = 1.0;
                }
            }

            return result;
        }

        public Dictionary<string, double> GetAllEffectiveness(string attackingType)
        {
            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            if (!_typeChart.TryGetValue(attackingType, out var matchups))
            {
                foreach (var defType in _typeChart.Keys)
                {
                    result[defType] = 1.0;
                }
                return result;
            }

            foreach (var defType in _typeChart.Keys)
            {
                if (matchups.TryGetValue(defType, out double multiplier))
                {
                    result[defType] = multiplier;
                }
                else
                {
                    result[defType] = 1.0;
                }
            }

            return result;
        }

        public Dictionary<string, double> GetCombinedResistances(string type1, string type2)
        {
            var combined = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var atkType in _typeChart.Keys)
            {
                double mult1 = 1.0;
                double mult2 = 1.0;

                if (_typeChart[atkType].TryGetValue(type1, out var m1))
                    mult1 = m1;

                if (_typeChart[atkType].TryGetValue(type2, out var m2))
                    mult2 = m2;

                combined[atkType] = mult1 * mult2;
            }

            return combined;
        }

        public Dictionary<string, double> GetCombinedOffensiveCoverage(string attackType1, string attackType2)
        {
            var combined = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var defType in _typeChart.Keys)
            {
                double mult1 = 1.0;
                double mult2 = 1.0;

                if (_typeChart[attackType1].TryGetValue(defType, out var m1))
                    mult1 = m1;

                if (_typeChart[attackType2].TryGetValue(defType, out var m2))
                    mult2 = m2;

                combined[defType] = Math.Max(mult1, mult2);
            }

            return combined;
        }

        public Dictionary<string, double> GetTeamCombinedDefensiveCoverage(List<(string Type1, string Type2)> teamTypes)
        {
            var combined = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var attackingType in _typeChart.Keys)
            {
                combined[attackingType] = 1.0;
            }

            foreach (var (type1, type2) in teamTypes)
            {
                var indivCoverage = GetCombinedResistances(type1, type2);
                foreach (var kvp in indivCoverage)
                {
                    combined[kvp.Key] *= kvp.Value;
                }
            }

            return combined;
        }

        public Dictionary<string, double> GetDefensiveProfile(string type1, string? type2 = null)
        {
            var profile = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var attackingType in _typeChart.Keys)
            {
                double mult1 = 1.0;
                double mult2 = 1.0;

                if (_typeChart[attackingType].TryGetValue(type1, out var m1))
                    mult1 = m1;

                if (!string.IsNullOrEmpty(type2) && _typeChart[attackingType].TryGetValue(type2, out var m2))
                    mult2 = m2;

                profile[attackingType] = mult1 * mult2;
            }

            return profile;
        }

        public Dictionary<string, double> GetOffensiveCoverage(string type1, string? type2 = null)
        {
            var coverage = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var defendingType in _typeChart.Keys)
            {
                double mult1 = 1.0;
                double mult2 = 1.0;

                if (_typeChart.TryGetValue(type1, out var dict1) && dict1.TryGetValue(defendingType, out var m1))
                    mult1 = m1;

                if (!string.IsNullOrEmpty(type2) && _typeChart.TryGetValue(type2, out var dict2) && dict2.TryGetValue(defendingType, out var m2))
                    mult2 = m2;

                coverage[defendingType] = string.IsNullOrEmpty(type2) ? mult1 : Math.Max(mult1, mult2);
            }

            return coverage;
        }

    }
}