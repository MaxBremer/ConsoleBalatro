using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Stakes;
using ConsoleBalatro.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBalatro.UI.EngineUI
{
    public class DeckChoicesDisplay : PanelDisplayEntity
    {
        private const int ArtPanelWidth = 25;
        private const int StakePanelWidth = 22;
        private const int ContentMargin = 2;
        private static readonly List<StakeType> StakeChoices = StakeManager.OfficialStakeOrder;

        public int SelectedDeckIndex { get; private set; } = 0;
        public int SelectedStakeIndex { get; private set; } = 0;

        private List<string> DeckNames => DeckDb.DeckDBNames;
        public string SelectedDeckName => DeckNames.Count == 0 ? string.Empty : DeckNames[SelectedDeckIndex];
        public StakeType SelectedStake => StakeChoices[SelectedStakeIndex];
        public string SelectedStakeName => SelectedStake.ToString();

        public DeckChoicesDisplay(int xLoc, int yLoc) : base(EngineDisplayConstants.DECK_CHOICE_DISPLAY_HEIGHT, EngineDisplayConstants.DECK_CHOICE_DISPLAY_WIDTH)
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }

        public bool CanSelectCurrentDeck => !string.IsNullOrEmpty(SelectedDeckName) && DeckDb.IsDeckUnlocked(SelectedDeckName);

        public bool CanSelectCurrentStake => !string.IsNullOrEmpty(SelectedDeckName) && UnlockManager.IsStakeUnlockedForDeck(SelectedDeckName, SelectedStake);

        public void SelectNextDeck()
        {
            MoveDeckSelection(1);
        }

        public void SelectPreviousDeck()
        {
            MoveDeckSelection(-1);
        }

        public void SelectNextStake()
        {
            MoveStakeSelection(1);
        }

        public void SelectPreviousStake()
        {
            MoveStakeSelection(-1);
        }

        public override void PreDisplaySetup()
        {
            ClampSelections();
            base.PreDisplaySetup();

            DrawHeader();
            DrawDeckArt();
            DrawDeckDetails();
            DrawStakePanel();
            DrawFooter();
        }

        private void MoveDeckSelection(int direction)
        {
            var deckCount = DeckNames.Count;
            if (deckCount == 0)
                return;

            SelectedDeckIndex = (SelectedDeckIndex + direction + deckCount) % deckCount;
        }

        private void MoveStakeSelection(int direction)
        {
            SelectedStakeIndex = (SelectedStakeIndex + direction + StakeChoices.Count) % StakeChoices.Count;
        }

        private void ClampSelections()
        {
            if (SelectedDeckIndex < 0 || SelectedDeckIndex >= DeckNames.Count)
                SelectedDeckIndex = 0;
            if (SelectedStakeIndex < 0 || SelectedStakeIndex >= StakeChoices.Count)
                SelectedStakeIndex = 0;
        }

        private void DrawHeader()
        {
            WriteCentered(1, "CHOOSE YOUR DECK");
            WriteLine(ContentMargin, 3, "Use Left/Right to browse decks. Up/Down are reserved for stake selection.");
        }

        private void DrawDeckArt()
        {
            var artX = ContentMargin;
            var artY = 5;
            DrawBox(artX, artY, ArtPanelWidth, 12, "DECK");

            var artLines = BuildDeckArtLines(SelectedDeckName, CanSelectCurrentDeck);
            for (int i = 0; i < artLines.Count && i < 8; i++)
            {
                WriteLine(artX + 2, artY + 2 + i, artLines[i], ArtPanelWidth - 4);
            }
        }

        private void DrawDeckDetails()
        {
            var detailX = ContentMargin + ArtPanelWidth + 2;
            var detailY = 5;
            var detailWidth = Width - ArtPanelWidth - StakePanelWidth - (ContentMargin * 2) - 4;
            DrawBox(detailX, detailY, detailWidth, 12, "EFFECT");

            if (string.IsNullOrEmpty(SelectedDeckName))
            {
                WriteLine(detailX + 2, detailY + 2, "No decks are available.", detailWidth - 4);
                return;
            }

            var data = DeckDb.DeckData[SelectedDeckName](null);
            var status = CanSelectCurrentDeck ? "UNLOCKED" : "LOCKED - not selectable";
            WriteLine(detailX + 2, detailY + 2, $"{data.JokerName} Deck", detailWidth - 4);
            WriteLine(detailX + 2, detailY + 3, status, detailWidth - 4);

            var descLines = WrapText(data.DescriptionBuilder(null), detailWidth - 4);
            for (int i = 0; i < descLines.Count && i < 5; i++)
            {
                WriteLine(detailX + 2, detailY + 5 + i, descLines[i], detailWidth - 4);
            }
        }

        private void DrawStakePanel()
        {
            var stakeX = Width - StakePanelWidth - ContentMargin;
            var stakeY = 5;
            DrawBox(stakeX, stakeY, StakePanelWidth, 12, "STAKES");
            WriteLine(stakeX + 2, stakeY + 2, "Current:", StakePanelWidth - 4);
            WriteLine(stakeX + 2, stakeY + 3, $"> {SelectedStakeName}", StakePanelWidth - 4);
            WriteLine(stakeX + 2, stakeY + 5, CanSelectCurrentStake ? "Playable" : "Locked", StakePanelWidth - 4);
            WriteLine(stakeX + 2, stakeY + 6, $"Beaten: {UnlockManager.GetStakesBeatenCountForDeck(SelectedDeckName)}", StakePanelWidth - 4);
            WriteLine(stakeX + 2, stakeY + 8, UnlockManager.HasDeckStakeSticker(SelectedDeckName, SelectedStake) ? "Sticker earned" : "No sticker yet", StakePanelWidth - 4);
            WriteLine(stakeX + 2, stakeY + 9, "Up/Down change", StakePanelWidth - 4);
        }

        private void DrawFooter()
        {
            var deckCount = DeckNames.Count;
            var positionText = deckCount == 0 ? "0 / 0" : $"{SelectedDeckIndex + 1} / {deckCount}";
            WriteLine(ContentMargin, Height - 3, $"<-- Previous    {positionText}    Next -->", Width - (ContentMargin * 2));
            var enterText = CanSelectCurrentDeck && CanSelectCurrentStake ? "Enter: start run with this deck/stake" : "Enter: locked deck/stake cannot be selected";
            WriteLine(ContentMargin, Height - 2, $"{enterText}    B/Escape: back", Width - (ContentMargin * 2));
        }

        private List<string> BuildDeckArtLines(string deckName, bool unlocked)
        {
            var name = string.IsNullOrEmpty(deckName) ? "?" : deckName;
            var middle = unlocked ? name : "LOCKED";
            return new List<string>
            {
                ".---------------.",
                "|  /---------\\  |",
                "| |           | |",
               $"| | {middle.PadRight(7).Substring(0, 7)}   | |",
                "| |           | |",
                "|  \\---------/  |",
                "'---------------'",
            };
        }

        private void DrawBox(int x, int y, int width, int height, string title)
        {
            for (int i = 0; i < width; i++)
            {
                SetTile(x + i, y, "-");
                SetTile(x + i, y + height - 1, "-");
            }
            for (int i = 0; i < height; i++)
            {
                SetTile(x, y + i, "|");
                SetTile(x + width - 1, y + i, "|");
            }
            WriteLine(x + 2, y, $" {title} ", width - 4);
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

        private void SetTile(int x, int y, string value)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                Sprite[y, x] = value;
        }

        private List<string> WrapText(string text, int width)
        {
            var ret = new List<string>();
            var words = (text ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var current = string.Empty;
            foreach (var word in words)
            {
                if (current.Length == 0)
                {
                    current = word;
                }
                else if (current.Length + word.Length + 1 <= width)
                {
                    current += " " + word;
                }
                else
                {
                    ret.Add(current);
                    current = word;
                }
            }
            if (!string.IsNullOrEmpty(current))
                ret.Add(current);
            return ret;
        }
    }
}
