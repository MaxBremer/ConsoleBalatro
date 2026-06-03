using ConsoleBalatro.Engine;
using ConsoleBalatro.Engine.Cards;
using ConsoleBalatro.Engine.Cards.Consumables;
using ConsoleBalatro.Engine.Cards.Enums;
using ConsoleBalatro.Engine.Cards.Jokers;
using ConsoleBalatro.Engine.Cards.Vouchers;
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
        public static Dictionary<string, Action<List<string>, List<string>>> Commands = new()
        {
            { "TEST", TestCommand },
            { "ADDCON", AddConsumable },
            { "ADDJOKER", AddJoker },
            { "ADDVOUCHER", AddVoucher },
            { "PRINT", PrintList },
            { "SETREQ", SetReq },
            { "HELP", Help },
            { "OP", Op },
            { "SETMONEY", SetMoney },
        };

        private static Dictionary<string, Func<CardZone>> CardZoneGetters = new()
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

        private static Dictionary<string, List<string>> GameStringLists = new()
        {
            { "TAROT", ConsumableManager.TarotNames },
            { "SPECTRAL", ConsumableManager.SpectralNames },
            { "PLANET", ConsumableManager.PlanetsToHandType.Keys.ToList() },
            { "JOKER", JokerDb.JokerDbNames },
        };
        
        public static void RunDebugCmdLine()
        {
            while(!QuitCmd)
            {
                Console.Write("DEBUG> ");
                var cmd = Console.ReadLine();
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
                Console.WriteLine($"Unknown command: {cmd}");
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
                    Console.WriteLine($"Invalid target index: {targetInd}");
                    return;
                }
                var targetZone = CardZoneGetters[zone]();
                if(targetIndex < 0 || targetIndex >= targetZone.Cards.Count)
                {
                    Console.WriteLine($"Target index out of range. Zone {zone} has {targetZone.Cards.Count} cards.");
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
                        Console.WriteLine($"Invalid edition: {operationParams[0]}");
                    }
                    break;
                case "SETENHANCE":
                    if(Enum.TryParse(operationParams[0], out Enhancement newEn))
                    {
                        c.SetEnhancementOfficial(newEn);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid enhancement: {operationParams[0]}");
                    }
                    break;
                case "ADDSTICKER":
                    //stickers are a bit more complex, so we'll just do add for now
                    if(Enum.TryParse(operationParams[0], out Sticker newSticker))
                    {
                        c.Stickers.Add(newSticker);
                    }
                    else
                    {
                        Console.WriteLine($"Invalid sticker: {operationParams[0]}");
                    }
                    break;
                case "SETSEAL":
                    if(Enum.TryParse(operationParams[0], out Seal newSeal))
                    {
                        c.Seal = newSeal;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid seal: {operationParams[0]}");
                    }
                    break;
                default:
                    Console.WriteLine($"Unknown operation: {operation}");
                    break;
            }
        }

        private static void Help(List<string> parameters, List<string> flags)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("TEST - A test command that prints parameters and flags.");
            Console.WriteLine("ADDCON <type> <dbName> [-ignorespace] - Adds a consumable card to the consumable zone. Use -ignorespace to add even if zone is full.");
            Console.WriteLine("ADDJOKER <jokerName> [-ignorespace] - Adds a joker card to the joker zone. Use -ignorespace to add even if zone is full.");
            Console.WriteLine("PRINT <listName> - Prints the contents of a predefined list. Valid list names are: " + string.Join(", ", GameStringLists.Keys));
            Console.WriteLine("SETREQ <amount> - Sets the required chips for the current blind during PlayRound state.");
            Console.WriteLine("QUIT or Q - Exits the debug command line.");
        }

        private static void SetReq(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                Console.WriteLine("Invalid number of parameters. Usage: setreq <amount>");
                return;
            }

            if(!int.TryParse(parameters[0], out int amount))
            {
                Console.WriteLine($"Invalid amount: {parameters[0]}");
                return;
            }

            if(Globals.CurrentGameState != GameState.PlayRound)
            {
                Console.WriteLine("Can only set required chips during PlayRound state.");
                return;
            }

            Globals.RequiredChipsForCurrentBlind = amount;
        }

        private static void SetMoney(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                Console.WriteLine("Invalid number of parameters. Usage: setmoney <amount>");
                return;
            }
            if(!int.TryParse(parameters[0], out int amount))
            {
                Console.WriteLine($"Invalid amount: {parameters[0]}");
                return;
            }
            Globals.EmitMoneyGain(amount - Globals.Money, null);
        }

        private static void PrintList(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                Console.WriteLine("Invalid number of parameters. Usage: print <listName>");
                return;
            }

            if(!GameStringLists.ContainsKey(parameters[0]))
            {
                var validListNames = string.Join(", ", GameStringLists.Keys);
                Console.WriteLine($"Unknown list name: {parameters[0]}. Valid list names are: {validListNames}");
                return;
            }

            var listToPrint = GameStringLists[parameters[0]];
            foreach(var ls in listToPrint)
            {
                Console.WriteLine(ls);
            }
        }

        private static void AddJoker(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 1)
            {
                Console.WriteLine("Invalid number of parameters. Usage: addjoker <jokerName>");
                return;
            }
            //NOTE: IF ADDING JOKER WITH SPACES, USE AMPERSAND INSTEAD OF SPACE IN THE NAME, E.G. "THE TRIBE" WOULD BE "THE&TRIBE"
            var jokerName = parameters[0].Replace("&", " ");
            if(!JokerDb.JokerDbNames.Contains(jokerName))
            {
                Console.WriteLine($"Unknown joker name: {jokerName}");
                return;
            }

            if (!ZoneManager.JokerZone.HasRoom && !flags.Contains("IGNORESPACE"))
            {
                Console.WriteLine("No room in joker zone to add card. Use -ignorespace flag to override.");
                return;
            }

            ZoneManager.JokerZone.AddCard(JokerDb.GenerateJokerCard(jokerName), overrideSpace: flags.Contains("IGNORESPACE"));
        }

        private static void AddVoucher(List<string> parameters, List<string> flags)
        {
            if (parameters.Count != 1)
            {
                Console.WriteLine("Invalid number of parameters. Usage: addvoucher <voucherName>");
                return;
            }

            //NOTE: IF ADDING VOUCHER WITH SPACES, USE AMPERSAND INSTEAD OF SPACE IN THE NAME, E.G. "5% OFF" WOULD BE "5%&OFF"
            var voucherName = parameters[0].Replace("&", " ");
            if (!VoucherDb.VoucherDBNames.Contains(voucherName))
            {
                Console.WriteLine($"Unknown voucher name: {voucherName}");
                return;
            }

            ZoneManager.ActiveVoucherZone.AddCard(VoucherDb.MakeVoucherCard(voucherName), overrideSpace: flags.Contains("IGNORESPACE"));
        }

        private static void AddConsumable(List<string> parameters, List<string> flags)
        {
            if(parameters.Count != 2)
            {
                Console.WriteLine("Invalid number of parameters. Usage: addcon <type> <dbName>");
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
                Console.WriteLine($"Invalid consumable type: {conType}. Valid types are: {validTypeNames}");
                return;
            }

            if(!ZoneManager.ConsumableZone.HasRoom && !flags.Contains("IGNORESPACE"))
            {
                Console.WriteLine("No room in consumable zone to add card. Use -ignorespace flag to override.");
                return;
            }

            switch (conType)
            {
                case "T":
                    if(!ConsumableManager.TarotNames.Contains(conDbName))
                    {
                        Console.WriteLine($"Invalid Tarot card name: {conDbName}");
                        return;
                    }
                    ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeTarotCard(conDbName), overrideSpace: flags.Contains("IGNORESPACE"));
                    break;
                case "S":
                    if(!ConsumableManager.SpectralNames.Contains(conDbName))
                    {
                        Console.WriteLine($"Invalid Spectral card name: {conDbName}");
                        return;
                    }
                    ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakeSpectralCard(conDbName), overrideSpace: flags.Contains("IGNORESPACE"));
                    break;
                case "P":
                    if(!ConsumableManager.PlanetsToHandType.Keys.Contains(conDbName))
                    {
                        Console.WriteLine($"Invalid Planet card name: {conDbName}");
                        return;
                    }
                    ZoneManager.ConsumableZone.AddCard(ConsumableManager.MakePlanetCard(conDbName), overrideSpace: flags.Contains("IGNORESPACE"));
                    break;
            }
        }

        private static void TestCommand(List<string> parameters, List<string> flags)
        {
            Console.WriteLine("Test command executed!");
            Console.WriteLine("Parameters: " + string.Join(", ", parameters));
            Console.WriteLine("Flags: " + string.Join(", ", flags));
        }
    }
}
