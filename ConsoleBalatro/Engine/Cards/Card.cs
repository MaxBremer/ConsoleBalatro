using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Tags;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Cards
{
    public class Card
    {
        private static int CardIdCount = 1;

        public const string CardInfoLineDivider = "$%";
        public static string CardInfoDoubleDivider => CardInfoLineDivider + CardInfoLineDivider;

        private bool _isSelected = false;
        private Enhancement _enhancement = Enhancement.NONE;
        private Rank _rank = Rank.NONE;
        private Suit _suit = Suit.NONE;

        public int ID;

        public Edition Edition = Edition.BASE;
        public Seal Seal = Seal.NONE;
        public int ChipsBase = 0;
        //NOTE: By default game rules, the below 2 are always 0 for playing cards. Editions and enhancements are handled by a global listener.
        //This is only for later/"modded" features, or for some jokers.
        //Maybe base jimbo joker? Things like that? ...are there any other things like that? Cavendish/banana?
        public double MultBase = 0;
        public double MultMultBase = 0;
        public CardZone MyZone = null;
        public bool Flipped = false;
        public bool Debuffed = false;
        public bool isJoker => JokerData != null && JokerData.isJoker;
        public bool isVoucher => JokerData != null && JokerData.isVoucher;
        public bool isTag => JokerData != null && JokerData.isTag;
        public JokerCardDataBlock JokerData = null;

        public PackType MyPackType = PackType.NONE;
        public bool isPack => MyPackType != PackType.NONE;


        public bool isConsumable => ConsumableData != null;
        public ConsumableCardDataBlock ConsumableData = null;

        public int BaseCost = 1; //Default cost of playing card.
        public int? BuyCostOverride = null;
        public int BuyCost => BuyCostOverride ?? (int)((BaseCost + EngineUtils.EditionCostIncreases[Edition]) * Globals.DiscountMultiplier);
        public int SellCost => Math.Max((BuyCost / 2) + BonusSellValue, 1);
        public int BonusSellValue = 0;

        public Card()
        {
            ID = CardIdCount;
            CardIdCount += 1;
            DataManager.TrackCard(this);
        }

        public bool isSelected
        {
            get => _isSelected;
            set
            {
                var evContext = new EventContext() { Context = EventContextType.CardSelect };
                var eventArgs = new EngineCardSelectedArgs() { MyContext = evContext, isNowSelected = value, wasPreviouslySelected = _isSelected, TargetedCard = this };
                _isSelected = value;
                EngineEventHandler.TriggerEvent(eventArgs);
            }
        }

        public Rank Rank
        {
            get
            {
                if(Enhancement == Enhancement.STONE)
                {
                    return Rank.NONE;
                }
                else
                {
                    return _rank;
                }
            }
            set
            {
                _rank = value;
            }
        }

        public Suit Suit
        {
            get
            {
                if(Enhancement == Enhancement.STONE)
                {
                    return Suit.NONE;
                }
                else
                {
                    return _suit;
                }
            }
            set
            {
                _suit = value;
            }
        }

        public Enhancement Enhancement
        {
            get => _enhancement;
            set
            {
                var old_enhancement = _enhancement;
                _enhancement = value;
                if(_enhancement == Enhancement.STONE)
                {
                    ChipsBase = 50;//ISNT THIS BAD?? SHOULDNT THIS BE A LISTENER?? TODO
                }else if(old_enhancement == Enhancement.STONE)
                {
                    SetChipsFromRank();
                }
            }
        }
        public Action<EventContext> TriggerCardForScoring;
        public List<Sticker> Stickers = new();

        public void SetEditionOfficial(Edition ed)
        {
            var oldEd = Edition;
            var evArgs = new EngineCardDetailsChangeArgs()
            {
                CardBeingChanged = this,
                isEditionChange = true,
                OldEdition = oldEd,
                NewEdition = ed,
                MyContext = new() { Context = EventContextType.CardDetailsChange},
            };
            EngineEventHandler.TriggerEvent(evArgs);
            Edition = ed;

            evArgs.isAfter = true;
            EngineEventHandler.TriggerEvent(evArgs);
        }

        public void SetEnhancementOfficial(Enhancement en)
        {
            var oldEn = Enhancement;
            var evArgs = new EngineCardDetailsChangeArgs()
            {
                CardBeingChanged = this,
                isEnhancementChange = true,
                OldEnhancement = oldEn,
                NewEnhancement = en,
                MyContext = new() { Context = EventContextType.CardDetailsChange },
            };
            EngineEventHandler.TriggerEvent(evArgs);
            Enhancement = en;

            evArgs.isAfter = true;
            EngineEventHandler.TriggerEvent(evArgs);
        }

        public void SetRankOfficial(Rank newRank)
        {
            Rank = newRank;//TODO: event??
            ChipsBase = EngineUtils.RankBaseChipAmounts[Rank];
        }

        public void SetSuitOfficial(Suit newSuit)
        {
            var oldSuit = Suit;
            var evArgs = new EngineCardDetailsChangeArgs()
            {
                CardBeingChanged = this,
                isSuitChange = true,
                OldSuit = oldSuit,
                NewSuit = newSuit,
                MyContext = new() { Context = EventContextType.CardDetailsChange },
            };
            EngineEventHandler.TriggerEvent(evArgs);
            Suit = newSuit;

            evArgs.isAfter = true;
            EngineEventHandler.TriggerEvent(evArgs);
        }

        public void ToggleSelect()
        {
            if (!isSelected && ZoneManager.HandZone.Cards.Contains(this) && Globals.CurNumCardsSelected >= Globals.SelectionMax)
                return;//SHOULD THIS ALSO BE HANDLED BY A LISTENER? EV THAT CHECKS FOR OBJECTIONS?

            isSelected = !isSelected;
        }

        public void ClearExtras()
        {
            Enhancement = Enhancement.NONE;
            Seal = Seal.NONE;
            Edition = Edition.BASE;
            Stickers.Clear();
            SetChipsFromRank();
        }

        public void SetChipsFromRank()
        {
            if(Rank != Rank.NONE)
                ChipsBase = EngineUtils.RankBaseChipAmounts[Rank];
        }

        //We do it like this cause cards can be multiple suits, i.e. smeared joker or wild cards.
        public List<Suit> GetCardSuits()
        {
            var ret = new List<Suit>();

            if(Enhancement == Enhancement.WILD)//TODO: GET RID OF THIS. SEE NOTE BELOW. MAKE A GLOBAL LISTENER INSTEAD. MUCH BETTER PRACTICE.
            {
                ret = new List<Suit>() { Suit.HEARTS, Suit.CLUBS, Suit.SPADES, Suit.DIAMONDS };
            }
            else if(Suit != Suit.NONE)
            {
                ret.Add(Suit);
            }

            var args = new EngineCardSuitPullArgs()//so that modifications can occur, i.e. smeared joker adding suits.
            {
                MyContext = new() { Context = EventContextType.CardSuitPull},
                CardBeingPulled = this,
                SuitsBeingReturned = ret,
            };//TODO: WILD SHOULD BE IN GLOBAL LISTENERS USING THIS
            EngineEventHandler.TriggerEvent(args);
            return args.SuitsBeingReturned;
        }

        public bool IsSuit(Suit possibleSuit) => GetCardSuits().Contains(possibleSuit);

        public void TriggerScoring(ScoringContext context)
        {
            if (Debuffed)
            {
                //TODO: prob some kind of event to show it tried to trigger but was debuffed? For the one joker that gives money?
                return;
            }

            var preTriggerArgs = new EngineCardPreTriggerArgs()
            {
                MyContext = new() { Context = EventContextType.CardPreTrigger, ScoringContext = context },
                CardAboutToTrigger = this,
            };
            EngineEventHandler.TriggerEvent(preTriggerArgs);
            for (int i = 0; i < preTriggerArgs.numTriggersToDo; i++)
            {
                if (ChipsBase > 0)
                    Globals.EmitChipsAdd(ChipsBase, this);
                if (MultBase > 0)
                    Globals.EmitMultAdd(MultBase, this);
                if(MultMultBase > 0)
                    Globals.EmitMultMult(MultMultBase, this);

                var myTriggerArgs = new EngineCardTriggerArgs()
                {
                    MyContext = new() { Context = EventContextType.CardTrigger, ScoringContext = context },
                    CardThatIsTriggering = this,
                    isScoringTrigger = true,
                    HandCurrentlyBeingPlayed = context.HandBeingPlayed,
                };

                EngineEventHandler.TriggerEvent(myTriggerArgs);
            }
        }

        public void TriggerInHandDuringScoring(ScoringContext context)
        {
            var preTriggerArgs = new EngineCardPreTriggerArgs()
            {
                MyContext = new() { Context = EventContextType.CardPreTrigger, ScoringContext = context },
                CardAboutToTrigger = this,
                isInHandPostScoringTrigger = true,
            };
            EngineEventHandler.TriggerEvent(preTriggerArgs);

            for (int i = 0; i < preTriggerArgs.numTriggersToDo; i++)
            {
                var myTriggerArgs = new EngineCardTriggerArgs()
                {
                    MyContext = new() { Context = EventContextType.CardTrigger, ScoringContext = context },
                    CardThatIsTriggering = this,
                    isInHandPostScoringTrigger = true,
                };
                EngineEventHandler.TriggerEvent(myTriggerArgs);
            }
        }

        public void TurnIntoCopyOfMe(Card target)
        {
            target.SetRankOfficial(_rank);
            target.SetSuitOfficial(_suit);

            target.ChipsBase = ChipsBase;
            target.MultBase = MultBase;
            target.MultMultBase = MultMultBase;

            target.Stickers.Clear();
            target.Stickers.AddRange(Stickers);

            target.SetEditionOfficial(Edition);
            target.Seal = Seal;
            target.SetEnhancementOfficial(Enhancement);

            //NOTE: Deciding here and now that joker, voucher, and tag are exclusive. A card can only be 0 to 1 of these things, not multiple.
            if (isJoker)
            {
                var jName = JokerData.DBName;
                JokerDb.MakeCardJoker(target, jName);
                JokerData.CopyDataDictTo(target.JokerData);
            }else if (isVoucher)
            {
                var vName = JokerData.DBName;
                VoucherDb.MakeCardVoucher(target, vName);
                JokerData.CopyDataDictTo(target.JokerData);
            }else if (isTag)
            {
                var tType = JokerData.TagData.MyType;
                TagDb.MakeCardTagOfType(target, tType);
                JokerData.CopyDataDictTo(target.JokerData);
            }
            if (isConsumable)
            {
                var cName = ConsumableData.DBName;
                switch (ConsumableData.Type)
                {
                    case ConsumableType.TAROT:
                        ConsumableManager.MakeCardTarotCard(cName, target);
                        break;
                    case ConsumableType.PLANET:
                        ConsumableManager.MakeCardPlanetCard(ConsumableData.PlanetHandType, target);
                        break;
                    case ConsumableType.SPECTRAL:
                        ConsumableManager.MakeCardSpectralCard(cName, target);
                        break;
                    default:
                        break;
                }
                ConsumableData.CopyDataDictTo(target.ConsumableData);
            }
            target.MyPackType = MyPackType;

            target.BaseCost = BaseCost;
            target.BuyCostOverride = BuyCostOverride;
            target.BonusSellValue = BonusSellValue;

            var evArgs = new EngineCardDetailsChangeArgs()
            {
                CardBeingChanged = target,
                isAfter = true,
                MyContext = new() { Context = EventContextType.CardDetailsChange },
            };
            EngineEventHandler.TriggerEvent(evArgs);//Honest to god don't know what this is for. Individual changes will trigger events on their own. Who knows.
        }

        public Card MakeCopy()
        {
            var ret = new Card();
            TurnIntoCopyOfMe(ret);
            return ret;
        }

        public void DestroyCard()
        {
            return;//FOR NOW, DONT ACTUALLY DO ANYTHING TO DESTROY A CARD. MAYBE LATER IDK.
            //TODO: CLEAN UP.
            //tbh idk if this actually does anything, but it makes me feel better.
            if(isJoker || isVoucher)
            {
                JokerData.DataDict.Clear();
                JokerData.OnJokerGainEffs.Clear();
                JokerData.OnJokerRemovalEffs.Clear();
                JokerData = null;
            }
            if (isConsumable)
            {
                ConsumableData.DataDict.Clear();
                ConsumableData = null;
            }
        }

        //TODO: BELOW ARE DISPLAY FUNCTIONS.
        //SHOULD NOT BE HERE IN ENGINE. SHOULD BE IN CARDDISPLAY.
        //Just leave behind some basic print for debugging or logging or something.
        public string PlayingCardBasicDisplay()
        {
            var ret = Rank.ToString() + " of " + Suit.ToString() + "\n";
            ret += "+" + ChipsBase + " Chips";
            return ret;
        }

        private string GetModifiersText()
        {
            var ret = "";

            if(Enhancement != Enhancement.NONE)
            {
                ret += "Enhancement: " + Enhancement.ToString() + "\n";
            }
            if(Edition != Edition.BASE)
            {
                ret += "Edition: " + Edition.ToString() + "\n";
            }
            if(Seal != Seal.NONE)
            {
                ret += "Seal: " + Seal.ToString() + "\n";
            }
            if(Stickers.Count > 0)
            {
                ret += "Stickers: ";
                foreach(var stick in Stickers)
                {
                    ret += stick.ToString() + ", ";
                }
                ret += "\n";
            }

            return ret;
        }

        public string DetailedInfoDisplay(EventContext context)
        {
            var retStr = "";
            //TODO: Below conditional precludes some weird modded cases, like being a joker and consumable or pack and consumable at the same time.
            //Prime example is playing card and consumable, which I know happens.
            if(isJoker || isVoucher)
            {
                retStr += JokerData.JokerName + CardInfoDoubleDivider;
                retStr += JokerData.DescriptionBuilder(context) + CardInfoDoubleDivider;
                if (isJoker)
                {
                    retStr += JokerData.Rarity.ToString();
                }
            }
            else if (isConsumable)
            {
                retStr += ConsumableData.ConsumableName + CardInfoDoubleDivider;
                retStr += ConsumableData.DescriptionBuilder(context);
            }else if(isPack && ConsumableManager.PackBasicNums.ContainsKey(MyPackType))
            {
                var packData = ConsumableManager.PackBasicNums[MyPackType];
                retStr += packData.PackName + CardInfoDoubleDivider;
                retStr += "Choose " + packData.NumCanBeTaken + " of " + packData.NumOptionsPresented + " " + packData.GetItemTypeString();
            }else if (Rank != Rank.NONE && Suit != Suit.NONE)
            {
                retStr = PlayingCardBasicDisplay().Replace("\n", CardInfoLineDivider);
            }
            else
            {
                retStr = "ERROR: Card unrecognized";
            }

            if (!retStr.Contains("ERROR"))
            {
                retStr += CardInfoLineDivider + GetModifiersText().Replace("\n", CardInfoLineDivider);
            }

            return retStr;
        }
    }
}
