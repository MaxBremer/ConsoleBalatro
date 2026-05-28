using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI
{
    public class CardDisplay : DisplayEntity
    {
        public const int CARD_HEIGHT = 5;
        public const int CARD_WIDTH = 5;
        public string CARD_DISP => CARD_TOP + CARD_SEGMENT + CARD_SEGMENT + CARD_SEGMENT + CARD_SEP + CARD_TOP;//"-----@|   |@|   |@|   |@-----";
        private string CARD_SEGMENT => CARD_SEP + CARD_WALL + "   " + CARD_WALL;
        public const string CARD_SEP = "@";
        public string CARD_WALL = "|";
        public string CARD_TOP = "-----";

        public const int PLAYING_CARD_SUIT_IND = 14;
        public const int PLAYING_CARD_RANK_IND = 7;

        private EngineEventListener MyListener;
        private EngineEventListener MyFlipListener;
        public CardDisplay(Card c) : base(CARD_HEIGHT, CARD_WIDTH)
        {
            MyCard = c;
        }

        public Card MyCard;

        public int GlobalX;
        public int GlobalY;

        public int SelectLevel = 1;

        public int CardSelectNumber = -1;

        public int PriceDisplay = -1;

        public bool isFlipped = false;

        public void AddListener()
        {
            MyListener = new EngineEventListener() { MyAction = OnCardDetailChange, MyContextType = EventContextType.CardDetailsChange };
            EngineEventHandler.StartListening(MyListener);
            MyFlipListener = new EngineEventListener() { MyAction = OnCardFlip, MyContextType = EventContextType.CardDetailsChange };
            EngineEventHandler.StartListening(MyFlipListener);
        }

        public void RemoveListener()
        {
            if(MyListener != null)
            {
                EngineEventHandler.StopListening(MyListener);
            }
            if (MyFlipListener != null)
            {
                EngineEventHandler.StopListening(MyFlipListener);
            }
        }

        //1 is no select, 2 is mid-select (i.e. part of played hand), 3 is full-select.
        public void SetDisplaySelectLevel(int lvl)
        {
            SelectLevel = lvl;
            if(lvl <= 1)
            {
                CARD_TOP = "-----";
            }else if(lvl == 2)
            {
                CARD_TOP = "_____";
            }
            else
            {
                CARD_TOP = "=====";
            }
            PreDisplaySetup();
        }

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();

            var tempDisplay = CARD_DISP;
            if(CardSelectNumber != -1)
            {
                tempDisplay = insertInto(tempDisplay, CardSelectNumber.ToString(), 26);
            }

            if (PriceDisplay != -1)
            {
                tempDisplay = insertInto(tempDisplay, "$" + PriceDisplay.ToString(), 0);
            }

            if (isFlipped)//if face down, close out here.
            {
                ImportFromString(tempDisplay, CARD_SEP);
                return;
            }

            if (MyCard.isJoker || MyCard.isVoucher)
            {
                var dispName = MyCard.JokerData.JokerName;
                if(dispName.Length > 3)
                {
                    if(dispName.StartsWith("The "))
                    {
                        dispName = dispName.Substring(4, 3);
                    }
                    else
                    {
                        dispName = dispName.Substring(0, 3);
                    }
                }
                tempDisplay = insertInto(tempDisplay, dispName, 13);
                tempDisplay = insertInto(tempDisplay, MyCard.isVoucher ? "V" : "J", 8);
            }else if (MyCard.isConsumable)
            {
                var dispName = MyCard.ConsumableData.ConsumableName;
                if(dispName.Length > 3)
                {
                    if(dispName.StartsWith("The "))//TODO: Replace this with a dict somewhere of strings for consumables.
                    {
                        dispName = dispName.Substring(4, 3);
                    }
                    else
                    {
                        dispName = dispName.Substring(0, 3);
                    }
                }
                tempDisplay = insertInto(tempDisplay, dispName, 13);
                tempDisplay = insertInto(tempDisplay, "C", 8);
            }else if (MyCard.isPack)
            {
                var topLine = ConsumableManager.PackBasicNums[MyCard.MyPackType].TopLine;
                var botLine = ConsumableManager.PackBasicNums[MyCard.MyPackType].BottomLine;
                tempDisplay = insertInto(tempDisplay, topLine, 7);
                tempDisplay = insertInto(tempDisplay, botLine, 13);
            }
            else
            {
                if(MyCard.Suit != Suit.NONE && MyCard.Rank != Rank.NONE)
                {
                    var suitStr = CardFactory.SuitToString[MyCard.Suit];
                    var rankStr = CardFactory.RankToString[MyCard.Rank];
                    if(MyCard.Enhancement != Enhancement.STONE)//TODO: do this a better way. idk what but a better way.
                    {
                        tempDisplay = insertInto(tempDisplay, suitStr, PLAYING_CARD_SUIT_IND);
                        tempDisplay = insertInto(tempDisplay, rankStr, PLAYING_CARD_RANK_IND);
                    }
                }
            }

            if(MyCard.Edition != Edition.BASE)
            {
                //If edition, give the corresponding edition border
                tempDisplay = tempDisplay.Replace("-", EngineDisplayGlobals.EditionBorderChars[MyCard.Edition]);
            }
            if(MyCard.Enhancement != Enhancement.NONE)
            {
                tempDisplay = EngineDisplayGlobals.EnhancementModifiers[MyCard.Enhancement](tempDisplay);
            }
            if(MyCard.Seal != Seal.NONE)
            {
                tempDisplay = EngineDisplayGlobals.SealModifiers[MyCard.Seal](tempDisplay);
            }
            if(MyCard.Stickers != null && MyCard.Stickers.Count > 0)
            {
                foreach (var s in MyCard.Stickers)
                {
                    tempDisplay = EngineDisplayGlobals.StickerModifiers[s](tempDisplay);
                }
            }
            if (MyCard.Debuffed)
            {
                tempDisplay = tempDisplay.Replace(CARD_WALL, "X");
            }
            ImportFromString(tempDisplay, CARD_SEP);
        }

        private string insertInto(string original, string newS, int ind)
        {
            if (newS.EndsWith("n"))
            {
                var x = 0;//I legitimately have no idea why this is here. Some debug from some era?
            }
            return original.Substring(0, ind) + newS + original.Substring(ind + newS.Length);
        }

        private void OnCardDetailChange(EngineEventArgs args)
        {
            if(args is EngineCardDetailsChangeArgs changeArgs && changeArgs.CardBeingChanged == MyCard && changeArgs.isAfter && !changeArgs.isFlip)
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    PreDisplaySetup();
                }, 0);
            }
        }

        private void OnCardFlip(EngineEventArgs args)
        {
            if (args is EngineCardDetailsChangeArgs changeArgs && changeArgs.CardBeingChanged == MyCard && changeArgs.isFlip && changeArgs.isAfter)
            {
                EngineDisplayGlobals.CacheAnimationAction(_ =>
                {
                    isFlipped = changeArgs.newFlipVal;
                    PreDisplaySetup();
                }, 0);
            }
        }
    }
}
