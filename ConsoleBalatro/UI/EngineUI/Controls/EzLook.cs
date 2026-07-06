using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static ConsoleBalatro.UI.EngineUI.Controls.ControlManager;

namespace ConsoleBalatro.UI.EngineUI.Controls
{
    public static class EzLook
    {
        public const ConsoleKey EZ_LOOK_KEY = ConsoleKey.P;

        public static Dictionary<CardZone, ZoneGridData> ZoneData = new();
        public static Card CurrentTarget = null;
        public static int CurrentTargetIndex = 0;
        public static CardZone? CurrentTargetZone;

        public static void EngageEzLook(ControlOptionset currentControls)
        {
            CurrentTarget = null;
            CurrentTargetZone = null;
            //Set up current zonedata by control info, including lookzones.
            SetupZoneDataFor(currentControls);

            CurrentTargetZone = ZoneData.Keys.ToList().FirstOrDefault(x => x.Cards.Any());
            if (CurrentTargetZone == null)
                return;
            if(CurrentTargetIndex >= CurrentTargetZone.Cards.Count)
                CurrentTargetIndex = CurrentTargetZone.Cards.Count - 1;

            RefreshTarget();

            var continueEzLook = true;
            while (continueEzLook)
            {
                var pressed = ControlManager.ReadKey();
                continueEzLook = MovementKeyPressed(pressed.Key);
                EngineDisplayGlobals.Redraw();
            }

            CurrentTargetZone = null;
            RefreshTarget();
        }

        

        public static void RefreshTarget()
        {
            if(CurrentTarget != null && EngineDisplayGlobals.GlobalCardDisplays.ContainsKey(CurrentTarget))
            {
                EngineDisplayGlobals.GlobalCardDisplays[CurrentTarget].SetDisplaySelectLevel(0);
                EngineDisplayGlobals.HideInfoDisplay();
            }
            if (CurrentTargetZone == null)
            {
                EngineDisplayGlobals.Redraw();
                return;
            }
            CurrentTarget = CurrentTargetZone.Cards[CurrentTargetIndex];
            EngineDisplayGlobals.GlobalCardDisplays[CurrentTarget].SetDisplaySelectLevel(3);
            EngineDisplayGlobals.DisplayDetailInfoForCard(CurrentTarget);
            EngineDisplayGlobals.Redraw();
        }

        //Return val indicates whether to continue ez look.
        public static bool MovementKeyPressed(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Escape:
                case EZ_LOOK_KEY:
                    return false;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (CurrentTargetIndex < CurrentTargetZone.Cards.Count - 1)
                    {
                        CurrentTargetIndex++;
                        RefreshTarget();
                    }
                    else if (ZoneData[CurrentTargetZone].RightHandZone != null && ZoneData[CurrentTargetZone].RightHandZone.MyZone.Cards.Count > 0)
                    {
                        CurrentTargetZone = ZoneData[CurrentTargetZone].RightHandZone.MyZone;
                        CurrentTargetIndex = 0;
                        RefreshTarget();
                    }
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (CurrentTargetIndex > 0)
                    {
                        CurrentTargetIndex--;
                        RefreshTarget();
                    }
                    else if (ZoneData[CurrentTargetZone].LeftHandZone != null && ZoneData[CurrentTargetZone].LeftHandZone.MyZone.Cards.Count > 0)
                    {
                        CurrentTargetZone = ZoneData[CurrentTargetZone].LeftHandZone.MyZone;
                        CurrentTargetIndex = CurrentTargetZone.Cards.Count - 1;
                        RefreshTarget();
                    }
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    /*if (ZoneData[CurrentTargetZone].BottomZone != null && ZoneData[CurrentTargetZone].BottomZone.MyZone.Cards.Count > 0)
                    {
                        CurrentTargetIndex = ZoneData[CurrentTargetZone].BottomZone.MyZone.Cards.Count > CurrentTargetIndex + ZoneData[CurrentTargetZone].BottomZone.OffsetFromTopZone ? CurrentTargetIndex + ZoneData[CurrentTargetZone].BottomZone.OffsetFromTopZone : ZoneData[CurrentTargetZone].BottomZone.MyZone.Cards.Count - 1;
                        CurrentTargetZone = ZoneData[CurrentTargetZone].BottomZone.MyZone;
                        RefreshTarget();
                    }*/
                    AttemptMoveZone(ZoneDirection.DOWN);
                    break;
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    /*if (ZoneData[CurrentTargetZone].TopZone != null && ZoneData[CurrentTargetZone].TopZone.MyZone.Cards.Count > 0)
                    {
                        CurrentTargetIndex = ZoneData[CurrentTargetZone].TopZone.MyZone.Cards.Count > CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone ? CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone : ZoneData[CurrentTargetZone].TopZone.MyZone.Cards.Count - 1;
                        CurrentTargetZone = ZoneData[CurrentTargetZone].TopZone.MyZone;
                        RefreshTarget();
                    }*/
                    AttemptMoveZone(ZoneDirection.UP);
                    break;
                default:
                    break;
            }
            return true;
        }

