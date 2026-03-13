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
        PlayRound,
        PostRoundRewardsMenu,
        BlindsMenu,
        ShopMenu,
        GameOverMenu,
        PauseMenu,

        SelectingPackOption,
    }
}
