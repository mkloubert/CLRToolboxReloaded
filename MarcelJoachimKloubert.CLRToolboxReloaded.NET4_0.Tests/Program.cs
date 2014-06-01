// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

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
                var oldForeColor = Console.ForegroundColor;
                var oldBgColor = Console.BackgroundColor;

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
            foreach (var type in Assembly.GetExecutingAssembly()
                                         .GetTypes()
                                         .Where(t => t.IsPublic &&
                                                     (t.IsAbstract == false))
                                         .OrderBy(t => t.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                if (type.GetCustomAttributes(typeof(global::NUnit.Framework.IgnoreAttribute), true)
                        .Any())
                {
                    continue;
                }

                if (type.GetCustomAttributes(typeof(global::NUnit.Framework.TestFixtureAttribute), true)
                        .Any() == false)
                {
                    continue;
                }

                var obj = Activator.CreateInstance(type);
                Console.WriteLine("{0} ...", obj.GetType().Name);

                var allMethods = obj.GetType()
                                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                    .OrderBy(m => m.Name, StringComparer.InvariantCultureIgnoreCase);

                var fixtureSetupMethod = allMethods.SingleOrDefault(m => m.GetCustomAttributes(typeof(global::NUnit.Framework.TestFixtureSetUpAttribute), true)
                                                                          .Any());
                var fixtureTearDownMethod = allMethods.SingleOrDefault(m => m.GetCustomAttributes(typeof(global::NUnit.Framework.TestFixtureTearDownAttribute), true)
                                                                             .Any());

                var setupMethod = allMethods.SingleOrDefault(m => m.GetCustomAttributes(typeof(global::NUnit.Framework.SetUpAttribute), true)
                                                                   .Any());
                var tearDownMethod = allMethods.SingleOrDefault(m => m.GetCustomAttributes(typeof(global::NUnit.Framework.TearDownAttribute), true)
                                                                      .Any());

                if (fixtureSetupMethod != null)
                {
                    fixtureSetupMethod.Invoke(obj, null);
                }

                foreach (var method in allMethods)
                {
                    var testAttribs = method.GetCustomAttributes(typeof(global::NUnit.Framework.TestAttribute), true);
                    if (testAttribs.Length < 1)
                    {
                        // not marked as test
                        continue;
                    }

                    var methodIgnoreAttribs = method.GetCustomAttributes(typeof(global::NUnit.Framework.IgnoreAttribute), true);
                    if (methodIgnoreAttribs.Length > 0)
                    {
                        // is ignored
                        continue;
                    }

                    try
                    {
                        var ta = (TestAttribute)testAttribs[0];

                        Console.Write("\t'{0}' ... ", method.Name);

                        if (setupMethod != null)
                        {
                            setupMethod.Invoke(obj, null);
                        }

                        method.Invoke(obj, null);

                        if (tearDownMethod != null)
                        {
                            tearDownMethod.Invoke(obj, null);
                        }

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

                if (fixtureTearDownMethod != null)
                {
                    fixtureTearDownMethod.Invoke(obj, null);
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