        private static void AttemptMoveZone(ZoneDirection direction)
        {
            if (direction == ZoneDirection.UP)
            {
                if (ZoneData[CurrentTargetZone].TopZone != null)
                {
                    if (ZoneData[CurrentTargetZone].TopZone.MyZone.Cards.Count > 0)
                    {
                        CurrentTargetIndex = ZoneData[CurrentTargetZone].TopZone.MyZone.Cards.Count > CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone ? CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone : ZoneData[CurrentTargetZone].TopZone.MyZone.Cards.Count - 1;
                        CurrentTargetZone = ZoneData[CurrentTargetZone].TopZone.MyZone;
                        RefreshTarget();
                    }
                    else if (ZoneData[CurrentTargetZone].TopZone.TopZone != null)
                    {
                        CurrentTargetIndex = ZoneData[CurrentTargetZone].TopZone.TopZone.MyZone.Cards.Count > CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone ? CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone : ZoneData[CurrentTargetZone].TopZone.TopZone.MyZone.Cards.Count - 1;
                        CurrentTargetZone = ZoneData[CurrentTargetZone].TopZone.TopZone.MyZone;
                        RefreshTarget();
                    }
                }
            }else if(direction == ZoneDirection.DOWN)
            {
                if (ZoneData[CurrentTargetZone].BottomZone != null)
                {
                    if (ZoneData[CurrentTargetZone].BottomZone.MyZone.Cards.Count > 0)
                    {
                        CurrentTargetIndex = ZoneData[CurrentTargetZone].BottomZone.MyZone.Cards.Count > CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone ? CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone : ZoneData[CurrentTargetZone].BottomZone.MyZone.Cards.Count - 1;
                        CurrentTargetZone = ZoneData[CurrentTargetZone].BottomZone.MyZone;
                        RefreshTarget();
                    }
                    else if (ZoneData[CurrentTargetZone].BottomZone.BottomZone != null)
                    {
                        CurrentTargetIndex = ZoneData[CurrentTargetZone].BottomZone.BottomZone.MyZone.Cards.Count > CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone ? CurrentTargetIndex + ZoneData[CurrentTargetZone].OffsetFromTopZone : ZoneData[CurrentTargetZone].BottomZone.BottomZone.MyZone.Cards.Count - 1;
                        CurrentTargetZone = ZoneData[CurrentTargetZone].BottomZone.BottomZone.MyZone;
                        RefreshTarget();
                    }
                }
            }
            
        }

