using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.UI.EngineUI.Animation;
using ConsoleBalatro.UI.EngineUI.Controls;
using ConsoleBalatro.UI.EngineUI.MarketPanels;
using ConsoleBalatro.UI.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleBalatro.UI.EngineUI
{
    public static class EngineDisplayGlobals
    {
        public const bool OVERRIDE_ANIMATIONS = true;

        public static readonly Dictionary<Edition, string> EditionBorderChars = new()
        {
            {Edition.FOIL, "^" },
            {Edition.HOLOGRAPHIC, "w" },
            {Edition.NEGATIVE, "_" },
            {Edition.POLYCHROME, "z" },

        };

        public static readonly Dictionary<Enhancement, Func<string, string>> EnhancementModifiers = new()
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

        public static readonly Dictionary<Seal, Func<string, string>> SealModifiers = new()
        {
            { Seal.RED, x => setCharAt(x, 9, 'r') },
            { Seal.BLUE, x => setCharAt(x, 9, 'b') },
            { Seal.PURPLE, x => setCharAt(x, 9, 'p') },
            { Seal.GOLD, x => setCharAt(x, 9, 'g') },
        };

        public static readonly Dictionary<Sticker, Func<string, string>> StickerModifiers = new()
        {
            { Sticker.ETERNAL, x => setCharAt(x, 10, 'E') },
            { Sticker.PERISHABLE, x => setCharAt(x, 16, 'P') },
            { Sticker.RENTAL, x => setCharAt(x, 22, 'R') },
        };

        /// <summary>
        /// Tracks the CardDisplay UI objects for each Card in the engine that should currently be displayed.
        /// </summary>
        public static Dictionary<Card, CardDisplay> GlobalCardDisplays = new();

        //Almost universally visible.
        /// <summary>
        /// Joker Zone display. (top bar that contains Jokers, almost always visible)
        /// </summary>
        public static CardZoneJokersDisplay JokersDisplay;
        /// <summary>
        /// Consumable Zone display. (top-right box that contains consumables, almost always visible)
        /// </summary>
        public static CardZoneConsumableDisplay ConsumableDisplay;

        //Play round visible.
        /// <summary>
        /// Hand Zone display. (shows cards in hand during play round)
        /// </summary>
        public static CardZoneHandDisplay HandDisplay;
        /// <summary>
        /// Being Played Zone display. (shows cards currently being played/scoring, pretty much only during scoring animation in play round)
        /// </summary>
        public static CardZoneBeingPlayedDisplay BeingPlayedDisplay;

        //Market round visible.
        /// <summary>
        /// Main market Zone display. (cards in the primary market that can be rerolled, i.e. Jokers and individual consumables)
        /// </summary>
        public static CardZoneDisplay MainMarketDisplay;
        /// <summary>
        /// Pack market Zone display. (packs in the market, rolled once per market round)
        /// </summary>
        public static CardZoneDisplay PackMarketDisplay;
        /// <summary>
        /// Voucher market Zone display. (voucher(s) in the market. rolled once per ante except in edge cases i.e. voucher tag)
        /// </summary>
        public static CardZoneDisplay VoucherMarketDisplay;

        /// <summary>
        /// Displays consumable options given by an opened pack
        /// </summary>
        public static CardZoneDisplay PackOptionsDisplay;

        /// <summary>
        /// Large side panel that displays score, controls, other info during a play round.
        /// </summary>
        public static ScoreDisplay ScoreDisplay;

        /// <summary>
        /// Large side panel that displays money, controls, other contextual info during market round, pack option round, and blind selection round.
        /// (name is a misnomer, originally was only for market)
        /// </summary>
        public static MarketSideDisplay MarketSidePanelDisplay;

        //Blind selection.
        /// <summary>
        /// Panel displaying info on first/small blind during blind selection.
        /// </summary>
        public static BlindDisplayEntity FirstBlindPanel;
        /// <summary>
        /// Panel displaying info on second/big blind during blind selection.
        /// </summary>
        public static BlindDisplayEntity SecondBlindPanel;
        /// <summary>
        /// Panel displaying info on third/boss blind during blind selection.
        /// </summary>
        public static BlindDisplayEntity ThirdBlindPanel;

        /// <summary>
        /// Popup displaying hand stats, i.e. hand level, hand num times played, current hand chipsxmult.
        /// </summary>
        public static HandStatsDisplay HandStatsMenu;

        /// <summary>
        /// Menu for choosing a deck at the start of a run.
        /// </summary>
        public static DeckChoicesDisplay DeckChoiceMenu;

        public static MainMenuDisplay MainMenu;

        public static PlaceholderMenuDisplay PlaceholderMenu;

        public static DeckViewDisplay DeckViewMenu;

        //DISPLAY fields are soft mirrors of their engine counterpart;
        //basically, they exist so that their update to match the engine can be delayed for animations to play out.
        public static BigInteger DisplayRequiredChipsForBlind;
        public static BigInteger DisplayTotalCurrentChips;

        public static int DisplayHandsRemaining;
        public static int DisplayDiscardsRemaining;

        public static int DisplayMoney = 0;

        public static BigInteger DisplayHandChips;
        public static double DisplayHandMult;
        public static PlayedHandType DisplayPlayedHand;

        public static Interface EngineInterface;
        public static List<DisplayEntity> EngineDisplays;

        public static TextDisplayPanel PostRoundRewardsPanel;
        public static TextDisplayPanel InfoDisplayPanel;
        public static CardDisplay CardBeingViewed = null;

        public static EngineActionAnimation Animation = new EngineActionAnimation();

        /// <summary>
        /// Flag indicating whether we've cached a simple redraw in the animation queue (so that only ONE is ever cached)
        /// </summary>
        private static bool RedrawCached = false;

        /// <summary>
        /// The big one. Initialize all displays, controls, UI listeners, etc.
        /// </summary>
        /// <param name="theGuy">The Interface object on which all this EngineUI will be displayed.</param>
        public static void InitializeDisplayAll(Interface theGuy)
        {
            EngineInterface = theGuy;

            SyncValues();
            InitializeGlobalListeners();
            ControlManager.InitializeControls();
            InitializeAllDisplays();
        }

        private static void SyncValues()
        {
            DisplayMoney = Globals.Money;
        }

        //TODO:...do I even fucking need this?
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

        /// <summary>
        /// Hide the general-purpose info panel.
        /// </summary>
        public static void HideInfoDisplay()
        {
            InfoDisplayPanel.Visible = false;
        }

        /// <summary>
        /// Show passed string, given a line-break character to split it on, in the general-purpose info panel.
        /// </summary>
        /// <param name="infoToDisplay">The string that will be displayed.</param>
        /// <param name="breakChar">The character to use to line-break the above string.</param>
        public static void ShowInfoDisplay(string infoToDisplay, string breakChar)
        {
            InfoDisplayPanel.SetLines(infoToDisplay, breakChar);
            InfoDisplayPanel.AdjustLinesByWrapWidth(EngineDisplayConstants.INFO_PANEL_WIDTH);
            InfoDisplayPanel.Visible = true;
            //Also not this funcs responsibility to redraw.
        }

        /// <summary>
        /// Show the passed string in the general-purpose info panel, given a maximum width of said panel (default 15).
        /// </summary>
        /// <param name="infoToDisplay">String to be displayed.</param>
        /// <param name="maxLen">Maximum width of the info panel when displaying this string (default 15)</param>
        public static void ShowInfoDisplay(string infoToDisplay, int maxLen = 15)
        {
            InfoDisplayPanel.SetLines(infoToDisplay, "$%^&");
            InfoDisplayPanel.AdjustLinesByWrapWidth(maxLen);
            InfoDisplayPanel.Visible = true;
        }

        /// <summary>
        /// Display detailed info for the passed card display in the info panel.
        /// </summary>
        /// <param name="cd">CardDisplay UI object of the card to be displayed.</param>
        public static void DisplayDetailInfoForCardDisplay(CardDisplay cd)
        {
            CardBeingViewed = cd;
            //TODO: EventContext maybe needs to be passed.
            ShowInfoDisplay(cd.MyCard.DetailedInfoDisplay(null), Card.CardInfoLineDivider);
            //TODO: adjust card display to highlight it or something.
        }

        /// <summary>
        /// Display detail info for the passed Card.
        /// </summary>
        /// <param name="c">Card whose details should be displayed.</param>
        public static void DisplayDetailInfoForCard(Card c)
        {
            if (GlobalCardDisplays.ContainsKey(c))
                DisplayDetailInfoForCardDisplay(GlobalCardDisplays[c]);
        }

        /// <summary>
        /// Clear all the "display-beneath" characters, used for zone selection.
        /// </summary>
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

        /// <summary>
        /// Turn all engine entities invisible.
        /// </summary>
        public static void HideAllEngineEntities()
        {
            foreach (var dispEnt in EngineDisplays)
            {
                dispEnt.Visible = false;
            }
        }

        /// <summary>
        /// Redraw the engine interface object.
        /// </summary>
        public static void Redraw()
        {
            ControlManager.ClearConsole();
            EngineInterface.Draw();
            RedrawCached = false;
        }

        //TODO: Careful spongebob. This could break some things. Currently you can't cache multiple redraws EVEN IF non-consecutive.
        /// <summary>
        /// Cache a full interface-redraw in the animation queue.
        /// </summary>
        public static void CacheRedraw()
        {
            if (!RedrawCached)
            {
                CacheAnimationAction(_ => { Redraw(); });
                RedrawCached = true;
            }
        }

        /// <summary>
        /// Play through all cached animations, returning the engine Interface to sync with the engine.
        /// </summary>
        public static void PlayCachedAnimations()
        {
            //TODO: For now, no args needed. Maybe later will be needed.
            Animation.PerformAnimatedAction(null, true);
        }

        /// <summary>
        /// Cache an action to be played as an animation.
        /// </summary>
        /// <param name="action">The action to be taken in the animation.</param>
        /// <param name="delayInvolved">The pause (in ms) to be taken after this action. Default = -1, which results in using the GlobalFrameDelay in the animation engine.</param>
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

        /// <summary>
        /// Reset the display fields for current hand being played.
        /// </summary>
        public static void ResetPlayedHand()
        {
            DisplayHandChips = 0;
            DisplayHandMult = 0;
            DisplayPlayedHand = PlayedHandType.FLUSHFIVE;
        }

        private static void SetupMainMenuState()
        {
            HideAllEngineEntities();

            MainMenu.Visible = true;
        }

        private static void SetupPlaceholderMenuState(string title, string body)
        {
            HideAllEngineEntities();

            PlaceholderMenu.Title = title;
            PlaceholderMenu.Body = body;
            PlaceholderMenu.Visible = true;
        }

        private static void SetupDeckChoiceState()
        {
            HideAllEngineEntities();

            DeckChoiceMenu.Visible = true;
        }

        private static void SetupPlayRoundState()
        {
            HideAllEngineEntities();

            HandDisplay.Visible = true;
            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            BeingPlayedDisplay.Visible = true;
            ScoreDisplay.Visible = true;
        }

        private static void SetupMarketState()
        {
            HideAllEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            MarketSidePanelDisplay.Visible = true;
            MarketSidePanelDisplay.SideImDisplaying = "MARKET";
            MainMarketDisplay.Visible = true;
            VoucherMarketDisplay.Visible = true;
            PackMarketDisplay.Visible = true;
        }

        private static void SetupPackOptionSelection()
        {
            HideAllEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            MarketSidePanelDisplay.Visible = true;
            MarketSidePanelDisplay.SideImDisplaying = "PACK";

            PackOptionsDisplay.Visible = true;
            HandDisplay.Visible = true;
        }

        private static void SetupPostRoundDisplay(List<(string, int)> moneyData)
        {
            HideAllEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;

            var totalMoney = moneyData.Sum(x => x.Item2);

            var breakStr = "@";
            var finalStr = "";
            foreach (var money in moneyData)
            {
                finalStr += money.Item1 + ": " + money.Item2 + breakStr;
            }

            finalStr += breakStr + "TOTAL: " + totalMoney + breakStr;
            finalStr += breakStr + "[C]ontinue";

            PostRoundRewardsPanel.SetLines(finalStr, breakStr);
            PostRoundRewardsPanel.AdjustLinesByWrapWidth(EngineDisplayConstants.INFO_PANEL_WIDTH);
            PostRoundRewardsPanel.Visible = true;
        }

        private static void SetupBlindSelectionState()
        {
            HideAllEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            MarketSidePanelDisplay.Visible= true;
            MarketSidePanelDisplay.SideImDisplaying = "BLIND";

            FirstBlindPanel.Visible = true;
            SecondBlindPanel.Visible = true;
            ThirdBlindPanel.Visible = true;
        }

        private static void InitializeGlobalListeners()
        {
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = DisplayAchievementUnlocked, MyContextType = EventContextType.AchievementUnlocked, NonEngineListener = true});
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = DisplayChipsMultEmit, MyContextType = EventContextType.GainEmit, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = DisplayMoneyEmit, MyContextType = EventContextType.MoneyGainEmit, NonEngineListener = true });

            StartMenuListener(TriggerMarketSetup);
            StartMenuListener(TriggerPlayRoundSetup);
            StartMenuListener(TriggerMainMenuSetup);
            StartMenuListener(TriggerCollectionMenuSetup);
            StartMenuListener(TriggerOptionsMenuSetup);
            StartMenuListener(TriggerDeckChoiceSetup);
            StartMenuListener(TriggerPackOptionSetup);
            StartMenuListener(TriggerPostRoundSetup);
            StartMenuListener(TriggerBlindsSetup);
            StartMenuListener(TriggerGameOver);
            StartMenuListener(TriggerWinSetup);
        }

        private static void InitializeAllDisplays()
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
            PostRoundRewardsPanel = new TextDisplayPanel(new List<string>() { "POST-ROUND REWARDS" }, EngineDisplayConstants.TEXT_DISPLAY_MINWIDTH, EngineDisplayConstants.TEXT_DISPLAY_MINHEIGHT)
            {
                xLoc = EngineDisplayConstants.TEXT_DISPLAY_XLOC
            };
            EngineDisplays.Add(PostRoundRewardsPanel);
            InfoDisplayPanel = new TextDisplayPanel(new List<string>() { "INFO" }, EngineDisplayConstants.TEXT_DISPLAY_MINWIDTH, EngineDisplayConstants.TEXT_DISPLAY_MINHEIGHT)
            {
                xLoc = EngineDisplayConstants.TEXT_DISPLAY_XLOC
            };
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

            HandStatsMenu = new HandStatsDisplay(EngineDisplayConstants.HAND_STATS_DISPLAY_XLOC, EngineDisplayConstants.HAND_STATS_DISPLAY_YLOC);
            EngineDisplays.Add(HandStatsMenu);

            DeckChoiceMenu = new DeckChoicesDisplay(EngineDisplayConstants.DECK_CHOICE_DISPLAY_XLOC, EngineDisplayConstants.DECK_CHOICE_DISPLAY_YLOC);
            EngineDisplays.Add(DeckChoiceMenu);

            MainMenu = new MainMenuDisplay(EngineDisplayConstants.MAIN_MENU_DISPLAY_XLOC, EngineDisplayConstants.MAIN_MENU_DISPLAY_YLOC);
            EngineDisplays.Add(MainMenu);

            PlaceholderMenu = new PlaceholderMenuDisplay(EngineDisplayConstants.MAIN_MENU_DISPLAY_XLOC, EngineDisplayConstants.MAIN_MENU_DISPLAY_YLOC);
            EngineDisplays.Add(PlaceholderMenu);

            DeckViewMenu = new DeckViewDisplay();
            EngineDisplays.Add(DeckViewMenu);

            foreach (var dispEnt in EngineDisplays)
            {
                dispEnt.Visible = false;
                EngineInterface.AddEntity(dispEnt);
            }
        }

        private static void DisplayAchievementUnlocked(EngineEventArgs args)
        {
            if (EngineInterface == null)
                return;
            if(args is EngineAchievementUnlockArgs achArgs)
            {
                var lines = new List<string>
                {
                    "ACHIEVEMENT UNLOCKED!",
                    achArgs.AchievementName,
                    achArgs.AchievementDesc,
                };
                var popup = new TextDisplayPanel(lines, 48, 7)
                {
                    xLoc = Math.Max(0, (Interface.Display_Width - 48) / 2),
                    yLoc = 1,
                    zSortOrder = 1000,
                    Visible = false,
                    ClearBg = false,
                };

                popup.AdjustLinesByWrapWidth(42);
                EngineInterface.AddEntity(popup);

                CacheAnimationAction(_ =>
                {
                    popup.Visible = true;
                }, 1500);

                CacheAnimationAction(_ =>
                {
                    popup.Visible = false;
                    EngineInterface.RemoveEntity(popup);
                }, 0);
            }
        }

        private static void StartMenuListener(Action<EngineEventArgs> listAct)
        {
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = listAct, MyContextType = EventContextType.GameStatePop, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = listAct, MyContextType = EventContextType.GameStatePush, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = listAct, MyContextType = EventContextType.GameStateReplace, NonEngineListener = true });
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

        private static void TriggerMainMenuSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.MainMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupMainMenuState();
                    ControlManager.CurrentControlset = "MAINMENU";
                    Redraw();
                });
            }
        }

        private static void TriggerCollectionMenuSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.CollectionMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupPlaceholderMenuState("COLLECTION", "Collection viewer will be implemented later.");
                    ControlManager.CurrentControlset = "PLACEHOLDERMENU";
                    Redraw();
                });
            }
        }

        private static void TriggerOptionsMenuSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.OptionsMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupPlaceholderMenuState("OPTIONS", "Options menu will be implemented later.");
                    ControlManager.CurrentControlset = "PLACEHOLDERMENU";
                    Redraw();
                });
            }
        }

        private static void TriggerDeckChoiceSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.DeckSelectMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupDeckChoiceState();
                    ControlManager.CurrentControlset = "DECKCHOICE";
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

        private static void SetupWinState()
        {
            HideAllEngineEntities();

            JokersDisplay.Visible = true;
            ConsumableDisplay.Visible = true;
            ShowInfoDisplay("YOU WIN!&!&Ante 8 boss defeated.&Your deck and current Jokers earned this stake sticker.&!&[E]nd run: save your progress and quit the game&[C]ontinue: play endless mode", "&");
        }

        private static void TriggerWinSetup(EngineEventArgs args)
        {
            if (args is EngineGameStateChangeArgs chgArgs && IsValidRoundOfState(chgArgs, GameState.WinMenu))
            {
                CacheAnimationAction(_ =>
                {
                    SetupWinState();
                    ControlManager.CurrentControlset = "WIN";
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
                    HideAllEngineEntities();
                    ShowInfoDisplay("GAME OVER", "*");
                    ControlManager.CurrentControlset = "GAMEOVER";
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
                        DisplayHandChips = Globals.CapChipCount(DisplayHandChips + gainArgs.ChipsGainEmitted);
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

        #region Helpers

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

        public static void AddButDontExpand(List<string> lines, string text)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            if (string.IsNullOrWhiteSpace(text))
                return;

            int maxLength = lines.Count > 0
                ? lines.Max(x => x.Length)
                : text.Length;

            string remaining = text.Trim();

            while (remaining.Length > 0)
            {
                if (remaining.Length <= maxLength)
                {
                    lines.Add(remaining);
                    break;
                }

                int splitIndex = remaining.LastIndexOf(' ', maxLength);

                // No suitable space found, force split
                if (splitIndex <= 0)
                {
                    lines.Add(remaining[..maxLength]);
                    remaining = remaining[maxLength..].TrimStart();
                }
                else
                {
                    lines.Add(remaining[..splitIndex]);
                    remaining = remaining[(splitIndex + 1)..].TrimStart();
                }
            }
        }
        #endregion
    }
}
