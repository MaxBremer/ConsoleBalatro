using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards.Decks
{
    public static class DeckDb
    {
        /// <summary>
        /// List of established deck DB names.
        /// </summary>
        public static List<string> DeckDBNames => DeckData.Keys.ToList();

        /// <summary>
        /// List of deck DB names of decks that are unlocked by default, at the start of the game.
        /// </summary>
        public static IReadOnlyCollection<string> DefaultUnlockedDeckNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RED" };

        /// <summary>
        /// List of deck DB names of decks that have been unlocked.
        /// </summary>
        public static List<string> UnlockedDeckDBNames => DeckDBNames.Where(UnlockManager.IsDeckUnlocked).ToList();

        /// <summary>
        /// Full DB of decks, alongside their builder function.
        /// </summary>
        public static Dictionary<string, Func<Card, JokerCardDataBlock>> DeckData = new()
        {
            {
                "RED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Red", "+1 discard every round");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxDiscardsPerRound += 1;
                    });

                    return ret;
                }
            },
            {
                "BLUE",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Blue", "+1 hand every round");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound += 1;
                    });

                    return ret;
                }
            },
            {
                "YELLOW",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Yellow", "Start with extra $10");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.EmitMoneyGain(10, c);
                    });

                    return ret;
                }
            },
            {
                "GREEN",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Green", "At end of each Round: $2 per remaining Hand, $1 per remaining Discard, no interest.");
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.GatherPostRoundMoney,
                        MyAction = args =>
                        {
                            if(args is EngineGatherPostRoundMoneyArgs mArgs)
                            {
                                mArgs.ExistingSources.RemoveAll(x => x.Item1 == "Interest");
                                var oldHandsAmt = mArgs.ExistingSources.FirstOrDefault(x => x.Item1 == "Hands Remaining").Item2;
                                mArgs.ExistingSources.RemoveAll(x => x.Item1 == "Hands Remaining");
                                mArgs.ExistingSources.Add(("Hands Remaining", oldHandsAmt * 2));
                                mArgs.ExistingSources.Add(("Discards Remaining", Globals.CurDiscardsRemaining));
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "BLACK",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Black", "+1 Joker slot, -1 hand every round");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.MaxHandsPerRound -= 1;
                        ZoneManager.JokerZone.MaxCapacity += 1;
                    });

                    return ret;
                }
            },
            {
                "MAGIC",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Magic", "Start run with the Crystal Ball voucher and 2 copies of The Fool");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("CRYSTAL BALL"));
                        for (int i = 0; i < 2; i++)
                            ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard("FOOL"));
                    });

                    return ret;
                }
            },
            {
                "NEBULA",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Nebula", "Start run with the Telescope voucher, -1 consumable slot");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.ConsumableZone.MaxCapacity -= 1;
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("TELESCOPE"));
                    });

                    return ret;
                }
            },
            {
                "GHOST",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Ghost", "Spectral cards may appear in the shop, start with a Hex card.");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeSpectralCard("HEX"));
                    });
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.MarketTypeBeingChosen,
                        MyAction = args =>
                        {
                            if(args is EngineMarketTypeBeingChosenArgs mArgs && !mArgs.WeightsBeingRolled.ContainsKey(BuyItemType.SPECTRAL_CARD))
                            {
                                mArgs.WeightsBeingRolled.Add(BuyItemType.SPECTRAL_CARD, 2);
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "ABANDONED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Abandoned", "Start run with no Face Cards in your deck.");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.DeckZone.Cards.RemoveAll(c => EngineUtils.isFace(c));
                    });

                    return ret;
                }
            },
            {
                "CHECKERED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Checkered", "Start run with 26 Spades and 26 Hearts in your deck");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        foreach (var c in ZoneManager.DeckZone.Cards)
                        {
                            if(c.Suit == Enums.Suit.DIAMONDS)
                            {
                                c.Suit = Enums.Suit.HEARTS;
                            }else if(c.Suit == Enums.Suit.CLUBS)
                            {
                                c.Suit = Enums.Suit.SPADES;
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "ZODIAC",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Zodiac", "Start run with Tarot Merchant, Planet Merchant, and Overstock.");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("TAROT MERCHANT"));
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("PLANET MERCHANT"));
                        ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard("OVERSTOCK"));
                    });

                    return ret;
                }
            },
            {
                "PAINTED",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Painted", "+2 hand size, -1 Joker slot");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        Globals.HandSize += 2;
                        ZoneManager.JokerZone.MaxCapacity -= 1;
                    });

                    return ret;
                }
            },
            {
                "ANAGLYPH",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Anaglyph", "After defeating each Boss Blind, gain a Double Tag");
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.EndPlayRound,
                        MyAction = args =>
                        {
                            if(FlowHandler.CurrentSelectedBlind == BlindType.BOSS)
                            {
                                TagDb.AddTagOfType(TagType.DOUBLE_TAG);
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "PLASMA",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Plasma", "Balance Chips and Mult when calculating score for played hand, X2 base Blind size");
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.PreFinalGainCheck,
                        MyAction = args =>
                        {
                            if(args is EnginePreFinalGainArgs preCalcArgs)
                            {
                                var splitVal = (preCalcArgs.FinalChips + preCalcArgs.FinalMult) / 2;
                                preCalcArgs.FinalChips = Globals.CapChipCount(splitVal);
                                preCalcArgs.FinalMult = splitVal;
                            }
                        }
                    });
                    ret.Listeners.Add(new EngineEventListener()
                    {
                        MyContextType = EventContextType.GetBlindChips,
                        MyAction = args =>
                        {
                            if(args is EngineGetBlindReqArgs blindArgs)
                            {
                                blindArgs.ChipRequirementAmount = Globals.CapChipCount(blindArgs.ChipRequirementAmount * 2);
                            }
                        }
                    });

                    return ret;
                }
            },
            {
                "ERRATIC",
                c =>
                {
                    var ret = JokerDb.BasicDataBlock("Erratic", "All Ranks and Suits in deck are randomized");
                    ret.OnJokerGainEffs.Add(() =>
                    {
                        foreach (var c in ZoneManager.DeckZone.Cards)
                        {
                            EngineUtils.RandomizePlayingCard(c);
	                    }
                    });

                    return ret;
                }
            },
        };

        /// <summary>
        /// Returns whether the passed deck has been unlocked.
        /// </summary>
        /// <param name="deckDbName">The deck to be checked.</param>
        /// <returns>A boolean value indicating whether the passed deck is unlocked.</returns>
        public static bool IsDeckUnlocked(string deckDbName) => UnlockManager.IsDeckUnlocked(deckDbName);

        /// <summary>
        /// Unlock the passed deck.
        /// </summary>
        /// <param name="deckDbName">DB name of the deck to be unlocked.</param>
        /// <param name="saveImmediately">A boolean value indicating whether to save the unlock immediately.</param>
        /// <returns>A boolean value indicating whether the deck was unlocked.</returns>
        public static bool UnlockDeck(string deckDbName, bool saveImmediately = true) => UnlockManager.UnlockDeck(deckDbName, saveImmediately);

        /// <summary>
        /// Sets the current deck of the run, "activating" it by applying its effect(s)/listener(s).
        /// </summary>
        /// <param name="deckDbName">The deck to activate.</param>
        public static void BecomeDeck(string deckDbName)
        {
            if (!DeckData.ContainsKey(deckDbName))
                return;
            var c = new Card();
            var jokerData = DeckData[deckDbName](c);
            c.JokerData = jokerData;
            ZoneManager.AddHiddenEffect(c);
        }
    }
}
