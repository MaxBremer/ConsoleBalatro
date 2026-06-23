using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBalatro.UI.EngineUI
{
    public class DeckViewDisplay : PanelDisplayEntity
    {
        private const int CardSpacingX = CardDisplay.CARD_WIDTH + 1;
        private const int CardSpacingY = CardDisplay.CARD_HEIGHT + 1;
        private const int CardsPerRow = 14;
        private const int RowsPerPage = 3;
        private const int CardsPerPage = CardsPerRow * RowsPerPage;

        private List<Card> CardsToDisplay = new();
        private readonly Dictionary<Card, CardDisplay> CardDisplays = new();
        private readonly Dictionary<Card, CardDisplay> PreviousGlobalDisplays = new();

        public int PageIndex { get; private set; }
        public int SelectedCardIndex { get; private set; }
        public bool IsCurrentStakeView { get; private set; }
        public bool CanToggleView { get; private set; }

        public DeckViewDisplay() : base(24, 90)
        {
            xLoc = 1;
            yLoc = 3;
            zSortOrder = 500;
            ClearBg = false;
            Visible = false;
        }

        public void Show(bool canToggleView)
        {
            CanToggleView = canToggleView;
            IsCurrentStakeView = canToggleView;
            PageIndex = 0;
            SelectedCardIndex = 0;
            Visible = true;
            RefreshCards();
        }

        public void Hide()
        {
            foreach (var display in CardDisplays.Values)
            {
                display.SetDisplaySelectLevel(1);
                display.RemoveListener();
            }

            foreach (var card in CardDisplays.Keys.ToList())
            {
                if (EngineDisplayGlobals.GlobalCardDisplays.TryGetValue(card, out var display) && display == CardDisplays[card])
                    EngineDisplayGlobals.GlobalCardDisplays.Remove(card);

                if (PreviousGlobalDisplays.TryGetValue(card, out var previousDisplay))
                    EngineDisplayGlobals.GlobalCardDisplays[card] = previousDisplay;
            }

            CardDisplays.Clear();
            PreviousGlobalDisplays.Clear();
            CardsToDisplay.Clear();
            Visible = false;
        }

        public void ToggleView()
        {
            if (!CanToggleView)
                return;

            IsCurrentStakeView = !IsCurrentStakeView;
            PageIndex = 0;
            SelectedCardIndex = 0;
            RefreshCards();
        }

        public void SelectPreviousCard()
        {
            if (CardsToDisplay.Count == 0)
                return;

            SelectedCardIndex = Math.Max(0, SelectedCardIndex - 1);
            EnsureSelectionOnPage();
            UpdateSelectionDisplay();
        }

        public void SelectNextCard()
        {
            if (CardsToDisplay.Count == 0)
                return;

            SelectedCardIndex = Math.Min(CardsToDisplay.Count - 1, SelectedCardIndex + 1);
            EnsureSelectionOnPage();
            UpdateSelectionDisplay();
        }

        public void PreviousPage()
        {
            if (PageIndex <= 0)
                return;

            PageIndex--;
            SelectedCardIndex = PageIndex * CardsPerPage;
            UpdateSelectionDisplay();
        }

        public void NextPage()
        {
            if (PageIndex >= PageCount - 1)
                return;

            PageIndex++;
            SelectedCardIndex = PageIndex * CardsPerPage;
            UpdateSelectionDisplay();
        }

        public Card SelectedCard => CardsToDisplay.Count == 0 ? null : CardsToDisplay[SelectedCardIndex];

        public override void PreDisplaySetup()
        {
            base.PreDisplaySetup();

            var title = IsCurrentStakeView ? "DECK VIEW - CURRENT STAKE (SORTED)" : "DECK VIEW - FULL RUN DECK";
            InsertOtherStringDirect(2, 1, title);
            InsertOtherStringDirect(2, 2, $"Cards: {CardsToDisplay.Count}   Page {PageIndex + 1}/{Math.Max(1, PageCount)}");

            var controls = CanToggleView
                ? "[Left/Right] Card  [Up/Down] Page  [D] Details  [F] Full/Current  [Esc/B] Close"
                : "[Left/Right] Card  [Up/Down] Page  [D] Details  [Esc/B] Close";
            InsertOtherStringDirect(2, Height - 2, controls);

            var pageCards = CardsToDisplay.Skip(PageIndex * CardsPerPage).Take(CardsPerPage).ToList();
            for (int i = 0; i < pageCards.Count; i++)
            {
                var card = pageCards[i];
                var row = i / CardsPerRow;
                var col = i % CardsPerRow;
                InsertOtherEntity(2 + col * CardSpacingX, 4 + row * CardSpacingY, CardDisplays[card]);
            }
        }

        private int PageCount => CardsToDisplay.Count == 0 ? 1 : (int)Math.Ceiling(CardsToDisplay.Count / (double)CardsPerPage);

        private void RefreshCards()
        {
            HideExistingDisplaysOnly();

            var sourceCards = IsCurrentStakeView
                ? ZoneManager.DeckZone.Cards.ToList()
                : ZoneManager.GetFullDeckPlayingCards();

            CardsToDisplay = sourceCards
                .OrderByDescending(x => x.Suit)
                .ThenByDescending(x => x.Rank)
                .ToList();

            foreach (var card in CardsToDisplay)
            {
                var display = new CardDisplay(card);
                display.PreDisplaySetup();
                display.AddListener();
                CardDisplays[card] = display;
                if (EngineDisplayGlobals.GlobalCardDisplays.TryGetValue(card, out var previousDisplay))
                    PreviousGlobalDisplays[card] = previousDisplay;
                EngineDisplayGlobals.GlobalCardDisplays[card] = display;
            }

            UpdateSelectionDisplay();
        }

        private void HideExistingDisplaysOnly()
        {
            foreach (var display in CardDisplays.Values)
                display.RemoveListener();

            foreach (var card in CardDisplays.Keys.ToList())
            {
                if (EngineDisplayGlobals.GlobalCardDisplays.TryGetValue(card, out var display) && display == CardDisplays[card])
                    EngineDisplayGlobals.GlobalCardDisplays.Remove(card);

                if (PreviousGlobalDisplays.TryGetValue(card, out var previousDisplay))
                    EngineDisplayGlobals.GlobalCardDisplays[card] = previousDisplay;
            }

            CardDisplays.Clear();
            PreviousGlobalDisplays.Clear();
        }

        private void EnsureSelectionOnPage()
        {
            PageIndex = SelectedCardIndex / CardsPerPage;
        }

        private void UpdateSelectionDisplay()
        {
            foreach (var kvp in CardDisplays)
                kvp.Value.SetDisplaySelectLevel(kvp.Key == SelectedCard ? 3 : 1);
        }
    }
}
