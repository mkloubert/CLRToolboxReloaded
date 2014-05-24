﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox._Tests
{
    internal static class Program
    {
        #region Fields (1)

        private static object _SYNC = new object();

        #endregion Fields

        #region Methods (4)

        private static void InvokeConsole(Action action)
        {
            InvokeConsole(action, null);
        }

        private static void InvokeConsole(Action action, ConsoleColor? foreColor)
        {
            InvokeConsole(action, foreColor, null);
        }

        private static void InvokeConsole(Action action, ConsoleColor? foreColor, ConsoleColor? bgColor)
        {
            lock (_SYNC)
            {
                ConsoleColor oldForeColor = Console.ForegroundColor;
                ConsoleColor oldBgColor = Console.BackgroundColor;

                try
                {
                    if (foreColor.HasValue)
                    {
                        Console.ForegroundColor = foreColor.Value;
                    }

                    if (bgColor.HasValue)
                    {
                        Console.BackgroundColor = bgColor.Value;
                    }

                    action();
                }
                finally
                {
                    Console.ForegroundColor = oldForeColor;
                    Console.BackgroundColor = oldBgColor;
                }
            }
        }

        private static void Main(string[] args)
        {
            foreach (Type type in Assembly.GetExecutingAssembly()
                                          .GetTypes()
                                          .OrderBy(t => t.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                object[] testFixureAttribs = type.GetCustomAttributes(typeof(global::NUnit.Framework.TestFixtureAttribute), true);
                if (testFixureAttribs.Length < 1)
                {
                    continue;
                }

                object obj = Activator.CreateInstance(type);
                Console.WriteLine("{0} ...", obj.GetType().Name);

                foreach (MethodInfo method in obj.GetType()
                                                 .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                                 .OrderBy(m => m.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    object[] testAttribs = method.GetCustomAttributes(typeof(global::NUnit.Framework.TestAttribute), true);
                    if (testAttribs.Length < 1)
                    {
                        continue;
                    }

                    try
                    {
                        var ta = (TestAttribute)testAttribs[0];

                        Console.Write("\t'{0}' ... ", method.Name);

                        method.Invoke(obj, null);

                        InvokeConsole(() =>
                            {
                                Console.WriteLine("[OK]");
                            }, ConsoleColor.Green
                             , ConsoleColor.Black);
                    }
                    catch (Exception ex)
                    {
                        InvokeConsole(() =>
                            {
                                Console.WriteLine("[ERROR: {0}]",
                                                  (ex.GetBaseException() ?? ex).Message);
                            }, ConsoleColor.Red
                             , ConsoleColor.Black);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("===== ENTER =====");

            Console.ReadLine();
        }

        #endregion Methods
    }
}