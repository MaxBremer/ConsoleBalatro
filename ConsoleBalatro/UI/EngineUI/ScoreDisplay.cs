using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class ScoreDisplay : TextDisplayPanel
    {
        public ScoreDisplay() : base(new List<string>(), EngineDisplayConstants.SCOREDISPLAY_WIDTH_MIN, EngineDisplayConstants.SCOREDISPLAY_HEIGHT_MIN)
        {
            ResetTextLines();
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnSelectAction, MyContextType = EventContextType.CardSelect, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnTotalChipGainAction, MyContextType = EventContextType.TotalChipsGained, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnTotalChipResetAction, MyContextType = EventContextType.TotalChipsReset, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnRequirementSet, MyContextType = EventContextType.RequiredChipsSet, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnHandsChange, MyContextType = EventContextType.HandsChange, NonEngineListener = true });
            EngineEventHandler.StartListening(new EngineEventListener() { MyAction = OnDiscChange, MyContextType = EventContextType.DiscardsChange, NonEngineListener = true });
        }

        private List<string> GetMyTextLines()
        {
            var ret = new List<string>();
            if(Globals.RequiredChipsForCurrentBlind < 0)
            {
                return ret;
            }
            ret.Add("CURRENT BLIND");
            ret.Add("");

            ret.Add(Globals.FormatChipCount(EngineDisplayGlobals.DisplayRequiredChipsForBlind));
            ret.Add("");
            ret.Add("TOTAL CHIPS");

            ret.Add(Globals.FormatChipCount(EngineDisplayGlobals.DisplayTotalCurrentChips));
            ret.Add("");
            ret.Add("MONEY");
            ret.Add(EngineDisplayGlobals.DisplayMoney.ToString() + "$");

            //Janky little fix to no NULL display hand enum option. What do you want?
            if(EngineDisplayGlobals.DisplayHandChips == 0)
            {
                ret.Add("NO HAND SELECTED");
                ret.Add("");
            }
            else
            {
                var curSelHand = EngineDisplayGlobals.DisplayPlayedHand;
                var curChips = EngineDisplayGlobals.DisplayHandChips;
                var curMult = EngineDisplayGlobals.DisplayHandMult;
                ret.Add(curSelHand.ToString());

                ret.Add(Globals.FormatChipCount(curChips) + " X " + curMult.ToString("F2"));
            }

            ret.Add("");
            ret.Add("HANDS: " + EngineDisplayGlobals.DisplayHandsRemaining);
            ret.Add("DISCARDS: " + EngineDisplayGlobals.DisplayDiscardsRemaining);
            ret.Add("");

            ret.Add("SORT BY [R]ANK");
            ret.Add("SORT BY [S]UIT");

            if(FlowHandler.CurrentSelectedBlind == BlindType.BOSS && ZoneManager.HiddenBlindAttributeZone.Cards.Count > 0)
            {
                ret.Add("");
                ret.Add("BOSS BLIND: ");
                ret.Add(FlowHandler.CurrentBossBlind);
                ret.Add("VIEW [D]ETAILS");
                //EngineDisplayGlobals.AddButDontExpand(ret, ZoneManager.HiddenBlindAttributeZone.Cards[0].JokerData.DescriptionBuilder(null));
            }

            return ret;
        }

        public override void PreDisplaySetup()
        {
            ResetTextLines();
            base.PreDisplaySetup();
        }

        private void ResetTextLines()
        {
            _textLines.Clear();
            _textLines.AddRange(GetMyTextLines());
        }

        private void OnSelectAction(EngineEventArgs args)
        {
            if(args.MyContext.Context == EventContextType.CardSelect && args is EngineCardSelectedArgs selArgs && ZoneManager.HandZone.Cards.Contains(selArgs.TargetedCard))
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    EngineDisplayGlobals.DisplayPlayedHand = EngineUtils.BestHandFromCards(ZoneManager.CardsSelectedInHand).Item1;//TODO: THIS SHOULDNT RECALC HAND. OVERRIDE HAND TYPE COULD HAPPEN.
                    EngineDisplayGlobals.DisplayHandChips = ScoreHandler.CurrentHandStats[EngineDisplayGlobals.DisplayPlayedHand].Item1;
                    EngineDisplayGlobals.DisplayHandMult = ScoreHandler.CurrentHandStats[EngineDisplayGlobals.DisplayPlayedHand].Item2;
                });
            }
        }

        private void OnHandsChange(EngineEventArgs args)
        {
            if(args is EngineHandDiscChangeArgs hArgs && hArgs.isHand)
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    EngineDisplayGlobals.DisplayHandsRemaining = hArgs.newVal;
                });
            }
        }

        private void OnDiscChange(EngineEventArgs args)
        {
            if(args is EngineHandDiscChangeArgs dArgs && !dArgs.isHand)
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    EngineDisplayGlobals.DisplayDiscardsRemaining = dArgs.newVal;
                });
            }
        }

        private void OnTotalChipGainAction(EngineEventArgs args)
        {
            if(args is EngineTotalChipsGainArgs gainArgs && gainArgs.AmountBeingGained > 0)
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    EngineDisplayGlobals.DisplayTotalCurrentChips = Globals.CapChipCount(EngineDisplayGlobals.DisplayTotalCurrentChips + gainArgs.AmountBeingGained);
                    EngineDisplayGlobals.ResetPlayedHand();
                });
            }
        }

        private void OnTotalChipResetAction(EngineEventArgs args)
        {
            if (args.MyContext.Context == EventContextType.TotalChipsReset)
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    EngineDisplayGlobals.DisplayTotalCurrentChips = 0;
                });
            }
        }

        private void OnRequirementSet(EngineEventArgs args)
        {
            if(args is EngineRequirementSetArgs reqArgs)
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    EngineDisplayGlobals.DisplayRequiredChipsForBlind = reqArgs.RequirementBeingSet;
                }, 0);
            }
        }
    }
}
