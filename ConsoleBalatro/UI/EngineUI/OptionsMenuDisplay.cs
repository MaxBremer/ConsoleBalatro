using ConsoleBalatro.Engine.Options;
using ConsoleBalatro.UI;
using System;
using System.Collections.Generic;

namespace ConsoleBalatro.UI.EngineUI
{
    public class OptionsMenuDisplay : PanelDisplayEntity
    {
        private const int Margin = 4;
        private const int OptionsStartY = 6;
        private const int OptionRowSpacing = 2;
        private const int VisibleOptionRows = 4;
        private readonly IReadOnlyList<GameOption> _options;

        public int SelectedIndex { get; private set; }
        public GameOption SelectedOption => _options[SelectedIndex];

        public OptionsMenuDisplay(int xLoc, int yLoc, IReadOnlyList<GameOption>? options = null)
            : base(EngineDisplayConstants.MAIN_MENU_DISPLAY_HEIGHT, EngineDisplayConstants.MAIN_MENU_DISPLAY_WIDTH)
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
            _options = options ?? GameOptions.All;

            if (_options.Count == 0)
                throw new ArgumentException("The options menu requires at least one option.", nameof(options));
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

            var page = SelectedIndex / VisibleOptionRows;
            var firstOption = page * VisibleOptionRows;
            var lastOption = Math.Min(firstOption + VisibleOptionRows, _options.Count);
            for (var i = firstOption; i < lastOption; i++)
            {
                var option = _options[i];
                var y = OptionsStartY + ((i - firstOption) * OptionRowSpacing);
                InsertOtherStringDirect(Margin, y, $"{(i == SelectedIndex ? '>' : ' ')} {option.Name}");
                InsertOtherStringDirect(55, y, $"< {option.DisplayValue} >");
            }

            var descriptionLabelY = OptionsStartY + (VisibleOptionRows * OptionRowSpacing);
            var pageCount = (_options.Count + VisibleOptionRows - 1) / VisibleOptionRows;
            InsertOtherStringDirect(Margin, descriptionLabelY, "DESCRIPTION");
            InsertOtherStringDirect(Width - 18, descriptionLabelY, $"Page {page + 1}/{pageCount}");
            WriteClipped(Margin, descriptionLabelY + 2, SelectedOption.Description);
            InsertOtherStringDirect(Margin, Height - 3, "Up/Down: choose    Left/Right/Enter: change    B/Escape: back");
        }

        private void MoveSelection(int direction)
        {
            SelectedIndex = (SelectedIndex + direction + _options.Count) % _options.Count;
        }

        private void WriteClipped(int x, int y, string text) =>
            InsertOtherStringDirect(x, y, text.Length <= Width - x - Margin
                ? text
                : text.Substring(0, Width - x - Margin - 3) + "...");

        private void WriteCentered(int y, string text) =>
            InsertOtherStringDirect(Math.Max(1, (Width - text.Length) / 2), y, text);
    }
}
