using ConsoleBalatro.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBalatro.UI.EngineUI
{
    public class MainMenuDisplay : PanelDisplayEntity
    {
        private const int ContentMargin = 2;
        private readonly List<string> MenuOptions = new()
        {
            "Start Run",
            "Collection",
            "Options",
        };

        public int SelectedIndex { get; private set; } = 0;
        public string SelectedOption => MenuOptions[SelectedIndex];

        public MainMenuDisplay(int xLoc, int yLoc) : base(EngineDisplayConstants.MAIN_MENU_DISPLAY_HEIGHT, EngineDisplayConstants.MAIN_MENU_DISPLAY_WIDTH)
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }

        public void SelectNextOption()
        {
            MoveSelection(1);
        }

        public void SelectPreviousOption()
        {
            MoveSelection(-1);
        }

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();

            DrawTitle();
            DrawJokerArt();
            DrawMenuOptions();
            DrawFooter();
        }

        private void MoveSelection(int direction)
        {
            SelectedIndex = (SelectedIndex + direction + MenuOptions.Count) % MenuOptions.Count;
        }

        private void DrawTitle()
        {
            WriteCentered(2, "    _    ____   ____ ___ ___   ____    _    _        _  _____ ____   ___  ");
            WriteCentered(3, "   / \\  / ___| / ___|_ _|_ _| | __ )  / \\  | |      / \\|_   _|  _ \\ / _ \\ ");
            WriteCentered(4, "  / _ \\ \\___ \\| |    | | | |  |  _ \\ / _ \\ | |     / _ \\ | | | |_) | | | |");
            WriteCentered(5, " / ___ \\ ___) | |___ | | | |  | |_) / ___ \\| |___ / ___ \\| | |  _ <| |_| |");
            WriteCentered(6, "/_/   \\_\\____/ \\____|___|___| |____/_/   \\_\\_____/_/   \\_\\_| |_| \\_\\___/ ");
        }

        private void DrawJokerArt()
        {
            var artLines = new List<string>
            {
                ".----------------.",
                "|J               |",
                "|   .-''''''-.   |",
                "|  /  o    o  \\  |",
                "| |      ^     | |",
                "| |   \\___/   | |",
                "|  \\  .---.  /  |",
                "|   '-.____.-'   |",
                "|             JOK|",
                "'----------------'",
            };

            var startX = ContentMargin + 10;
            var startY = 9;
            for (int i = 0; i < artLines.Count; i++)
            {
                WriteLine(startX, startY + i, artLines[i]);
            }
        }

        private void DrawMenuOptions()
        {
            var menuX = Width / 2 + 8;
            var menuY = 11;
            WriteLine(menuX, menuY - 2, "MAIN MENU");
            for (int i = 0; i < MenuOptions.Count; i++)
            {
                var prefix = i == SelectedIndex ? ">" : " ";
                WriteLine(menuX, menuY + (i * 2), $"{prefix} {MenuOptions[i]}");
            }
        }

        private void DrawFooter()
        {
            WriteLine(ContentMargin, Height - 3, "Up/Down: choose    Enter: select", Width - (ContentMargin * 2));
            WriteLine(ContentMargin, Height - 2, "ASCII Balatro: not Balatro on a console, Balatro in your console.", Width - (ContentMargin * 2));
        }

        private void WriteCentered(int y, string text)
        {
            WriteLine(Math.Max(1, (Width - text.Length) / 2), y, text);
        }

        private void WriteLine(int x, int y, string text, int? maxWidth = null)
        {
            if (y < 0 || y >= Height)
                return;

            var trimmed = text ?? string.Empty;
            if (maxWidth.HasValue && trimmed.Length > maxWidth.Value)
                trimmed = trimmed.Substring(0, maxWidth.Value);
            InsertOtherStringDirect(x, y, trimmed);
        }
    }
}
