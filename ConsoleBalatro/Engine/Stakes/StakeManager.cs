using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using ConsoleBalatro.Engine.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine.Stakes
{
    public static class StakeManager
    {
        public static List<StakeType> OfficialStakeOrder = new()
        {
            StakeType.WHITE,
            StakeType.RED,//no small blind money
            StakeType.GREEN,//faster req scaling
            StakeType.BLACK,//30% chance shop/pack jokers to be eternal
            StakeType.BLUE,//-1 discard
            StakeType.PURPLE,//faster req scaling
            StakeType.ORANGE,//30% chance shop/pack jokers to be perishable
            StakeType.GOLD,//30% chance pack/shop jokers to be rental
        };

        public static StakeType CurrentStake = StakeType.WHITE;

        //White stake has nothing cause it does nothing.
        public static Dictionary<StakeType, Func<Card, JokerCardDataBlock>> StakeBuilders = new()
        {
            { StakeType.RED, c => 
            {
                var ret = JokerDb.BasicDataBlock("Red Stake", "Small blind gives no money.");

                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.GatherPostRoundMoney,
                    MyAction = args =>
                    {
                        if(args is EngineGatherPostRoundMoneyArgs moneyArgs && FlowHandler.CurrentSelectedBlind == BlindType.SMALL)
                        {
                            moneyArgs.ExistingSources.RemoveAll(x => x.Item1.ToUpper() == "BLIND");
                        }
                    }
                });

                return ret;
            } },
            {StakeType.GREEN, c => JokerDb.BasicDataBlock("Green Stake", "Faster ante scaling.") },
            {StakeType.BLACK, c =>
            {
                var ret = JokerDb.BasicDataBlock("Black Stake", "Jokers can be Eternal.");

                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.RolledCardGenerated,
                    MyAction = args =>
                    {
                        if(args is EngineCardRollGeneratedArgs rollArgs && rollArgs.RollRequest.Pool == ItemPool.Joker && (rollArgs.RollRequest.Source == GenerationSource.Market || rollArgs.RollRequest.Source == GenerationSource.Pack))
                        {
                            if(Globals.ChooseRandomInclusive(1, 3) == 1)//separate line for readability
                                rollArgs.FinalCardRolled.AddSticker(Sticker.ETERNAL);
                        }
                    }
                });

                return ret;
            } },
            {StakeType.BLUE, c =>
            {
                var ret = JokerDb.BasicDataBlock("Blue Stake", "-1 Discard.");

                ret.OnJokerGainEffs.Add(() =>
                {
                    Globals.MaxDiscardsPerRound -= 1;
                });

                return ret;
            } },
            {StakeType.PURPLE, c => JokerDb.BasicDataBlock("Purple Stake", "Even faster ante scaling.") },
            {StakeType.ORANGE, c =>
            {
                var ret = JokerDb.BasicDataBlock("Orange Stake", "Jokers can be Perishable.");

                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.RolledCardGenerated,
                    MyAction = args =>
                    {
                        if(args is EngineCardRollGeneratedArgs rollArgs && rollArgs.RollRequest.Pool == ItemPool.Joker && (rollArgs.RollRequest.Source == GenerationSource.Market || rollArgs.RollRequest.Source == GenerationSource.Pack))
                        {
                            if(Globals.ChooseRandomInclusive(1, 3) == 1)//separate line for readability
                                rollArgs.FinalCardRolled.AddSticker(Sticker.PERISHABLE);
                        }
                    }
                });

                return ret;
            } },
            {StakeType.GOLD, c =>
            {
                var ret = JokerDb.BasicDataBlock("Gold Stake", "Jokers can be Rental.");

                ret.Listeners.Add(new EngineEventListener()
                {
                    MyContextType = EventContextType.RolledCardGenerated,
                    MyAction = args =>
                    {
                        if(args is EngineCardRollGeneratedArgs rollArgs && rollArgs.RollRequest.Pool == ItemPool.Joker && (rollArgs.RollRequest.Source == GenerationSource.Market || rollArgs.RollRequest.Source == GenerationSource.Pack))
                        {
                            if(Globals.ChooseRandomInclusive(1, 3) == 1)//separate line for readability
                                rollArgs.FinalCardRolled.AddSticker(Sticker.RENTAL);
                        }
                    }
                });

                return ret;
            } },
        };

        public static bool StakeActive(StakeType stakeType)
        {
            return CurrentStake == stakeType || OfficialStakeOrder.IndexOf(stakeType) <= OfficialStakeOrder.IndexOf(CurrentStake);
        }

        public static void SetStake(StakeType stakeType)
        {
            CurrentStake = stakeType;
            AddStakeEffectsThrough(stakeType);
        }

        private static void AddStakeEffectsThrough(StakeType stakeType)
        {
            var stakeIndex = OfficialStakeOrder.IndexOf(stakeType);
            if(stakeIndex <= 0)
            {
                return;
            }

            for (var i = stakeIndex; i > 0; i--)
            {
                var stakeEffectType = OfficialStakeOrder[i];
                var cToAdd = new Card();
                cToAdd.JokerData = StakeBuilders[stakeEffectType](cToAdd);
                ZoneManager.AddHiddenEffect(cToAdd);
            }
        }
    }
}
