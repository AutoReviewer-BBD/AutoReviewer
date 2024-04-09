using System;
using System.Diagnostics.CodeAnalysis;
using AutoCli.Commands;
using System.Text.Json;

namespace AutoCli {
    class Program {
        static async Task Main(string[] args) {

            Console.WriteLine("Autoreview intitiated! Enter a command.");

            while (true) {
                string input = Console.ReadLine();
                string command = inputs[0].ToLower();
                string[] inputs = input.Split(' ');

                else if (command == "help") {
                    HelpCommand.Run();
                }
                else if (command == "clear") {
                    ClearCommand.Run();
                }
                else if (command == "login") {
                    //To be implemented
                }
                else if (command == "history") {
                    if (accessToken != null) {
                        HistoryCommand.Run();
                    }
                    else {
                        Console.WriteLine("Unauthorized User. Login to view history");
                    }
                }
                else
                {
                    Console.WriteLine("invalid command. Try 'help'.");
                }
            }
        }
    }
}