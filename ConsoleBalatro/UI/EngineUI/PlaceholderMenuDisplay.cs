using ConsoleBalatro.UI;
using System;

namespace ConsoleBalatro.UI.EngineUI
{
    public class PlaceholderMenuDisplay : PanelDisplayEntity
    {
        public string Title { get; set; } = "COMING SOON";
        public string Body { get; set; } = "This menu will be implemented later.";

        public PlaceholderMenuDisplay(int xLoc, int yLoc) : base(EngineDisplayConstants.MAIN_MENU_DISPLAY_HEIGHT, EngineDisplayConstants.MAIN_MENU_DISPLAY_WIDTH)
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();
            WriteCentered(4, Title);
            WriteCentered(8, Body);
            WriteCentered(12, "Placeholder menu");
            WriteCentered(Height - 3, "B/Escape: back to main menu");
        }

        private void WriteCentered(int y, string text)
        {
            InsertOtherStringDirect(Math.Max(1, (Width - text.Length) / 2), y, text);
        }
    }
}