        public enum ZoneDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
        }

        public class ZoneGridData
        {
            public CardZone MyZone;
            public ZoneGridData RightHandZone;
            public ZoneGridData LeftHandZone;
            public ZoneGridData TopZone;
            public ZoneGridData BottomZone;

            public int OffsetFromTopZone = 0;

            public ZoneGridData(CardZone targetZone)
            {
                MyZone = targetZone;
            }
        }

        private static void SetupZoneDataFor(ControlOptionset currentControls)
        {
            ZoneData.Clear();
            switch (currentControls.SchemaName)
            {
                case "PlayRound":
                    SetupPlayRoundZoneInfo();
                    break;
                case "Market":
                    SetupMarketZoneInfo();
                    break;
                case "PackOptionSelection":
                    SetupPackOptionZoneInfo();
                    break;
                case "PostRoundScreen":
                    SetupBasicZoneInfo();
                    break;
                case "BlindSelection":
                    SetupBlindSelectionZoneInfo();
                    break;
            }
        }

        private static void SetupPackOptionZoneInfo()
        {
            SetupBasicZoneInfo();
            var packOptions = new ZoneGridData(ZoneManager.PackOptionZone);
            var handOptions = new ZoneGridData(ZoneManager.HandZone);
            BindUD(ZoneData[ZoneManager.JokerZone], packOptions);
            BindUD(packOptions, handOptions);
            ZoneData.Add(ZoneManager.PackOptionZone, packOptions);
            ZoneData.Add(ZoneManager.HandZone, handOptions);
        }

        private static void SetupMarketZoneInfo()
        {
            SetupBasicZoneInfo();
            var mainMarket = new ZoneGridData(ZoneManager.MainMarketZone);
            var vouchMarket = new ZoneGridData(ZoneManager.VoucherMarketZone);
            var packMarket = new ZoneGridData(ZoneManager.PackMarketZone);
            BindUD(ZoneData[ZoneManager.JokerZone], mainMarket);
            if(ZoneManager.VoucherMarketZone.Cards.Count > 0)//TODO: Yup, here it is, gross special-case handling. What? U gonna fight me? HUh?????
            {
                BindUD(mainMarket, vouchMarket);
                BindLR(vouchMarket, packMarket);
            }else if(ZoneManager.PackMarketZone.Cards.Count > 0)
            {
                BindUD(mainMarket, packMarket);
            }
            /*BindUD(mainMarket, vouchMarket);
            BindLR(vouchMarket, packMarket);*/
            ZoneData.Add(ZoneManager.MainMarketZone, mainMarket);
            ZoneData.Add(ZoneManager.VoucherMarketZone, vouchMarket);
            ZoneData.Add(ZoneManager.PackMarketZone, packMarket);
        }

        private static void SetupPlayRoundZoneInfo()
        {
            SetupBasicZoneInfo();
            var handZoneData = new ZoneGridData(ZoneManager.HandZone);
            BindUD(ZoneData[ZoneManager.JokerZone], handZoneData);
            ZoneData.Add(ZoneManager.HandZone, handZoneData);
        }

        private static void SetupBlindSelectionZoneInfo()
        {
            SetupBasicZoneInfo();
            //TODO: TAGS?? BOSS BLIND??
        }

        private static void SetupBasicZoneInfo()
        {
            var jokerZoneData = new ZoneGridData(ZoneManager.JokerZone);
            var consumableData = new ZoneGridData(ZoneManager.ConsumableZone);
            BindLR(jokerZoneData, consumableData);
            ZoneData.Add(ZoneManager.JokerZone, jokerZoneData);
            ZoneData.Add(ZoneManager.ConsumableZone, consumableData);
        }

        private static void BindLR(ZoneGridData leftHandZone, ZoneGridData rightHandZone)
        {
            leftHandZone.RightHandZone = rightHandZone;
            rightHandZone.LeftHandZone = leftHandZone;
        }

        private static void BindUD(ZoneGridData upperZone, ZoneGridData lowerZone)
        {
            upperZone.BottomZone = lowerZone;
            lowerZone.TopZone = upperZone;
        }
    }
}
