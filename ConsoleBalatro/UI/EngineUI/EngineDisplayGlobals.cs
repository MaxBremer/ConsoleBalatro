using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.UI.EngineUI.Animation;
using ConsoleBalatro.UI.EngineUI.Controls;
using ConsoleBalatro.UI.EngineUI.MarketPanels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public static class EngineDisplayGlobals
    {
        public const bool OVERRIDE_ANIMATIONS = true;

        public static Dictionary<Edition, string> EditionBorderChars = new()
        {
            {Edition.FOIL, "^" },
            {Edition.HOLOGRAPHIC, "w" },
            {Edition.NEGATIVE, "_" },
            {Edition.POLYCHROME, "z" },

        };

        public static Dictionary<Enhancement, Func<string, string>> EnhancementModifiers = new()
        {
            { Enhancement.MULT, x => setCharsAt(x, 19, "MMM") },
            { Enhancement.BONUSCHIPS, x => setCharsAt(x, 19, "^^^") },
            { Enhancement.LUCKY, x => setCharsAt(x, 19, "LLL") },
            { Enhancement.WILD, x => setCharAt(x, 15, '*') },
            { Enhancement.GOLD, x => setCharsAt(x, 19, "GGG") },
            { Enhancement.GLASS, x => setCharsAt(x, 19, "GLA") },
            { Enhancement.STEEL, x => setCharsAt(x, 19, "STE") },
            { Enhancement.STONE, x => setCharsAt(x, 13, "STO") },
        };

        public static Dictionary<Seal, Func<string, string>> SealModifiers = new()
        {
            { Seal.RED, x => setCharAt(x, 9, 'r') },
            { Seal.BLUE, x => setCharAt(x, 9, 'b') },
            { Seal.PURPLE, x => setCharAt(x, 9, 'p') },
            { Seal.GOLD, x => setCharAt(x, 9, 'g') },
        };

        public static Dictionary<Sticker, Func<string, string>> StickerModifiers = new()
        {
            { Sticker.ETERNAL, x => setCharAt(x, 10, 'E') },
            { Sticker.PERISHABLE, x => setCharAt(x, 10, 'P') },
            { Sticker.RENTAL, x => setCharAt(x, 10, 'R') },
        };

        public static Dictionary<Card, CardDisplay> GlobalCardDisplays = new();
        public static CardZoneBeingPlayedDisplay BeingPlayedDisplay;
        public static CardZoneJokersDisplay JokersDisplay;
        public static CardZoneHandDisplay HandDisplay;
        public static CardZoneConsumableDisplay ConsumableDisplay;

        public static CardZoneDisplay MainMarketDisplay;
        public static CardZoneDisplay PackMarketDisplay;
        public static CardZoneDisplay VoucherMarketDisplay;

        public static CardZoneDisplay PackOptionsDisplay;

        public static ScoreDisplay ScoreDisplay;

        public static MarketSideDisplay MarketSidePanelDisplay;

        public static BlindDisplayEntity FirstBlindPanel;
        public static BlindDisplayEntity SecondBlindPanel;
        public static BlindDisplayEntity ThirdBlindPanel;

        public static int DisplayRequiredChipsForBlind;
        public static int DisplayTotalCurrentChips;

        public static int DisplayHandsRemaining;
        public static int DisplayDiscardsRemaining;

        public static int DisplayMoney = 0;

        public static int DisplayHandChips;
        public static double DisplayHandMult;
        public static PlayedHandType DisplayPlayedHand;

        public static Interface EngineInterface;
        public static List<DisplayEntity> EngineDisplays;

        public static TextDisplayPanel InfoDisplayPanel;
        public static CardDisplay CardBeingViewed = null;

        public static EngineActionAnimation Animation = new EngineActionAnimation();

        private static bool RedrawCached = false;

        public static void InitializeDisplayAll(Interface theGuy)
        {
            EngineInterface = theGuy;

            InitializeGlobalListeners();
            ControlManager.InitializeControls();
            InitializeAllDisplays();
        }

        public static CardZoneDisplay GetZoneDisplayOfCard(Card c)
        {
            //TODO: Stupid idiot fucking way to do it. But what other option do I have?
            //...I guess I could make a dict of zone to display, then iterate thru the keys and check...sigh...more work...
            if (ZoneManager.JokerZone.Cards.Contains(c))
                return JokersDisplay;
            if (ZoneManager.ConsumableZone.Cards.Contains(c))
                return ConsumableDisplay;
            if (ZoneManager.HandZone.Cards.Contains(c))
                return HandDisplay;
            if (ZoneManager.CurrentlyBeingPlayedZone.Cards.Contains(c))
                return BeingPlayedDisplay;
            if (ZoneManager.MainMarketZone.Cards.Contains(c))
                return MainMarketDisplay;
            if (ZoneManager.PackMarketZone.Cards.Contains(c))
                return PackMarketDisplay;
            if (ZoneManager.VoucherMarketZone.Cards.Contains(c))
                return VoucherMarketDisplay;
            if (ZoneManager.PackOptionZone.Cards.Contains(c))
                return PackOptionsDisplay;
            //ZONE TODO: ADD OTHERS
            return null;
        }

        public static void HideInfoDisplay()
        {
            InfoDisplayPanel.Visible = false;
        }

        public static void ShowInfoDisplay(string infoToDisplay, string breakChar)
        {
            InfoDisplayPanel.SetLines(infoToDisplay, breakChar);
            InfoDisplayPanel.AdjustLinesByWrapWidth(EngineDisplayConstants.INFO_PANEL_WIDTH);
            InfoDisplayPanel.Visible = true;
            //Also not this funcs responsibility to redraw.
        }

        public static void DisplayDetailInfoForCardDisplay(CardDisplay cd)
        {
            CardBeingViewed = cd;
            //TODO: EventContext maybe needs to be passed.
            ShowInfoDisplay(cd.MyCard.DetailedInfoDisplay(null), Card.CardInfoLineDivider);
            //TODO: adjust card display to highlight it or something.
        }

        public static void DisplayDetailInfoForCard(Card c)
        {
            if (GlobalCardDisplays.ContainsKey(c))
                DisplayDetailInfoForCardDisplay(GlobalCardDisplays[c]);
        }

        public static void InitializeAllDisplays()
        {
            EngineDisplays = new List<DisplayEntity>();

            JokersDisplay = new CardZoneJokersDisplay(ZoneManager.JokerZone);
            EngineDisplays.Add(JokersDisplay);
            ConsumableDisplay = new CardZoneConsumableDisplay(ZoneManager.ConsumableZone);
            EngineDisplays.Add(ConsumableDisplay);
            BeingPlayedDisplay = new CardZoneBeingPlayedDisplay(ZoneManager.CurrentlyBeingPlayedZone);
            EngineDisplays.Add(BeingPlayedDisplay);
            HandDisplay = new CardZoneHandDisplay(ZoneManager.HandZone);
            EngineDisplays.Add(HandDisplay);
            MainMarketDisplay = new MarketTemplateDisplay(ZoneManager.MainMarketZone, EngineDisplayConstants.MAIN_MARKET_HEIGHT, EngineDisplayConstants.MAIN_MARKET_WIDTH, EngineDisplayConstants.MAIN_MARKET_XPOS, EngineDisplayConstants.MAIN_MARKET_YPOS, false);
            EngineDisplays.Add(MainMarketDisplay);
            VoucherMarketDisplay = new MarketTemplateDisplay(ZoneManager.VoucherMarketZone, EngineDisplayConstants.VOUCH_MARKET_HEIGHT, EngineDisplayConstants.VOUCH_MARKET_WIDTH, EngineDisplayConstants.VOUCH_MARKET_XPOS, EngineDisplayConstants.VOUCH_MARKET_YPOS, false);
            EngineDisplays.Add(VoucherMarketDisplay);
            PackMarketDisplay = new MarketTemplateDisplay(ZoneManager.PackMarketZone, EngineDisplayConstants.PACK_MARKET_HEIGHT, EngineDisplayConstants.PACK_MARKET_WIDTH, EngineDisplayConstants.PACK_MARKET_XPOS, EngineDisplayConstants.PACK_MARKET_YPOS, false);
            EngineDisplays.Add(PackMarketDisplay);
            ScoreDisplay = new ScoreDisplay();
            EngineDisplays.Add(ScoreDisplay);
            InfoDisplayPanel = new TextDisplayPanel(new List<string>() { "INFO" }, EngineDisplayConstants.TEXT_DISPLAY_MINWIDTH, EngineDisplayConstants.TEXT_DISPLAY_MINHEIGHT);
            InfoDisplayPanel.xLoc = EngineDisplayConstants.TEXT_DISPLAY_XLOC;
            EngineDisplays.Add(InfoDisplayPanel);
            PackOptionsDisplay = new CardZoneTemplateDisplay(ZoneManager.PackOptionZone, EngineDisplayConstants.PACK_OPTIONS_HEIGHT, EngineDisplayConstants.PACK_OPTIONS_WIDTH, EngineDisplayConstants.PACK_OPTIONS_XPOS, EngineDisplayConstants.PACK_OPTIONS_YPOS, false);
            EngineDisplays.Add(PackOptionsDisplay);
            MarketSidePanelDisplay = new MarketSideDisplay();
            EngineDisplays.Add(MarketSidePanelDisplay);

            FirstBlindPanel = new BlindDisplayEntity(BlindType.SMALL, EngineDisplayConstants.BLIND_PANEL1_XLOC, EngineDisplayConstants.BLIND_PANEL_YLOC);
            EngineDisplays.Add(FirstBlindPanel);

            SecondBlindPanel = new BlindDisplayEntity(BlindType.BIG, EngineDisplayConstants.BLIND_PANEL2_XLOC, EngineDisplayConstants.BLIND_PANEL_YLOC);
            EngineDisplays.Add(SecondBlindPanel);

            ThirdBlindPanel = new BlindDisplayEntity(BlindType.BOSS, EngineDisplayConstants.BLIND_PANEL3_XLOC, EngineDisplayConstants.BLIND_PANEL_YLOC);
            EngineDisplays.Add(ThirdBlindPanel);

            foreach (var dispEnt in EngineDisplays)
            {
                dispEnt.Visible = false;
                EngineInterface.AddEntity(dispEnt);
            }
        }

        public static void ClearDisplayBeneathChars()
        {
            //TODO: Again...... very stupid. Do better.
            if (HandDisplay != null)
                HandDisplay.DisplayBeneath = "";
            if (JokersDisplay != null)
                JokersDisplay.DisplayBeneath = "";
            if (ConsumableDisplay != null)
                ConsumableDisplay.DisplayBeneath = "";
            if (BeingPlayedDisplay != null)
                BeingPlayedDisplay.DisplayBeneath = "";
            if (MainMarketDisplay != null)
                MainMarketDisplay.DisplayBeneath = "";
            if (VoucherMarketDisplay != null)
                VoucherMarketDisplay.DisplayBeneath = "";
            if (PackMarketDisplay != null)
                PackMarketDisplay.DisplayBeneath = "";
            if (PackOptionsDisplay != null)
                PackOptionsDisplay.DisplayBeneath = "";
        }

        public static void SetupPlayRoundState()
        {
            ClearEngineEntities();

            HandDisplay.Visible = true;
            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            BeingPlayedDisplay.Visible = true;
            ScoreDisplay.Visible = true;
            //ShowInfoDisplay("Num consumables " + ConsumableManager.TarotNames.Count, "&");
        }

        public static void SetupMarketState()
        {
            ClearEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            MarketSidePanelDisplay.Visible = true;
            MarketSidePanelDisplay.SideImDisplaying = "MARKET";
            MainMarketDisplay.Visible = true;
            VoucherMarketDisplay.Visible = true;
            PackMarketDisplay.Visible = true;
        }

        public static void SetupPackOptionSelection()
        {
            ClearEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            MarketSidePanelDisplay.Visible = true;
            MarketSidePanelDisplay.SideImDisplaying = "PACK";

            PackOptionsDisplay.Visible = true;
            HandDisplay.Visible = true;
        }

        public static void SetupPostRoundDisplay(List<(string, int)> moneyData)
        {
            ClearEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;

            var totalMoney = moneyData.Select(x => x.Item2).Sum();

            var breakStr = "@";
            var finalStr = "";
            foreach (var money in moneyData)
            {
                finalStr += money.Item1 + ": " + money.Item2 + breakStr;
            }

            finalStr += breakStr + "TOTAL: " + totalMoney + breakStr;
            finalStr += breakStr + "[C]ontinue";

            ShowInfoDisplay(finalStr, breakStr);
        }

        public static void SetupBlindSelectionState()
        {
            ClearEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            MarketSidePanelDisplay.Visible= true;
            MarketSidePanelDisplay.SideImDisplaying = "BLIND";

            FirstBlindPanel.Visible = true;
            SecondBlindPanel.Visible = true;
            ThirdBlindPanel.Visible = true;
        }

        public static void InitializeGlobalListeners()
        {
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = DisplayChipsMultEmit, MyContextType = EventContextType.GainEmit });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = DisplayMoneyEmit, MyContextType = EventContextType.MoneyGainEmit });

            StartMenuListener(TriggerMarketSetup);
            StartMenuListener(TriggerPlayRoundSetup);
            StartMenuListener(TriggerPackOptionSetup);
            StartMenuListener(TriggerPostRoundSetup);
            StartMenuListener(TriggerBlindsSetup);
            StartMenuListener(TriggerGameOver);
        }

        private static void StartMenuListener(Action<EngineEventArgs> listAct)
        {
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = listAct, MyContextType = EventContextType.GameStatePop });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = listAct, MyContextType = EventContextType.GameStatePush });
        }

        public static void ClearEngineEntities()
        {
            foreach (var dispEnt in EngineDisplays)
            {
                dispEnt.Visible = false;
            }
        }

        //TODO: AGAIN, I was just snorting dumb juice when writing this. lookit all the repeated code. Make better.
        private static void TriggerMarketSetup(EngineEventArgs args)
        {
            if(args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.ShopMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupMarketState();
                    ControlManager.CurrentControlset = "MARKET";
                    Redraw();
                });
            }
        }

        private static void TriggerPlayRoundSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.PlayRound))
            {
                CacheAnimationAction(_ =>
                {
                    SetupPlayRoundState();
                    ControlManager.CurrentControlset = "ROUND";
                    Redraw();
                });
            }
        }

        private static void TriggerPackOptionSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.SelectingPackOption))
            {
                CacheAnimationAction(_ =>
                {
                    SetupPackOptionSelection();
                    ControlManager.CurrentControlset = "PACK";
                    Redraw();
                });
            }
        }

        private static void TriggerPostRoundSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.PostRoundRewardsMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupPostRoundDisplay(chgArgs.NewState.PostRoundMoneySources);
                    ControlManager.CurrentControlset = "POSTROUND";
                    Redraw();
                });
            }
        }

        private static void TriggerBlindsSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.BlindsMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupBlindSelectionState();
                    ControlManager.CurrentControlset = "BLIND";
                    Redraw();
                });
            }
        }

        private static void TriggerGameOver(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.GameOverMenu))
            {
                CacheAnimationAction(_ =>
                {
                    ClearEngineEntities();
                    ShowInfoDisplay("GAME OVER", "*");
                    Redraw();
                });
            }
        }

        private static bool IsValidRoundOfState(EngineGameStateChangeArgs args, GameState state)
        {
            return (args.isPop && args.NewStateRevealedByPop != null && args.NewStateRevealedByPop.GameState == state) || (args.isPush && args.NewStateBeingPushed != null && args.NewStateBeingPushed.GameState == state);
        }

        private static void DisplayChipsMultEmit(EngineEventArgs args)
        {
            if(args is EngineChipsMultGainEmitArgs gainArgs)
            {
                var textLine = "";
                if(gainArgs.ChipsGainEmitted > 0)
                {
                    textLine = "+ " + gainArgs.ChipsGainEmitted + " Chips";
                }else if(gainArgs.MultGainEmitted > 0)
                {
                    textLine = "+ " + gainArgs.MultGainEmitted + " Mult";
                }else if(gainArgs.MultMultEmitted > 0)
                {
                    textLine = "* " + gainArgs.MultMultEmitted + " Mult!";
                }

                if (string.IsNullOrEmpty(textLine) || (!GlobalCardDisplays.ContainsKey(gainArgs.SourceOfEmit)))
                    return;

                //TELL ME WHY TF THIS DOESN'T WORK???
                //ONLY GOING DIRECTLY LIKE IT IS NOW IN THE ACTIONS WORKS. USING A VARIABLE LIKE THE BELOW DOESN'T.
                //WHYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
                //Note from later Max: conclusion is that it's because I cache those actions, but don't actually execute until later.
                //The functions call vars local to this scope. In this scope it gets changed before they're called.
                //Nope, internet says that's not how it works, still don't understand. lol.
                //var cardDisplay = GlobalCardDisplays[gainArgs.SourceOfEmit];
                //var oldSelLevel = GlobalCardDisplays[gainArgs.SourceOfEmit].SelectLevel;
                int oldSelLevel = 1;
                AboveCardDisplay textDisplay = null;
                if(gainArgs.SourceOfEmit != null && GlobalCardDisplays.ContainsKey(gainArgs.SourceOfEmit))
                {
                    textDisplay = new AboveCardDisplay(GlobalCardDisplays[gainArgs.SourceOfEmit]);
                    textDisplay.MyOverrideLines.Add(textLine);
                    textDisplay.OverrideXLoc = EngineDisplayConstants.SCOREDISPLAY_HARD_X_LOC;
                    textDisplay.OverrideYLoc = EngineDisplayConstants.SCOREDISPLAY_HARD_Y_LOC;
                    EngineInterface.AddEntity(textDisplay);
                }

                CacheAnimationAction(_ =>
                {
                    if (textDisplay != null)
                        textDisplay.Display = true;
                    if (GlobalCardDisplays.ContainsKey(gainArgs.SourceOfEmit))
                    {
                        oldSelLevel = GlobalCardDisplays[gainArgs.SourceOfEmit].SelectLevel;
                        GlobalCardDisplays[gainArgs.SourceOfEmit].SetDisplaySelectLevel(3);
                    }

                    if(BeingPlayedDisplay != null)
                    {
                        BeingPlayedDisplay.PreDisplaySetup();
                    }
                    if(gainArgs.ChipsGainEmitted > 0)
                    {
                        DisplayHandChips += gainArgs.ChipsGainEmitted;
                    }else if(gainArgs.MultGainEmitted > 0)
                    {
                        DisplayHandMult += gainArgs.MultGainEmitted;
                    }else if(gainArgs.MultMultEmitted > 0)
                    {
                        DisplayHandMult *= gainArgs.MultMultEmitted;
                    }
                });
                CacheAnimationAction(_ =>
                {
                    if (textDisplay != null)
                        textDisplay.Display = false;
                    if (GlobalCardDisplays.ContainsKey(gainArgs.SourceOfEmit))
                        GlobalCardDisplays[gainArgs.SourceOfEmit].SetDisplaySelectLevel(oldSelLevel);
                    if(textDisplay != null)
                        EngineInterface.RemoveEntity(textDisplay);
                });
            }
        }

        private static void DisplayMoneyEmit(EngineEventArgs args)
        {
            if(args is EngineGoldGainEmitArgs goldArgs && goldArgs.AmountGained != 0)
            {
                var textLine = "";
                if(goldArgs.AmountGained > 0)
                {
                    textLine = "+ " + goldArgs.AmountGained + " money!";
                }
                else
                {
                    int absVal = Math.Abs(goldArgs.AmountGained);
                    textLine = "- " + absVal + " money";
                }

                //TODO: Repeated code from above function...
                int oldSelLevel = 1;
                AboveCardDisplay textDisplay = null;
                if (goldArgs.SourceOfEmit != null && GlobalCardDisplays.ContainsKey(goldArgs.SourceOfEmit))
                {
                    textDisplay = new AboveCardDisplay(GlobalCardDisplays[goldArgs.SourceOfEmit]);
                    textDisplay.MyOverrideLines.Add(textLine);
                    textDisplay.OverrideXLoc = EngineDisplayConstants.SCOREDISPLAY_HARD_X_LOC;
                    textDisplay.OverrideYLoc = EngineDisplayConstants.SCOREDISPLAY_HARD_Y_LOC;
                    EngineInterface.AddEntity(textDisplay);
                }

                CacheAnimationAction(_ =>
                {
                    if(textDisplay != null)
                    {
                        textDisplay.Display = true;
                        if (GlobalCardDisplays.ContainsKey(goldArgs.SourceOfEmit))
                        {
                            oldSelLevel = GlobalCardDisplays[goldArgs.SourceOfEmit].SelectLevel;
                            GlobalCardDisplays[goldArgs.SourceOfEmit].SetDisplaySelectLevel(3);
                        }
                    }

                    if (BeingPlayedDisplay != null)
                        BeingPlayedDisplay.PreDisplaySetup();

                    if (goldArgs.AmountGained != 0)
                        DisplayMoney += goldArgs.AmountGained;
                });

                CacheAnimationAction(_ =>
                {
                    if(textDisplay != null)
                    {
                        textDisplay.Display = false;
                        if (GlobalCardDisplays.ContainsKey(goldArgs.SourceOfEmit))
                            GlobalCardDisplays[goldArgs.SourceOfEmit].SetDisplaySelectLevel(oldSelLevel);

                        EngineInterface.RemoveEntity(textDisplay);
                    }
                });
            }
        }

        public static void Redraw()
        {
            Console.Clear();
            EngineInterface.Draw();
            RedrawCached = false;
        }

        //TODO: Careful spongebob. This could break some things. Currently you can't cache multiple redraws EVEN IF non-consecutive.
        public static void CacheRedraw()
        {
            if (!RedrawCached)
            {
                CacheAnimationAction(_ => { Redraw(); });
                RedrawCached = true;
            }
        }

        public static void PlayCachedAnimations()
        {
            //TODO: For now, no args needed. Maybe later will be needed.
            Animation.PerformAnimatedAction(null, true);
        }

        public static void CacheAnimationAction(Action<AnimationFrameArgs> action, int delayInvolved = -1)
        {
            if (OVERRIDE_ANIMATIONS)
            {
                Animation.FrameActions.Add(new AnimationFrame() { MyAction = action, MyFrameDelay = 1 });
            }
            else
            {
                Animation.FrameActions.Add(new AnimationFrame() { MyAction = action, MyFrameDelay = delayInvolved });
            }
        }
        public static void ResetPlayedHand()
        {
            DisplayHandChips = 0;
            DisplayHandMult = 0;
            DisplayPlayedHand = PlayedHandType.FLUSHFIVE;
        }
        public static string setCharAt(string baseS, int ind, char x)
        {
            var cArr = baseS.ToCharArray();
            cArr[ind] = x;
            return new string(cArr);
        }

        public static string setCharsAt(string baseS, int startInd, List<char> charsToInsert)
        {
            var ret = baseS;
            for (int i = startInd; i < startInd + charsToInsert.Count; i++)
            {
                ret = setCharAt(ret, i, charsToInsert[i - startInd]);
            }
            return ret;
        }

        public static string setCharsAt(string baseS, int startInd, string charsInsert)
        {
            return setCharsAt(baseS, startInd, charsInsert.ToCharArray().ToList());
        }
    }
}
