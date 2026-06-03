using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Pools.Rollables;
using ConsoleBalatro.Engine.Pools.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Pools
{
    public static class PoolManager
    {
        public static readonly List<string> SpecialPlanets = new()
        {
            "Ceres",
            "Planet X",
            "Eris",
        };

        public static readonly List<PlayedHandType> SpecialHandTypes = new()
        {
            PlayedHandType.FIVEOFAKIND,
            PlayedHandType.FLUSHHOUSE,
            PlayedHandType.FLUSHFIVE
        };

        //Joker master pool is the normal JokerMetadata pool from JokerDb
        //These are the immutable master pools
        public static Dictionary<string, RollableDefinition> TarotCardPool = new Dictionary<string, RollableDefinition>();
        public static Dictionary<string, RollableDefinition> SpectralCardPool = new Dictionary<string, RollableDefinition>();
        public static Dictionary<string, RollableDefinition> PlanetCardPool = new Dictionary<string, RollableDefinition>();
        public static Dictionary<string, RollableDefinition> JokerPool = new Dictionary<string, RollableDefinition>();
        public static Dictionary<string, RollableDefinition> PackPool = new Dictionary<string, RollableDefinition>();
        public static Dictionary<string, RollableDefinition> VoucherPool = new Dictionary<string, RollableDefinition>();
        public static Dictionary<string, RollableDefinition> PlayingCardPool = new Dictionary<string, RollableDefinition>();

        public static List<IMarketPoolRule> Rules = new List<IMarketPoolRule>();

        public static void Initialize()
        {
            InitializeAllPools();
            InitializeGlobalPoolRules();
        }

        public static void InitializeAllPools()
        {
            TarotCardPool.Clear();
            SpectralCardPool.Clear();
            PlanetCardPool.Clear();
            JokerPool.Clear();
            PackPool.Clear();
            VoucherPool.Clear();
            PlayingCardPool.Clear();

            foreach (var tc in ConsumableManager.TarotNames)
            {
                TarotCardPool.Add(tc, new TarotRollableDefinition(tc));
            }

            foreach (var sc in ConsumableManager.SpectralNames)
            {
                SpectralCardPool.Add(sc, new SpectralRollableDefinition(sc));
            }

            foreach (var pc in ConsumableManager.PlanetCardNames)
            {
                if (!SpecialPlanets.Contains(pc.Value))
                    PlanetCardPool.Add(pc.Value, new PlanetRollableDefinition(pc.Value.ToUpper()));
                else
                    PlanetCardPool.Add(pc.Value, new SpecialPlanetRollableDefinition(pc.Value.ToUpper(), pc.Key));
            }
            foreach (var j in JokerDb.JokerMetadata.Values)
            {
                JokerPool.Add(j.DBName, new JokerRollableDefinition(j));
            }

            foreach (var p in ConsumableManager.PackBasicNums.Values.Select(x => x.ID))
            {
                PackPool.Add(p, new PackRollableDefinition(p));
            }

            foreach (var v in VoucherDb.VoucherDBNames)
            {
                VoucherPool.Add(v, new VoucherRollableDefinition(v));
            }

            foreach (var rankS in new List<string> { "A", "K", "Q", "J", "1", "9", "8", "7", "6", "5", "4", "3", "2"})
            {
                foreach (var suitS in new List<string> { "S", "C", "H", "D"})
                {
                    var c = rankS + suitS;
                    PlayingCardPool.Add(c, new PlayingCardRollableDefinition() { Id = c });
                }
            }
        }

        public static void InitializeGlobalPoolRules()
        {
            Rules.Clear();
            Rules.Add(new NoOwnedDuplicatesRule());
            Rules.Add(new ShowmanMarketRule());
            Rules.Add(new JokerForcedRarityRule());
            Rules.Add(new JokerNoLegosRule());
            Rules.Add(new VoucherPoolRules());
            Rules.Add(new PackRareReplacementRule());
            Rules.Add(new SpectralNoSoulBlackHoleRule());
        }

        public static RollableDefinition RollSingle(ContentRollRequest request)
        {
            var candidates = BuildCandidates(request);

            if(candidates.Count == 0)
                throw new InvalidOperationException("No eligible items found in the pool for the given request.");

            var rarity = request.ForcedRarity ?? RaritySelector.SelectRarity(new RaritySelectionContext
            {
                Pool = request.Pool,
                Source = request.Source,
                Candidates = candidates,
            });

            var candidatesOfRarity = candidates.Where(c => c.Definition.Rarity == rarity).ToList();

            if (candidatesOfRarity.Count == 0)
            {
                candidatesOfRarity = ApplyRarityFallback(candidates, rarity);
            }

            var chosen = PickWeighted(candidatesOfRarity);

            request.Batch?.GeneratedIds.Add(chosen.Definition.Id);

            return chosen.Definition;
        }

        private static List<WeightedCandidate> BuildCandidates(ContentRollRequest request)
        {
            var pool = request.Pool switch
            {
                ItemPool.Tarot => TarotCardPool,
                ItemPool.Spectral => SpectralCardPool,
                ItemPool.Planet => PlanetCardPool,
                ItemPool.Joker => JokerPool,
                ItemPool.Pack => PackPool,
                ItemPool.Voucher => VoucherPool,
                ItemPool.PlayingCard => PlayingCardPool,
                _ => throw new ArgumentException("Invalid content pool specified.")
            };
            if (pool.Count == 0)
                throw new InvalidOperationException($"The {request.Pool} pool is empty.");

            var eligibilityContext = new MarketEligibilityContext { 
                Pool = request.Pool,
                Source = request.Source
            };

            var poolContext = new MarketPoolContext
            {
                Pool = request.Pool,
                Source = request.Source,
                BatchContext = request.Batch,

            };

            foreach (var def in pool.Values)
            {
                if((request.Batch != null && !request.Batch.AllowDuplicateResultsInSameBatch) && request.Batch.GeneratedIds.Contains(def.Id))
                    continue;
                if(!def.IsEligible(eligibilityContext))
                    continue;

                poolContext.Candidates.Add(new WeightedCandidate 
                { 
                    Definition = def, 
                    Weight = def.BaseWeight
                });
            }

            foreach (var rule in Rules.OrderBy(r => r.Priority))
            {
                rule.ModifyCandidates(poolContext);
            }

            poolContext.Candidates.RemoveAll(x => x.Weight <= 0);

            return poolContext.Candidates;
        }

        private static List<WeightedCandidate> ApplyRarityFallback(
        List<WeightedCandidate> candidates,
        JokerRarity desiredRarity)
        {
            // Simple fallback: if desired rarity is impossible,
            // roll from all available candidates instead.
            //
            // You could replace this with:
            // - next lower rarity
            // - next higher rarity
            // - reroll rarity
            // - throw exception
            return candidates;
        }

        private static WeightedCandidate PickWeighted(IReadOnlyList<WeightedCandidate> candidates)
        {
            int totalWeight = candidates.Sum(c => c.Weight);

            if (totalWeight <= 0)
                throw new InvalidOperationException("Cannot roll from candidates with no positive weight.");

            int roll = Globals.randomNext(1, totalWeight + 1);

            int running = 0;

            foreach (var candidate in candidates)
            {
                running += candidate.Weight;

                if (roll <= running)
                    return candidate;
            }

            return candidates[^1];
        }
    }
}
