using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Blinds;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Jokers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBalatro.UI.EngineUI
{
    /// <summary>Browsable, read-only view of the items discovered in the current profile.</summary>
    public class CollectionDisplay : PanelDisplayEntity
    {
        private const int CardsPerPage = 20;
        private const int CardsPerRow = 10;
        private readonly string[] Categories = { "Jokers", "Tarot", "Planets", "Spectral", "Boss Blinds" };
        private List<Card> CurrentCards = new();

        public int SelectedCategoryIndex { get; private set; }
        public int SelectedCardIndex { get; private set; }
        public bool IsViewingCategory { get; private set; }
        public string SelectedCategory => Categories[SelectedCategoryIndex];

        public CollectionDisplay(int xLoc, int yLoc)
            : base(EngineDisplayConstants.MAIN_MENU_DISPLAY_HEIGHT, EngineDisplayConstants.MAIN_MENU_DISPLAY_WIDTH)
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }

        public void Reset()
        {
            IsViewingCategory = false;
            SelectedCardIndex = 0;
            CurrentCards.Clear();
        }

        public void SelectPreviousCategory() => SelectedCategoryIndex = Wrap(SelectedCategoryIndex - 1, Categories.Length);
        public void SelectNextCategory() => SelectedCategoryIndex = Wrap(SelectedCategoryIndex + 1, Categories.Length);

        public void EnterCategory()
        {
            CurrentCards = BuildCards(SelectedCategory).ToList();
            SelectedCardIndex = 0;
            IsViewingCategory = true;
        }

        /// <returns>True when the collection menu itself should be closed.</returns>
        public bool Back()
        {
            if (!IsViewingCategory)
                return true;

            IsViewingCategory = false;
            CurrentCards.Clear();
            return false;
        }

        public int IndOfJoker(string dbName) => CurrentCards.IndexOf(CurrentCards.FirstOrDefault(c => c.IsJoker && c.JokerData != null && c.JokerData.DBName == dbName) ?? new Card());

        public void SelectPreviousCard() => MoveCard(-1);
        public void SelectNextCard() => MoveCard(1);
        public void SelectCardAbove() => MoveCard(-CardsPerRow);
        public void SelectCardBelow() => MoveCard(CardsPerRow);

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();
            WriteCentered(2, IsViewingCategory ? $"COLLECTION - {SelectedCategory.ToUpper()}" : "COLLECTION");
            if (IsViewingCategory)
                DrawCards();
            else
                DrawCategories();
        }

        private void DrawCategories()
        {
            WriteCentered(4, $"Discovered: {UnlockManager.CollectionCount} items");
            for (var i = 0; i < Categories.Length; i++)
            {
                var count = GetNames(Categories[i]).Count;
                WriteCentered(7 + i * 2, $"{(i == SelectedCategoryIndex ? ">" : " ")} {Categories[i],-12} {count,3}");
            }
            WriteCentered(Height - 3, "Up/Down: choose    Enter: view    B/Escape: main menu");
        }

        private void DrawCards()
        {
            if (CurrentCards.Count == 0)
            {
                WriteCentered(9, "No items from this collection have been discovered yet.");
                WriteCentered(Height - 3, "B/Escape: collections");
                return;
            }

            var page = SelectedCardIndex / CardsPerPage;
            var first = page * CardsPerPage;
            var last = Math.Min(first + CardsPerPage, CurrentCards.Count);
            for (var i = first; i < last; i++)
            {
                var local = i - first;
                var display = new CardDisplay(CurrentCards[i]);
                display.SetDisplaySelectLevel(i == SelectedCardIndex ? 3 : 1);
                InsertOtherEntity(3 + (local % CardsPerRow) * 7, 5 + (local / CardsPerRow) * 7, display);
            }

            var detailX = 75;
            WriteLine(detailX, 5, $"{SelectedCardIndex + 1}/{CurrentCards.Count}  Page {page + 1}/{(CurrentCards.Count + CardsPerPage - 1) / CardsPerPage}");
            var detail = CurrentCards[SelectedCardIndex].DetailedInfoDisplay(null)
                .Replace(Card.CardInfoDoubleDivider, Card.CardInfoLineDivider);
            var detailLines = detail.Split(Card.CardInfoLineDivider);
            var y = 7;
            foreach (var line in detailLines.SelectMany(line => WrapText(line, Width - detailX - 2)))
            {
                if (y >= Height - 3) break;
                WriteLine(detailX, y++, line);
            }
            WriteLine(3, Height - 3, "Arrows: select    B/Escape: collections");
        }

        private void MoveCard(int offset)
        {
            if (CurrentCards.Count > 0)
                SelectedCardIndex = Wrap(SelectedCardIndex + offset, CurrentCards.Count);
        }

        private IEnumerable<Card> BuildCards(string category)
        {
            foreach (var name in GetNames(category))
            {
                Card? card = category switch
                {
                    "Jokers" => JokerDb.GenerateJokerCard(name),
                    "Tarot" => ConsumableManager.MakeTarotCard(name),
                    "Planets" => ConsumableManager.MakePlanetCard(name),
                    "Spectral" => ConsumableManager.MakeSpectralCard(name),
                    "Boss Blinds" => BossBlindDb.GenerateBlindCard(name),
                    _ => null,
                };
                if (card != null) yield return card;
            }
        }

        private IReadOnlyList<string> GetNames(string category)
        {
            if (category == "Jokers") return UnlockManager.CollectedJokerDbNames.ToList();
            if (category == "Boss Blinds") return UnlockManager.CollectedBossBlindDbNames.ToList();

            var collected = UnlockManager.CollectedConsumableDbNames;
            return category switch
            {
                "Tarot" => collected.Where(ConsumableManager.TarotConsumableDb.ContainsKey).ToList(),
                "Planets" => collected.Where(ConsumableManager.PlanetsToHandType.ContainsKey).ToList(),
                "Spectral" => collected.Where(ConsumableManager.SpectralConsumablesDb.ContainsKey).ToList(),
                _ => new List<string>(),
            };
        }

        private static IEnumerable<string> WrapText(string text, int width)
        {
            var remaining = text.Trim();
            while (remaining.Length > width)
            {
                var split = remaining.LastIndexOf(' ', width);
                if (split <= 0) split = width;
                yield return remaining.Substring(0, split);
                remaining = remaining.Substring(split).TrimStart();
            }
            if (remaining.Length > 0) yield return remaining;
        }

        private static int Wrap(int value, int count) => (value % count + count) % count;
        private void WriteCentered(int y, string text) => WriteLine(Math.Max(1, (Width - text.Length) / 2), y, text);
        private void WriteLine(int x, int y, string text) => InsertOtherStringDirect(x, y, text.Substring(0, Math.Min(text.Length, Width - x)));
    }
}
