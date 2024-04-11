using System;
using System.Collections.Generic;

namespace AutoCli.Commands {
    public class HelpCommand {
        
        public static List<CommandDTO> helpItems = new List<CommandDTO>() {
            new CommandInfo { name = "login", description = "This allows you to login with Github"},
            new CommandInfo { name = "history", description = "Returns a list of PRs you have made using Autoreviewer"},
            new CommandInfo { name = "help", description = "This shows you the help menu"},
            new CommandInfo { name = "clear", description = "This clears the console" },
        };

        public static void Run() {
            foreach (var helpItems in commands) {
                Console.WriteLine("Here is a list of the available commands");
                Console.WriteLine($"command: {command.name} -> {command.description}");
                Console.WriteLine("____________________________________________________________________");
                Console.WriteLine("\n");
            }
            
        }
    }
}