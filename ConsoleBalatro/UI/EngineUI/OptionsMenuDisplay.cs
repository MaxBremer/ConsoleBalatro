using ConsoleBalatro.Engine.Options;
using ConsoleBalatro.UI;
using System;

namespace ConsoleBalatro.UI.EngineUI
{
    public class OptionsMenuDisplay : PanelDisplayEntity
    {
        private const int Margin = 4;
        public int SelectedIndex { get; private set; }
        public GameOption SelectedOption => GameOptions.All[SelectedIndex];

        public OptionsMenuDisplay(int xLoc, int yLoc)
            : base(EngineDisplayConstants.MAIN_MENU_DISPLAY_HEIGHT, EngineDisplayConstants.MAIN_MENU_DISPLAY_WIDTH)
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }

        public void SelectNext() => MoveSelection(1);
        public void SelectPrevious() => MoveSelection(-1);
        public void ChangeSelected(int direction) => SelectedOption.Change(direction);

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();
            WriteCentered(2, "OPTIONS");
            InsertOtherStringDirect(Margin, 4, "SETTING");
            InsertOtherStringDirect(55, 4, "VALUE");

            for (var i = 0; i < GameOptions.All.Count; i++)
            {
                var option = GameOptions.All[i];
                var y = 6 + (i * 2);
                InsertOtherStringDirect(Margin, y, $"{(i == SelectedIndex ? '>' : ' ')} {option.Name}");
                InsertOtherStringDirect(55, y, $"< {option.DisplayValue} >");
            }

            InsertOtherStringDirect(Margin, 16, "DESCRIPTION");
            InsertOtherStringDirect(Margin, 18, SelectedOption.Description);
            InsertOtherStringDirect(Margin, Height - 3, "Up/Down: choose    Left/Right/Enter: change    B/Escape: back");
        }

        private void MoveSelection(int direction)
        {
            SelectedIndex = (SelectedIndex + direction + GameOptions.All.Count) % GameOptions.All.Count;
        }

        private void WriteCentered(int y, string text) =>
            InsertOtherStringDirect(Math.Max(1, (Width - text.Length) / 2), y, text);
    }
}
