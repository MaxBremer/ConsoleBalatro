using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;

namespace ConsoleBalatro.Engine.Challenges;

public static class ChallengeManager
{
    private static readonly Dictionary<string, ChallengeDefinition> Definitions =
        new(StringComparer.OrdinalIgnoreCase);

    public static Dictionary<string, Func<ChallengeDefinition>> ChallengeDb = new()
    {
        {"THE OMELETTE", () =>
        {
            var omelette = new ChallengeDefinition
            {
                Id = "THE OMELETTE",
                Name = "The Omelette",
                ChallengeIndex = 1,
                Description = "All Blinds give no reward money. Extra hands no longer earn money. Earn no Interest at end of round."
            };
            for (var i = 0; i < 5; i++)
                omelette.StartingJokers.Add(() => JokerDb.GenerateJokerCard("EGG"));
            omelette.BannedPoolItems.Add(Pools.ItemPool.Voucher, new HashSet<string> { "SEED MONEY", "MONEY TREE" });
            omelette.BannedPoolItems.Add(Pools.ItemPool.Joker, new HashSet<string> { "TO THE MOON", "ROCKET", "GOLDEN JOKER", "SATELLITE" });
            omelette.CustomRulesJokerBuilder = c =>
            {
                var ret = JokerDb.BasicDataBlock("Omelette challenge", "All Blinds give no reward money. Extra hands no longer earn money. Earn no Interest at end of round.");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GatherPostRoundMoney,
                    MyAction = args =>
                    {
                        if (args is EngineGatherPostRoundMoneyArgs mArgs)
                        {
                            mArgs.ExistingSources.RemoveAll(x => x.Item1 == "Interest");
                            mArgs.ExistingSources.RemoveAll(x => x.Item1 == "Hands Remaining");
                            mArgs.ExistingSources.RemoveAll(x => x.Item1 == "Blind");
                        }
                    }
                });

                return ret;
            };
            return omelette;
        } },
        {"15 MINUTE CITY", () =>
        {
            var challengeDef = new ChallengeDefinition
            {
                Id = "15 MINUTE CITY",
                Name = "15 Minute City",
                ChallengeIndex = 2,
                Description = ""
            };

            string[] jokes = ["RIDE THE BUS", "SHORTCUT"];
            foreach (var j in jokes)
            {
                challengeDef.StartingJokers.Add(() =>
                {
                    var ret = JokerDb.GenerateJokerCard(j);
                    ret.AddSticker(Sticker.ETERNAL);
                    return ret;
                });
            }

            challengeDef.ModifyStartingDeck = cz =>
            {
                //Remove Aces, Twos, and Threes.
                List<Rank> toRemSuits = [Rank.ACE, Rank.TWO, Rank.THREE];
                var toRem = new List<Card>();
                toRem.AddRange(cz.Cards.Where(c => toRemSuits.Contains(c.Rank)));
                foreach (var cr in toRem)
                {
                    cz.RemoveCard(cr);
	            }

                //Extra copy of each face card
                List<Rank> faceRanks = [Rank.JACK, Rank.QUEEN, Rank.KING];
                List<Suit> realSuits = [Suit.DIAMONDS, Suit.HEARTS, Suit.CLUBS, Suit.SPADES];
                foreach (var suit in realSuits)
                {
                    foreach (var rank in faceRanks)
                    {
                        cz.AddCard(CardFactory.PlayingCardFromRankSuit(rank, suit));
	                }
	            }
            };

            return challengeDef;
        } },

        {"RICH GET RICHER", () =>
        {
            var challengeDef = new ChallengeDefinition
            {
                Id = "RICH GET RICHER",
                Name = "Rich get Richer",
                ChallengeIndex = 3,
                Description = "Chips cannot exceed the current $. Start with $100."
            };

            string[] vouches = ["SEED MONEY", "MONEY TREE"];
            foreach (var j in vouches)
            {
                challengeDef.StartingVouchers.Add(() => VoucherDb.MakeVoucherCard(j));
            }

            challengeDef.CustomRulesJokerBuilder = c =>
            {
                var ret = JokerDb.BasicDataBlock("Rich get richer challenge", "Gain $100 starting money. Chips cannot exceed current $");

                ret.OnJokerGainEffs.Add(() => Globals.EmitMoneyGain(100 - Globals.Money, c));

                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GainEmit,
                    MyAction = args =>
                    {
                        if (args is EngineChipsMultGainEmitArgs gArgs && gArgs.ChipsGainEmitted > 0)
                        {
                            //Cases:
                            //Already greater than or eq to Chips, cancel gain.
                            if(Globals.CurrentChips >= Globals.Money)
                            {
                                gArgs.ChipsGainEmitted = 0;
                            }
                            //The gain amount added to Current chips is greq Money, make the gain amount the difference, so we go to but don't exceed money amount.
                            else if(Globals.CurrentChips + gArgs.ChipsGainEmitted >= Globals.Money)
                            {
                                var diff = Globals.Money - Globals.CurrentChips;
                                gArgs.ChipsGainEmitted = diff;
                            }
                        }
                    }
                });

                return ret;
            };

            return challengeDef;
        } },
        {"ON A KNIFES EDGE", () =>
        {
            var challengeDef = new ChallengeDefinition
            {
                Id = "ON A KNIFES EDGE",
                Name = "On a Knife's Edge",
                ChallengeIndex = 4,
                Description = "Eternal, pinned Ceremonial Dagger."
            };

            challengeDef.StartingJokers.Add(() =>
            {
                var ret = JokerDb.GenerateJokerCard("CEREMONIAL DAGGER");
                ret.AddSticker(Sticker.ETERNAL);
                ret.Pinned = true;
                return ret;
            });

            return challengeDef;
        } },
        {"X RAY VISION", () =>
        {
            var ret = new ChallengeDefinition
            {
                Id = "X RAY VISION",
                Name = "X-ray Vision",
                ChallengeIndex = 5,
                Description = "1 in 4 cards drawn face down."
            };
            
            ret.CustomRulesJokerBuilder = c =>
            {
                var retJok = JokerDb.BasicDataBlock("X-ray vision challenge", "1 in 4 cards drawn face down.");
                retJok.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.CardDrawnToZone,
                    MyAction = args =>
                    {
                        if(args is EngineCardDrawnToZoneArgs drawArgs && drawArgs.ZoneDrawnTo == ZoneManager.HandZone && Globals.RollRandom(1, 4, c) && drawArgs.CardBeingDrawn != null)
                        {
                            drawArgs.CardBeingDrawn.FaceDown = true;
                        }
                    },
                });

                return retJok;
            };
            return ret;
        } },
        {"MAD WORLD", () =>
        {
            var challengeDef = new ChallengeDefinition
            {
                Id = "MAD WORLD",
                Name = "Mad World",
                ChallengeIndex = 6,
                Description = "Extra hands no longer earn money, earn no Interest at end of round. Eternal negative Pareidolia, Eternal Business Card. Only ranks 2 through 9 in deck."
            };

            challengeDef.StartingJokers.Add(() =>
            {
                var ret = JokerDb.GenerateJokerCard("PAREIDOLIA");
                ret.AddSticker(Sticker.ETERNAL);
                ret.SetEditionOfficial(Edition.NEGATIVE);
                return ret;
            });
            challengeDef.StartingJokers.Add(() =>
            {
                var ret = JokerDb.GenerateJokerCard("BUSINESS CARD");
                ret.AddSticker(Sticker.ETERNAL);
                return ret;
            });

            challengeDef.ModifyStartingDeck = cz =>
            {
                //Remove Aces, Twos, and Threes.
                List<Rank> toKeepRanks = [Rank.TWO, Rank.THREE, Rank.FOUR, Rank.FIVE, Rank.SIX, Rank.SEVEN, Rank.EIGHT, Rank.NINE];
                var toRem = new List<Card>();
                toRem.AddRange(cz.Cards.Where(c => !toKeepRanks.Contains(c.Rank)));
                foreach (var cr in toRem)
                {
                    cz.RemoveCard(cr);
                }
            };

            challengeDef.CustomRulesJokerBuilder = c =>
            {
                var ret = JokerDb.BasicDataBlock("Mad World challenge", "Extra hands no longer earn money. Earn no Interest at end of round.");
                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GatherPostRoundMoney,
                    MyAction = args =>
                    {
                        if (args is EngineGatherPostRoundMoneyArgs mArgs)
                        {
                            mArgs.ExistingSources.RemoveAll(x => x.Item1 == "Interest");
                            mArgs.ExistingSources.RemoveAll(x => x.Item1 == "Hands Remaining");
                        }
                    }
                });

                return ret;
            };

            challengeDef.BannedBossBlinds.Add("THE PLANT");

            return challengeDef;
        } },
        {"LUXURY TAX", () =>
        {
            var challengeDef = new ChallengeDefinition
            {
                Id = "LUXURY TAX",
                Name = "Luxury Tax",
                ChallengeIndex = 7,
                Description = "Hold -1 cards in hand for every $5 you have. Start with 10 hand size."
            };

            challengeDef.CustomRulesJokerBuilder = c =>
            {
                var ret = JokerDb.BasicDataBlock("Luxury Tax challenge");

                Func<int> getHandReductionAmt = () => Globals.Money / 5;
                ret.DataDict.Add("CUR_HAND_REDUCTION", new JokerData() {MyDataType = JokerDataType.INT, IntData = 0});

                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.MoneyGainEmit,
                    MyAction = args =>
                    {
                        if (args is EngineGoldGainEmitArgs mArgs)
                        {
                            var curSize = getHandReductionAmt();
                            var diff = ret.DataDict["CUR_HAND_REDUCTION"].IntData - curSize;//if new reduction is smaller than old reduction, this num is positive, diff should be added to handsize.
                            Globals.HandSize += diff;
                            ret.DataDict["CUR_HAND_REDUCTION"].IntData = curSize;
                        }
                    }
                });
                ret.OnJokerGainEffs.Add(() =>
                {
                    ret.DataDict["CUR_HAND_REDUCTION"].IntData = getHandReductionAmt();
                    var totalHandSize = 10 - ret.DataDict["CUR_HAND_REDUCTION"].IntData;
                    Globals.HandSize = totalHandSize;
                });

                return ret;
            };

            return challengeDef;
        } },
    };

    public static IReadOnlyList<ChallengeDefinition> All => Definitions.Values.ToList();
    public static IReadOnlyCollection<string> DefaultUnlockedChallengeIds { get; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "THE OMELETTE", "15 MINUTE CITY" };
    public static IReadOnlyCollection<string> UnlockedChallengeIds =>
        Definitions.Keys.Where(UnlockManager.IsChallengeUnlocked).OrderBy(x => x).ToList();
    public static ChallengeDefinition? CurrentChallenge { get; private set; }

    static ChallengeManager()
    {
        //Register all challenges.
        //TODO: Later, undo this register thing? Make it work same as other DBs?
        foreach (var v in ChallengeDb.Values)
        {
            var def = v();
            Register(def);
        }
    }

    public static void Register(ChallengeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (string.IsNullOrWhiteSpace(definition.Id))
            throw new ArgumentException("A challenge must have an ID.", nameof(definition));
        Definitions.Add(definition.Id, definition);
    }

    public static bool TryGet(string id, out ChallengeDefinition definition) =>
        Definitions.TryGetValue(id, out definition!);

    public static bool IsUnlocked(string id) => UnlockManager.IsChallengeUnlocked(id);

    public static bool Unlock(string id, bool saveImmediately = true) =>
        UnlockManager.UnlockChallenge(id, saveImmediately);

    public static bool IsBeaten(string id) => UnlockManager.IsChallengeBeaten(id);

    public static void Begin(ChallengeDefinition definition)
    {
        CurrentChallenge = definition;
        definition.Apply();
    }

    public static void Clear() => CurrentChallenge = null;
}
