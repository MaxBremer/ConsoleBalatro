using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Events;
using ConsoleBalatro.Engine.Events.Args;
using Xunit;

namespace ConsoleBalatro.Tests;

public class UnlockManagerTests : TestClassBase
{
    [Fact]
    public void DeckUnlocks_SaveAndLoad_WithDefaultDecks()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);

            Assert.True(DeckDb.IsDeckUnlocked("RED"));
            Assert.False(DeckDb.IsDeckUnlocked("BLUE"));

            Assert.True(DeckDb.UnlockDeck("BLUE"));
            Assert.True(File.Exists(savePath));

            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            Assert.False(DeckDb.IsDeckUnlocked("BLUE"));

            Assert.True(UnlockManager.LoadProgress());
            Assert.True(DeckDb.IsDeckUnlocked("RED"));
            Assert.True(DeckDb.IsDeckUnlocked("BLUE"));
        }
        finally
        {
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            DeleteTempSave(savePath);
        }
    }

    [Fact]
    public void AchievementListener_AchievesOnce_SavesAndDoesNotRelistenAfterLoad()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        var conditionChecks = 0;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            UnlockManager.RegisterAchievementListener(
                "PLAY_A_HAND",
                EventContextType.HandPlayDone,
                _ =>
                {
                    conditionChecks++;
                    return true;
                });

            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });

            Assert.True(UnlockManager.IsAchievementAchieved("PLAY_A_HAND"));
            Assert.Equal(1, conditionChecks);
            Assert.True(File.Exists(savePath));

            UnlockManager.LoadProgress();
            EngineEventHandler.TriggerEvent(new EngineEventArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });

            Assert.True(UnlockManager.IsAchievementAchieved("PLAY_A_HAND"));
            Assert.Equal(1, conditionChecks);
        }
        finally
        {
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }


    [Fact]
    public void BuiltInAchievements_TriggerFromDifferentEvents()
    {
        var savePath = BuildTempSavePath();
        var originalPath = UnlockManager.SaveFilePath;
        try
        {
            UnlockManager.SaveFilePath = savePath;
            EngineEventHandler.ResetFullEventHandler();
            UnlockManager.ResetProgressToDefaults();
            EngineEventHandler.ResetSavedEvents();

            for (var i = 0; i < 9; i++)
            {
                EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            }
            Assert.False(UnlockManager.IsAchievementAchieved(UnlockManager.TenHandsPlayedAchievementId));

            EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs { MyContext = new EventContext { Context = EventContextType.HandPlayDone } });
            Assert.True(UnlockManager.IsAchievementAchieved(UnlockManager.TenHandsPlayedAchievementId));

            EngineEventHandler.TriggerEvent(new EngineHandPlayDoneArgs
            {
                MyContext = new EventContext { Context = EventContextType.HandPlayDone },
                CurrentTotalChips = 10000,
            });
            Assert.True(UnlockManager.IsAchievementAchieved(UnlockManager.ScoreTenThousandAchievementId));

            EngineEventHandler.TriggerEvent(new EngineDiscardDoneArgs
            {
                MyContext = new EventContext { Context = EventContextType.HandDiscardDone },
                BeingDiscarded = CardFactory.CardListFromDefString("AS,KS,QS,JS,1S", ","),
            });
            Assert.True(UnlockManager.IsAchievementAchieved(UnlockManager.DiscardRoyalFlushAchievementId));
        }
        finally
        {
            UnlockManager.SaveFilePath = originalPath;
            UnlockManager.ResetProgressToDefaults(clearAchievementDefinitions: true);
            EngineEventHandler.ResetFullEventHandler();
            DeleteTempSave(savePath);
        }
    }

    private static string BuildTempSavePath()
    {
        return Path.Combine(Path.GetTempPath(), "ConsoleBalatroTests", $"unlocks-{Guid.NewGuid():N}.json");
    }

    private static void DeleteTempSave(string savePath)
    {
        var directory = Path.GetDirectoryName(savePath);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        if (File.Exists(savePath + ".tmp"))
        {
            File.Delete(savePath + ".tmp");
        }
        if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
        {
            Directory.Delete(directory);
        }
    }
}
