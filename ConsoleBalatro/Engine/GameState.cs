using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.Engine
{
    public enum GameState
    {
        BASE,

        MainMenu,
        CollectionMenu,
        OptionsMenu,
        DeckSelectMenu,
        PlayRound,
        PostRoundRewardsMenu,
        BlindsMenu,
        ShopMenu,
        GameOverMenu,
        WinMenu,
        PauseMenu,

        SelectingPackOption,
    }
}
