using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Decks;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
using ConsoleBalatro.Engine.Stakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBalatro.UI.EngineUI.Controls
{
    public static class DebugManager
    {
        private static bool QuitCmd = false;

        //pass in params, flags
        public static readonly Dictionary<string, Action<List<string>, List<string>>> Commands = new()
        {
            { "TEST", TestCommand },
            { "ADDCON", AddConsumable },
            { "ADDJOKER", AddJoker },
            { "ADDVOUCHER", AddVoucher },
            { "PRINT", PrintList },
            { "SETREQ", SetReq },
            { "SETANTE", SetAnte },
            { "HELP", Help },
            { "OP", Op },
            { "SETMONEY", SetMoney },
            { "PERMAPROGRESS", PermanentProgress },
            { "UNLOCKDECK", UnlockDeck },
        };

        private static readonly Dictionary<string, Func<CardZone>> CardZoneGetters = new()
        {
            { "CONSUMABLE", () => ZoneManager.ConsumableZone },
            { "JOKER", () => ZoneManager.JokerZone },
            { "DECK", () => ZoneManager.DeckZone },
            { "DISCARD", () => ZoneManager.DiscardZone },
            { "HAND", () => ZoneManager.HandZone },
            { "MAINMARKET", () => ZoneManager.MainMarketZone  },
            { "VOUCHMARKET", () => ZoneManager.VoucherMarketZone },
            { "PACKMARKET", () => ZoneManager.PackMarketZone },
            { "PACKOPTION", () => ZoneManager.PackOptionZone  },
        };

        private static readonly Dictionary<string, List<string>> GameStringLists = new()
        {
            { "TAROT", ConsumableManager.TarotNames },
            { "SPECTRAL", ConsumableManager.SpectralNames },
            { "PLANET", ConsumableManager.PlanetsToHandType.Keys.ToList() },
            { "JOKER", JokerDb.JokerDbNames },
            { "DECK", DeckDb.DeckDBNames },
            { "STAKE", StakeManager.OfficialStakeOrder.Select(x => x.ToString()).ToList() },
        };

        private static readonly List<string> HelpLines = new()
        {
            "Available commands:",
            "TEST - A test command that prints passed parameters and flags.",
            "ADDCON <type> <dbName> [-ignorespace] - Adds a consumable card to the consumable zone. Use -ignorespace to add even if zone is full.",
            "ADDJOKER <jokerName> [-ignorespace] - Adds a joker card to the joker zone. Use -ignorespace to add even if zone is full.",
            "PRINT <listName> - Prints the contents of a predefined list. Valid list names are: " + string.Join(", ", GameStringLists.Keys),
            "SETREQ <amount> - Sets the required chips for the current blind during PlayRound state.",
            "SETANTE <amount> - Sets the current ante.",
            "HELP - prints available commands. You ran it to get this list, dummy.",
            "OP <opName> <targetZone> <targetInd|ALL> <opParams...> - Performs the passed operation, with passed params, on the card(s) specified by targetZone and targetInd. For more info, use help -op",
            "SETMONEY <amount> - Sets the players money to the passed amount.",
            "PERMAPROGRESS <enable|disable|status> - Toggles whether permanent progress saves are written.",
            "UNLOCKDECK <deckName> [stakeName] [-beaten] - Unlocks a deck, or unlocks stakes for that deck up to stakeName. Use -beaten to also award the stakeName sticker.",
            "QUIT or Q - Exits the debug command line.",
        };
        
        public static void RunDebugCmdLine()
        {
            while(!QuitCmd)
            {
                ConsoleWrite("DEBUG> ");
                var cmd = ControlManager.ReadLine();
                if(cmd == null)
                    continue;
                if(cmd.ToUpper() == "QUIT" || cmd.ToUpper() == "Q")
                {
                    QuitCmd = true;
                    continue;
                }
                ReadCommand(cmd);
            }
            QuitCmd = false;
        }

        public static void ReadCommand(string command)
        { 
            var args = command.Split(' ');
            var cmd = args[0].ToUpper();
            if(!Commands.ContainsKey(cmd))
            {
                ConsoleWriteLine($"Unknown command: {cmd}");
                return;
            }

            var flags = new List<string>();
            var parameters = new List<string>();
            foreach(var arg in args.Skip(1))
            {
                if(arg.StartsWith("-"))
                    flags.Add(arg.Replace("-", "").ToUpper());
                else
                    parameters.Add(arg.ToUpper());
            }

            Commands[cmd](parameters, flags);
        }

        public static void ConsoleWrite(string s) => Console.Write(s);

        public static void ConsoleWriteLine(string s) => Console.WriteLine(s);

        private static void Op(List<string> parameters, List<string> flags)
        {
            var operation = parameters[0].ToUpper();
            var zone = parameters[1].ToUpper();
            var targetInd = parameters[2];
            var operationParams = parameters.Skip(3).ToList();

            if(targetInd == "ALL")
            {
                foreach(var c in CardZoneGetters[zone]().Cards)
                {
                    PerformOperation(c, operation, operationParams);
                }
            }
            else
            {
                if(!int.TryParse(targetInd, out int targetIndex))
                {
                    ConsoleWriteLine($"Invalid target index: {targetInd}");
                    return;
                }
                var targetZone = CardZoneGetters[zone]();
                if(targetIndex < 0 || targetIndex >= targetZone.Cards.Count)
                {
                    ConsoleWriteLine($"Target index out of range. Zone {zone} has {targetZone.Cards.Count} cards.");
                    return;
                }
                var targetCard = targetZone.Cards[targetIndex];
                PerformOperation(targetCard, operation, operationParams);
            }
        }

        private static void PerformOperation(Card c, string operation, List<string> operationParams)
        {
            switch(operation)
            {
                case "SETED":
                    if(Enum.TryParse(operationParams[0], out Edition newEd))
                    {
                        c.SetEditionOfficial(newEd);
                    }
                    else
                    {
                        ConsoleWriteLine($"Invalid edition: {operationParams[0]}");
                    }
                    break;
                case "SETENHANCE":
                    if(Enum.TryParse(operationParams[0], out Enhancement newEn))
                    {
                        c.SetEnhancementOfficial(newEn);
                    }
                    else
                    {
                        ConsoleWriteLine($"Invalid enhancement: {operationParams[0]}");
                    }
                    break;
                case "ADDSTICKER":
                    //stickers are a bit more complex, so we'll just do add for now
                    if(Enum.TryParse(operationParams[0], out Sticker newSticker))
                    {
                        c.AddSticker(newSticker);
                    }
                    else
                    {
                        ConsoleWriteLine($"Invalid sticker: {operationParams[0]}");
                    }
                    break;
                case "SETSEAL":
                    if(Enum.TryParse(operationParams[0], out Seal newSeal))
                    {
                        c.SetSealOfficial(newSeal);
                    }
                    else
                    {
                        ConsoleWriteLine($"Invalid seal: {operationParams[0]}");
                    }
                    break;
                default:
                    ConsoleWriteLine($"Unknown operation: {operation}");
                    break;
            }
        }

        private static void Help(List<string> parameters, List<string> flags)
        {
            if (flags.Contains("OP"))
            {
                ConsoleWriteLine("Haven't implemented this yet lol, no op help 4 u");
                return;
            }
            foreach (var l in HelpLines)
                ConsoleWriteLine(l);
        }

        private static void SetReq(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: setreq <amount>");
                return;
            }

            if(!int.TryParse(parameters[0], out int amount))
            {
                ConsoleWriteLine($"Invalid amount: {parameters[0]}");
                return;
            }

            if(Globals.CurrentGameState != GameState.PlayRound)
            {
                ConsoleWriteLine("Can only set required chips during PlayRound state.");
                return;
            }

            Globals.RequiredChipsForCurrentBlind = amount;
        }

        private static void SetAnte(List<string> parameters, List<string> flags)
        {
            if (parameters.Count != 1)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: setante <amount>");
                return;
            }

            if (!int.TryParse(parameters[0], out int amount))
            {
                ConsoleWriteLine($"Invalid amount: {parameters[0]}");
                return;
            }

            FlowHandler.CurrentAnte = amount;
        }

        private static void UnlockDeck(List<string> parameters, List<string> flags)
        {
            if (parameters.Count < 1 || parameters.Count > 2)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: unlockdeck <deckName> [stakeName] [-beaten]");
                return;
            }

            var deckName = parameters[0].Replace("&", " ").ToUpper();
            if (!DeckDb.DeckData.ContainsKey(deckName))
            {
                ConsoleWriteLine($"Unknown deck name: {deckName}");
                return;
            }

            StakeType? targetStake = null;
            if (parameters.Count == 2)
            {
                if (!Enum.TryParse(parameters[1], ignoreCase: true, out StakeType parsedStake)
                    || !StakeManager.OfficialStakeOrder.Contains(parsedStake))
                {
                    ConsoleWriteLine($"Invalid stake: {parameters[1]}. Valid stakes are: {string.Join(", ", StakeManager.OfficialStakeOrder)}");
                    return;
                }
                targetStake = parsedStake;
            }

            var deckWasUnlocked = DeckDb.UnlockDeck(deckName, saveImmediately: false);
            var progressChanged = false;

            if (targetStake.HasValue)
            {
                var targetStakeIndex = StakeManager.OfficialStakeOrder.IndexOf(targetStake.Value);
                var highestStakeIndexToMarkBeaten = flags.Contains("BEATEN") ? targetStakeIndex : targetStakeIndex - 1;
                for (var i = 0; i <= highestStakeIndexToMarkBeaten; i++)
                {
                    progressChanged |= UnlockManager.MarkDeckStakeBeaten(deckName, StakeManager.OfficialStakeOrder[i], saveImmediately: false);
                }
            }

            if (deckWasUnlocked || progressChanged)
            {
                UnlockManager.SaveProgress();
            }

            var highestBeaten = UnlockManager.GetHighestBeatenStakeForDeck(deckName)?.ToString() ?? "NONE";
            var highestPlayable = StakeManager.OfficialStakeOrder.LastOrDefault(stake => UnlockManager.IsStakeUnlockedForDeck(deckName, stake)).ToString();
            ConsoleWriteLine($"{deckName} deck unlocked. Highest playable stake: {highestPlayable}. Highest sticker: {highestBeaten}.");
        }

        private static void PermanentProgress(List<string> parameters, List<string> flags)
        {
            if (parameters.Count != 1)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: permaprogress <enable|disable|status>");
                return;
            }

            switch (parameters[0])
            {
                case "ENABLE":
                case "ON":
                    UnlockManager.PermanentProgressSavingDisabled = false;
                    ConsoleWriteLine("Permanent progress saving is ENABLED.");
                    break;
                case "DISABLE":
                case "OFF":
                    UnlockManager.PermanentProgressSavingDisabled = true;
                    ConsoleWriteLine("Permanent progress saving is DISABLED. Achievements, collections, and unlocks will still occur but will not be saved.");
                    break;
                case "STATUS":
                    var status = UnlockManager.PermanentProgressSavingDisabled ? "DISABLED" : "ENABLED";
                    ConsoleWriteLine($"Permanent progress saving is {status}.");
                    break;
                default:
                    ConsoleWriteLine($"Unknown permanent progress option: {parameters[0]}. Usage: permaprogress <enable|disable|status>");
                    break;
            }
        }

        private static void SetMoney(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: setmoney <amount>");
                return;
            }
            if(!int.TryParse(parameters[0], out int amount))
            {
                ConsoleWriteLine($"Invalid amount: {parameters[0]}");
                return;
            }
            Globals.EmitMoneyGain(amount - Globals.Money, null);
        }

        private static void PrintList(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: print <listName>");
                return;
            }

            if(!GameStringLists.ContainsKey(parameters[0]))
            {
                var validListNames = string.Join(", ", GameStringLists.Keys);
                ConsoleWriteLine($"Unknown list name: {parameters[0]}. Valid list names are: {validListNames}");
                return;
            }

            var listToPrint = GameStringLists[parameters[0]];
            foreach(var ls in listToPrint)
            {
                ConsoleWriteLine(ls);
            }
        }

        private static void AddJoker(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: addjoker <jokerName>");
                return;
            }
            //NOTE: IF ADDING JOKER WITH SPACES, USE AMPERSAND INSTEAD OF SPACE IN THE NAME, E.G. "THE TRIBE" WOULD BE "THE&TRIBE"
            var jokerName = parameters[0].Replace("&", " ");
            if(!JokerDb.JokerDbNames.Contains(jokerName))
            {
                ConsoleWriteLine($"Unknown joker name: {jokerName}");
                return;
            }

            if (!ZoneManager.JokerZone.HasRoom && !flags.Contains("IGNORESPACE"))
            {
                ConsoleWriteLine("No room in joker zone to add card. Use -ignorespace flag to override.");
                return;
            }

            ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard(jokerName), overrideSpace: flags.Contains("IGNORESPACE"));
        }

        private static void AddVoucher(List<string> parameters, List<string> flags)
        {
            if (parameters.Count != 1)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: addvoucher <voucherName>");
                return;
            }

            //NOTE: IF ADDING VOUCHER WITH SPACES, USE AMPERSAND INSTEAD OF SPACE IN THE NAME, E.G. "5% OFF" WOULD BE "5%&OFF"
            var voucherName = parameters[0].Replace("&", " ");
            if (!VoucherDb.VoucherDBNames.Contains(voucherName))
            {
                ConsoleWriteLine($"Unknown voucher name: {voucherName}");
                return;
            }

            ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard(voucherName), overrideSpace: flags.Contains("IGNORESPACE"));
        }

        private static void AddConsumable(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 2)
            {
                ConsoleWriteLine("Invalid number of parameters. Usage: addcon <type> <dbName>");
                return;
            }
            var conType = parameters[0].ToUpper();
            var conDbName = parameters[1].ToUpper();

            var validTypes = new List<string> { "T", "S", "P" };
            var typeNames = new Dictionary<string, string>
            {
                { "T", "Tarot" },
                { "S", "Spectral" },
                { "P", "Planet" }
            };
            if (!validTypes.Contains(conType))
            {
                var validTypeNames = string.Join(", ", validTypes.Select(t => $"{t} ({typeNames[t]})"));
                ConsoleWriteLine($"Invalid consumable type: {conType}. Valid types are: {validTypeNames}");
                return;
            }

            if(!ZoneManager.ConsumableZone.HasRoom && !flags.Contains("IGNORESPACE"))
            {
                ConsoleWriteLine("No room in consumable zone to add card. Use -ignorespace flag to override.");
                return;
            }

            switch (conType)
            {
                case "T":
                    if(!ConsumableManager.TarotNames.Contains(conDbName))
                    {
                        ConsoleWriteLine($"Invalid Tarot card name: {conDbName}");
                        return;
                    }
                    ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard(conDbName), overrideSpace: flags.Contains("IGNORESPACE"));
                    break;
                case "S":
                    if(!ConsumableManager.SpectralNames.Contains(conDbName))
                    {
                        ConsoleWriteLine($"Invalid Spectral card name: {conDbName}");
                        return;
                    }
                    ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeSpectralCard(conDbName), overrideSpace: flags.Contains("IGNORESPACE"));
                    break;
                case "P":
                    if(!ConsumableManager.PlanetsToHandType.Keys.Contains(conDbName))
                    {
                        ConsoleWriteLine($"Invalid Planet card name: {conDbName}");
                        return;
                    }
                    ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakePlanetCard(conDbName), overrideSpace: flags.Contains("IGNORESPACE"));
                    break;
            }
        }

        private static void TestCommand(List<string> parameters, List<string> flags)
        {
            ConsoleWriteLine("Test command executed!");
            ConsoleWriteLine("Parameters: " + string.Join(", ", parameters));
            ConsoleWriteLine("Flags: " + string.Join(", ", flags));
        }
    }
}
