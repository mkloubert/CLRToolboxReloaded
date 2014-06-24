﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Configuration;
using System;
using System.IO;

namespace MarcelJoachimKloubert.FileBox.Server
{
    internal static class Program
    {
        #region Methods (1)

        private static void Main(string[] args)
        {
            var configFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "config.json"));
            var conf = new JsonFileConfigRepository(file: configFile,
                                                    isReadOnly: true);

            using (var host = new FileBoxHost(config: conf))
            {
                host.Port = 23979;
                host.UseSecureConnections = false;

                if (host.IsInitialized == false)
                {
                    host.Initialize();
                }

                host.Start();

                Console.WriteLine("Running...");

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("===== ENTER =====");

                Console.ReadLine();
            }
        }

        #endregion Methods (1)
    }
}