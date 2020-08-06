using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Ganss.IO;

namespace Initium.Portal.Data
{
    class Program
    {
         
        static void Main(string[] args)
        {
            var types = new Dictionary<string, string>
            {
                {"Schemas", @"Database\Schemas\*.sql"},
                {"Tables", @"Database\**\Tables\*.sql"},
                {"Constraints", @"Database\**\Constraints\*.sql"},
                {"Indexes", @"Database\**\Indexes\*.sql"},
                {"Views", @"Database\**\Views\*.sql"},
            };

            foreach (var type in types)
            {
                Console.WriteLine($"-Starting Processing Of Type {type.Key}");
                foreach (var fileSystemInfo in Glob.Expand(Path.Combine(Environment.CurrentDirectory, type.Value)))
                {
                    Console.WriteLine($"--Processing Object {fileSystemInfo.Name}");
                    var d = DeployChanges.To
                            .PostgresqlDatabase(args[0])
                            .WithoutTransaction()
                            .WithScripts(new SqlScript(fileSystemInfo.Name, File.ReadAllText(fileSystemInfo.FullName)))
                        .JournalTo(new NullJournal())
                        .Build();

                    d.PerformUpgrade();
                }
                
                Console.WriteLine($"-Finishing Processing Of Type {type.Key}");
            }
        }
    }
}