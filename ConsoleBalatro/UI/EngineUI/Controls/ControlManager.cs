using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Market;
using ConsoleBalatro.Engine.Stakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.Controls
{
    public static class ControlManager
    {
        private static List<ConsoleKey> numKeyList = new List<ConsoleKey>() { ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9, ConsoleKey.D0 };

        public static Dictionary<string, ControlOptionset> AvailableControlSets = new();

        public static ControlOptionset CurrentOptions;
        public static ControlContext CurrentContext;

        public static string CurrentControlset = "ROUND";

        public static void InitializeControls()
        {
            AvailableControlSets.Add("ROUND", BuildPlayRoundOptions());
            AvailableControlSets.Add("MARKET", BuildMarketRoundOptions());
            AvailableControlSets.Add("PACK", BuildPackOptionSelectionOptions());
            AvailableControlSets.Add("POSTROUND", BuildPostRoundOptions());
            AvailableControlSets.Add("BLIND", BuildBlindOptions());
            AvailableControlSets.Add("GAMEOVER", BuildEmptyOptions());
            AvailableControlSets.Add("DECKCHOICE", BuildDeckSelectOptions());
        }

        public static void EngageControlset(ControlOptionset options, ControlContext context)
        {
            var inp = ReadKey();
            if (options.AvailableActions.ContainsKey(inp.Key))
            {
                options.AvailableActions[inp.Key](context);
            }
        }

        public static void EngageCurrentControlset(ControlContext context)
        {
            var curSet = AvailableControlSets[CurrentControlset];
            EngageControlset(curSet, context);
        }

        public static Card KeySelectCardFromZone(CardZone zone)
        {
            //null represents a cancelled select.
            //Requires all cards in the zone to exist as CardDisplays.
            var dispList = new Dictionary<int, (CardDisplay, Card)>();
            int curKey = 1;
            foreach (var c in zone.Cards)
            {
                if (EngineDisplayGlobals.GlobalCardDisplays.ContainsKey(c))
                {
                    var targetDisp = EngineDisplayGlobals.GlobalCardDisplays[c];
                    dispList.Add(curKey, (targetDisp, c));
                    targetDisp.CardSelectNumber = curKey;
                    targetDisp.PreDisplaySetup();
                    curKey += 1;
                }
            }
            EngineDisplayGlobals.Redraw();

            var selInp = ReadLine();
            Card toRet = null;
            if(Int32.TryParse(selInp, out int result) && dispList.ContainsKey(result))
            {
                toRet = dispList[result].Item2;
            }
            foreach(var k in dispList.Keys)
            {
                dispList[k].Item1.CardSelectNumber = -1;
                dispList[k].Item1.PreDisplaySetup();
            }
            EngineDisplayGlobals.Redraw();
            return toRet;
        }

        public static string ReadLine()
        {
            return Console.ReadLine();
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public static void ClearConsole()
        {
            Console.Clear();
        }

        private static void AddStandardControls(ControlOptionset ret, LookZonesAvailable lookZones)
        {
            ret.AvailableActions.Add(ConsoleKey.L, BuildLookAction(lookZones));
            ret.ZonesAvailable = lookZones;

            ret.AvailableActions.Add(EzLook.EZ_LOOK_KEY, _ =>
            {
                EzLook.EngageEzLook(ret);
            });

            ret.AvailableActions.Add(ConsoleKey.T, _ =>
            {
                EngineDisplayGlobals.HandStatsMenu.Visible = true;
                EngineDisplayGlobals.Redraw();
                var x = ReadKey();
                EngineDisplayGlobals.HandStatsMenu.Visible = false;
                EngineDisplayGlobals.Redraw();
            });

            if (Globals.ALLOW_DEBUG_COMMANDS)
            {
                ret.AvailableActions.Add(ConsoleKey.Oem3, _ =>
                {
                    ClearConsole();
                    DebugManager.RunDebugCmdLine();
                    EngineDisplayGlobals.Redraw();
                });
            }
        }

        private static ControlOptionset BuildPlayRoundOptions()
        {
            var ret = new ControlOptionset();
            ret.SchemaName = "PlayRound";

            var lookZones = new LookZonesAvailable()
            {
                HandAvailable = true,
                JokersAvailable = true,
                ConsumablesAvailable = true,
            };
            AddStandardControls(ret, lookZones);

            ret.AvailableActions.Add(ConsoleKey.Backspace, _ =>
            {
                if (Globals.CanDiscard)
                    Globals.DiscardSelectedFromHand();
            });
            ret.AvailableActions.Add(ConsoleKey.Enter, _ =>
            {
                Globals.PlayCurrentlySelectedHand();
            });
            ret.AvailableActions.Add(ConsoleKey.S, _ =>
            {
                ZoneManager.SortZoneBySuit(ZoneManager.HandZone);
                EngineDisplayGlobals.HandDisplay.ResetFromZoneList();
                EngineDisplayGlobals.Redraw();
            });

            ret.AvailableActions.Add(ConsoleKey.R, _ =>
            {
                ZoneManager.SortZoneByRank(ZoneManager.HandZone);
                EngineDisplayGlobals.HandDisplay.ResetFromZoneList();
                EngineDisplayGlobals.Redraw();
            });

            
            ret.AvailableActions.Add(ConsoleKey.D, _ =>
            {
                if (FlowHandler.CurrentSelectedBlind != BlindType.BOSS || ZoneManager.HiddenBlindAttributeZone.Cards.Count == 0)
                    return;
                EngineDisplayGlobals.ShowInfoDisplay(FlowHandler.CurrentBossBlind + "&!&!" + ZoneManager.HiddenBlindAttributeZone.Cards[0].JokerData.DescriptionBuilder(null) + "&!&!(PRESS ANY KEY TO CLOSE)", "&!");
                EngineDisplayGlobals.Redraw();
                var x = ReadKey();
                EngineDisplayGlobals.HideInfoDisplay();
                EngineDisplayGlobals.Redraw();
            });
            

            foreach (var n in numKeyList)
            {
                ret.AvailableActions.Add(n, BuildActionForNumKey(n));
            }

            return ret;
        }

        private static ControlOptionset BuildMarketRoundOptions()
        {
            var ret = new ControlOptionset();
            ret.SchemaName = "Market";

            var lookZones = new LookZonesAvailable()
            {
                JokersAvailable = true,
                ConsumablesAvailable = true,
                PackMarketAvailable = true,
                MainMarketAvailable = true,
                VoucherMarketAvailable = true,
            };
            AddStandardControls(ret, lookZones);

            ret.AvailableActions.Add(ConsoleKey.R, _ =>
            {
                MarketGeneralManager.RerollMainMarket();
            });
            if (Globals.ALLOW_DEBUG_COMMANDS)
            {
                ret.AvailableActions.Add(ConsoleKey.D, _ =>
                {
                    MarketGeneralManager.DebugRefreshPackMarket();
                });
            }
            ret.AvailableActions.Add(ConsoleKey.E, _ =>
            {
                FlowHandler.CloseMarketRound();
            });

            return ret;
        }

        private static ControlOptionset BuildPackOptionSelectionOptions()
        {
            var ret = new ControlOptionset();
            ret.SchemaName = "PackOptionSelection";

            var lookZones = new LookZonesAvailable()
            {
                JokersAvailable = true,
                ConsumablesAvailable = true,
                HandAvailable = true,
                PackOptionsAvailable = true,
            };
            AddStandardControls(ret, lookZones);

            foreach (var n in numKeyList)
            {
                ret.AvailableActions.Add(n, BuildActionForNumKey(n));
            }
            ret.AvailableActions.Add(ConsoleKey.S, _ =>
            {
                PackActions.SkipCurrentPack();
            });

            return ret;
        }

        private static ControlOptionset BuildPostRoundOptions()
        {
            var ret = new ControlOptionset();
            ret.SchemaName = "PostRoundScreen";

            var lookZones = new LookZonesAvailable()
            {
                JokersAvailable = true,
                ConsumablesAvailable = true,
            };
            AddStandardControls(ret, lookZones);


            ret.AvailableActions.Add(ConsoleKey.C, _ =>
            {
                FlowHandler.ClosePostRound();
            });

            return ret;
        }

        private static ControlOptionset BuildBlindOptions()
        {
            var ret = new ControlOptionset();
            ret.SchemaName = "BlindSelection";

            var lookZones = new LookZonesAvailable()
            {
                JokersAvailable = true,
                ConsumablesAvailable = true,
            };
            AddStandardControls(ret, lookZones);

            ret.AvailableActions.Add(ConsoleKey.S, _ =>
            {
                FlowHandler.DoSkip();
            });

            ret.AvailableActions.Add(ConsoleKey.B, _ =>
            {
                FlowHandler.StartSelectedBlind();
            });

            if (Globals.ALLOW_DEBUG_COMMANDS)
            {
                ret.AvailableActions.Add(ConsoleKey.D, _ =>
                {
                    FlowHandler.RerollBossBlind(isPlayerReroll: false);
                    EngineDisplayGlobals.Redraw();
                });
            }

            ret.AvailableActions.Add(ConsoleKey.R, _ =>
            {
                if (Globals.CanRerollBossBlind)
                {
                    FlowHandler.RerollBossBlind(isPlayerReroll: true);
                    EngineDisplayGlobals.Redraw();
                }
            });

            return ret;
        }

        private static ControlOptionset BuildDeckSelectOptions()
        {
            var ret = new ControlOptionset
            {
                SchemaName = "DeckSelection"
            };

            ret.AvailableActions.Add(ConsoleKey.LeftArrow, _ =>
            {
                EngineDisplayGlobals.DeckChoiceMenu.SelectPreviousDeck();
                EngineDisplayGlobals.Redraw();
            });

            ret.AvailableActions.Add(ConsoleKey.RightArrow, _ =>
            {
                EngineDisplayGlobals.DeckChoiceMenu.SelectNextDeck();
                EngineDisplayGlobals.Redraw();
            });

            ret.AvailableActions.Add(ConsoleKey.UpArrow, _ =>
            {
                EngineDisplayGlobals.DeckChoiceMenu.SelectNextStake();
                EngineDisplayGlobals.Redraw();
            });

            ret.AvailableActions.Add(ConsoleKey.DownArrow, _ =>
            {
                EngineDisplayGlobals.DeckChoiceMenu.SelectPreviousStake();
                EngineDisplayGlobals.Redraw();
            });

            ret.AvailableActions.Add(ConsoleKey.Enter, _ =>
            {
                if (EngineDisplayGlobals.DeckChoiceMenu.CanSelectCurrentDeck && EngineDisplayGlobals.DeckChoiceMenu.CanSelectCurrentStake)
                    FlowHandler.DeckChosen(EngineDisplayGlobals.DeckChoiceMenu.SelectedDeckName, (StakeType)EngineDisplayGlobals.DeckChoiceMenu.SelectedStakeIndex);
            });

            return ret;
        }

        private static ControlOptionset BuildEmptyOptions()
        {
            var ret = new ControlOptionset
            {
                SchemaName = "TEST OPTIONS IGNORE"
            };
            return ret;
        }

        private static ControlOptionset GetAvailableCardActions(Card c)
        {
            var ret = new ControlOptionset();

            //everyone gets view detail
            ret.AvailableActions.Add(ConsoleKey.D, context =>
            {
                EngineDisplayGlobals.DisplayDetailInfoForCard(c);
                EngineDisplayGlobals.Redraw();
                var _ = ReadKey();
            });

            //Everyone besides market zones gets move.
            if(!(ZoneManager.MainMarketZone.Cards.Contains(c) || ZoneManager.PackMarketZone.Cards.Contains(c) || ZoneManager.VoucherMarketZone.Cards.Contains(c)))
            {
                ret.AvailableActions.Add(ConsoleKey.M, context =>
                {
                    var targetZone = c.MyZone;
                    var secondCardForSwap = KeySelectCardFromZone(targetZone);
                    if (c == secondCardForSwap)
                        return;

                    targetZone.SwapCardPositions(c, secondCardForSwap);

                    EngineDisplayGlobals.Redraw();
                });
            }

            //Joker and Consumable zones get Sell
            if((ZoneManager.JokerZone.Cards.Contains(c) || ZoneManager.ConsumableZone.Cards.Contains(c)) && Globals.CanBeSold(c))
            {
                ret.AvailableActions.Add(ConsoleKey.S, context =>
                {
                    Globals.PerformSell(c, c.MyZone);
                });
            }

            //Consumables that are activatable
            //TODO: Yeah the stupid activatable args.
            if(c.isConsumable && ZoneManager.ConsumableZone.Cards.Contains(c) && c.ConsumableData.IsActivatable(null))
            {
                ret.AvailableActions.Add(ConsoleKey.A, context =>
                {
                    ConsumableManager.UseConsumable(c, c.MyZone);
                });
            }

            //Pack option selection
            if(ZoneManager.PackOptionZone.Cards.Contains(c) && PackActions.CanAcceptPackOption(c))
            {
                ret.AvailableActions.Add(ConsoleKey.C, context =>
                {
                    PackActions.PackOptionSelectionMade(c);
                });
            }

            //For now, only offer toggle select for hand cards. Later, prob all.
            if(c.MyZone == ZoneManager.HandZone)
            {
                ret.AvailableActions.Add(ConsoleKey.T, context =>
                {
                    c.ToggleSelect();
                });
            }

            //Purchase
            if((c.MyZone == ZoneManager.MainMarketZone || c.MyZone == ZoneManager.PackMarketZone || c.MyZone == ZoneManager.VoucherMarketZone) && Globals.CanBePurchased(c))
            {
                ret.AvailableActions.Add(ConsoleKey.B, context =>
                {
                    Globals.PerformPurchaseByType(c);
                });
            }

            //Buy and use instantly
            if(c.MyZone == ZoneManager.MainMarketZone && Globals.CanBuyAndUse(c))
            {
                ret.AvailableActions.Add(ConsoleKey.U, context =>
                {
                    Globals.PerformBuyAndUse(c);
                });
            }

            //everyone gets cancel
            ret.AvailableActions.Add(ConsoleKey.E, context => { });

            return ret;
        }

        private static string GetStringOfAvailableActions(Card c, string lineDivider)
        {
            //everyone gets view detail
            var ret = "[D]etail" + lineDivider;

            //Everyone besides market zones gets move.
            if (!(ZoneManager.MainMarketZone.Cards.Contains(c) || ZoneManager.PackMarketZone.Cards.Contains(c) || ZoneManager.VoucherMarketZone.Cards.Contains(c)))
            {
                ret += "[M]ove card" + lineDivider;
            }

            //Joker and Consumable zones get Sell
            if ((ZoneManager.JokerZone.Cards.Contains(c) || ZoneManager.ConsumableZone.Cards.Contains(c)) && Globals.CanBeSold(c))
            {
                ret += "[S]ell (" + c.SellCost + ")" + lineDivider;
            }

            //Consumables that are activatable
            //TODO: Yeah the stupid activatable args.
            if (c.isConsumable && ZoneManager.ConsumableZone.Cards.Contains(c) && c.ConsumableData.IsActivatable(null))
            {
                ret += "[A]ctivate" + lineDivider;
            }

            //Pack option selection
            if (ZoneManager.PackOptionZone.Cards.Contains(c) && PackActions.CanAcceptPackOption(c))
            {
                ret += "[C]hoose" + lineDivider;
            }

            //For now, only offer toggle select for hand cards. Later, prob all.
            if (c.MyZone == ZoneManager.HandZone)
            {
                ret += "[T]oggle select" + lineDivider;
            }

            //Purchase
            if ((c.MyZone == ZoneManager.MainMarketZone || c.MyZone == ZoneManager.PackMarketZone || c.MyZone == ZoneManager.VoucherMarketZone) && Globals.CanBePurchased(c))
            {
                ret += "[B]uy card" + lineDivider;
            }

            //Buy and use instantly
            if (c.MyZone == ZoneManager.MainMarketZone && Globals.CanBuyAndUse(c))
            {
                ret += "Buy and [U]se" + lineDivider;
            }

            //everyone gets cancel
            ret += "Canc[E]l" + lineDivider;

            return ret;
        }

        public class LookZonesAvailable
        {
            public bool HandAvailable = false;
            public bool JokersAvailable = false;
            public bool ConsumablesAvailable = false;
            public bool BeingPlayedAvailable = false;
            public bool PackMarketAvailable = false;
            public bool MainMarketAvailable = false;
            public bool VoucherMarketAvailable = false;

            public bool PackOptionsAvailable = false;
        }

        private static Action<ControlContext> BuildLookAction(LookZonesAvailable zonesAvailable)
        {
            //TODO: move this out into its own function? would make sense.
            Func<CardZone> SelectCardZone = () =>
            {
                Dictionary<ConsoleKey, CardZone> OptsToPickFrom = new();

                //TODO: MORE BAD REPEATED CODE. ME ANGYY.
                if(zonesAvailable.HandAvailable && ZoneManager.HandZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.H, ZoneManager.HandZone);
                    EngineDisplayGlobals.HandDisplay.DisplayBeneath = "H";
                }
                if (zonesAvailable.JokersAvailable && ZoneManager.JokerZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.J, ZoneManager.JokerZone);
                    EngineDisplayGlobals.JokersDisplay.DisplayBeneath = "J";
                }
                if (zonesAvailable.ConsumablesAvailable && ZoneManager.ConsumableZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.C, ZoneManager.ConsumableZone);
                    EngineDisplayGlobals.ConsumableDisplay.DisplayBeneath = "C";
                }
                if (zonesAvailable.BeingPlayedAvailable && ZoneManager.CurrentlyBeingPlayedZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.L, ZoneManager.CurrentlyBeingPlayedZone);
                    EngineDisplayGlobals.BeingPlayedDisplay.DisplayBeneath = "L";
                }
                if (zonesAvailable.PackMarketAvailable && ZoneManager.PackMarketZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.P, ZoneManager.PackMarketZone);
                    EngineDisplayGlobals.PackMarketDisplay.DisplayBeneath = "P";
                }
                if (zonesAvailable.MainMarketAvailable && ZoneManager.MainMarketZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.M, ZoneManager.MainMarketZone);
                    EngineDisplayGlobals.MainMarketDisplay.DisplayBeneath = "M";
                }
                if (zonesAvailable.VoucherMarketAvailable && ZoneManager.VoucherMarketZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.V, ZoneManager.VoucherMarketZone);
                    EngineDisplayGlobals.VoucherMarketDisplay.DisplayBeneath = "V";
                }
                if (zonesAvailable.PackOptionsAvailable && ZoneManager.PackOptionZone.Cards.Count > 0)
                {
                    OptsToPickFrom.Add(ConsoleKey.O, ZoneManager.PackOptionZone);
                    EngineDisplayGlobals.PackOptionsDisplay.DisplayBeneath = "O";
                }
                if(OptsToPickFrom.Count == 0)
                {
                    return null;
                }
                EngineDisplayGlobals.Redraw();
                ConsoleKey sel = ReadKey().Key;
                EngineDisplayGlobals.ClearDisplayBeneathChars();
                EngineDisplayGlobals.Redraw();
                if (!OptsToPickFrom.ContainsKey(sel))
                {
                    return null;
                }
                else
                {
                    return OptsToPickFrom[sel];
                }
            };

            return context =>
            {
                var cz = SelectCardZone();
                if (cz == null)
                {
                    EngineDisplayGlobals.Redraw();
                    return;
                }
                Card targetCard = null;
                if(cz.Cards.Count == 0)
                {
                    return;
                }else if(cz.Cards.Count == 1)
                {
                    targetCard = cz.Cards[0];
                }
                else
                {
                    targetCard = KeySelectCardFromZone(cz);
                }

                if(targetCard == null)
                {
                    EngineDisplayGlobals.Redraw();
                    return;
                }

                //CardSelectedOptions(targetCard);
                var showStr = GetStringOfAvailableActions(targetCard, "&");
                EngineDisplayGlobals.ShowInfoDisplay(showStr, "&");
                EngineDisplayGlobals.Redraw();
                EngageControlset(GetAvailableCardActions(targetCard), null);
                EngineDisplayGlobals.HideInfoDisplay();
                EngineDisplayGlobals.Redraw();
            };
        }

        private static Action<ControlContext> BuildActionForNumKey(ConsoleKey numKey)
        {
            if (numKeyList.Contains(numKey))
            {
                return _ =>
                {
                    var targetInd = numKeyList.IndexOf(numKey);
                    if (ZoneManager.HandZone.Cards.Count > targetInd)
                        ZoneManager.HandZone.Cards[targetInd].ToggleSelect();
                };
            }
            return null;
        }
    }
}
