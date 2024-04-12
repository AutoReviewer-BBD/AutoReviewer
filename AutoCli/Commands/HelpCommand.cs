using System;
using System.Collections.Generic;

namespace AutoCli.Commands {
    public class HelpCommand {
        
        public static List<CommandDTO> helpItems = new List<CommandDTO>() {
            new CommandDTO { name = "login", description = "This allows you to login with Github"},
            new CommandDTO { name = "history", description = "Returns a list of PRs you have made using Autoreviewer"},
            new CommandDTO { name = "help", description = "This shows you the help menu"},
            new CommandDTO { name = "clear", description = "This clears the console" },
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