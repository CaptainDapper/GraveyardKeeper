﻿using QModInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QModManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArgs = new Dictionary<string, string>();
            bool forceInstall = false;
            bool forceUninstall = false;

            foreach (var arg in args)
            {
                if (arg.Contains("="))
                {
                    parsedArgs = args.Select(s => s.Split(new[] { '=' }, 1)).ToDictionary(s => s[0], s => s[1]);
                }
                else if (arg.StartsWith("-"))
                {
                    if (arg == "-i")
                        forceInstall = true;

                    if (arg == "-u")
                        forceUninstall = true;
                }
            }

            string GraveyardKeeperDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Graveyard Keeper";
            string ManagedDirectory = Path.Combine(GraveyardKeeperDirectory, @"\Graveyard Keeper_Data\Managed");

            if (parsedArgs.Keys.Contains("GraveyardKeeperDirectory"))
                GraveyardKeeperDirectory = parsedArgs["GraveyardKeeperDirectory"];

            Logger.StartNewLog(Path.Combine(GraveyardKeeperDirectory, @"\QMods\output_log.txt"));
            Logger.WriteLog("\n" + DateTime.Now + "\n");

            Logger.WriteLog("DEBUG: Instantiated QModInjector");
            QModInjector injector = new QModInjector(GraveyardKeeperDirectory);

            Logger.WriteLog("DEBUG: Finished constructing QModInjector");

            bool isInjected = injector.IsPatcherInjected();
            if (forceInstall)
            {
                if (!isInjected)
                {
					Console.WriteLine("Installing QMods...");
					injector.Inject();
                }
                else
                {
                    Console.WriteLine("Tried to Force Install, was already injected. Skipping installation.");
                    return;
                }
            }
            else if (forceUninstall)
            {
                if (isInjected)
                {
					Console.WriteLine("Uninstalling QMods...");
					injector.Remove();
                }
                else
                {
                    Console.WriteLine("Tried to Force Uninstall, was not injected. Skipping uninstallation.");
                    return;
                }
            }
            else
            {
                if (!injector.IsPatcherInjected())
                {
                    Console.WriteLine("No patch detected, type 'yes' to install: ");
                    string consent = Console.ReadLine().Replace("'", "");
                    if (consent == "yes" || consent == "YES")
                    {
                        if (injector.Inject())
                            Console.WriteLine("QMods was installed!");
                        else
                            Console.WriteLine("Error installed QMods. Please contact us on Discord");
                    }
                }
                else
                {
                    Console.WriteLine("Patch already installed! Type 'yes' to remove: ");
                    string consent = Console.ReadLine().Replace("'", "");
                    if (consent == "yes" || consent == "YES")
                    {
                        if (injector.Remove())
                            Console.WriteLine("QMods was removed!");
                        else
                            Console.WriteLine("Error removing QMods. Please contact us on Discord");
                    }
                }

                Console.WriteLine("Press any key to exit ...");

                Console.ReadKey();
            }
        }
    }
}
