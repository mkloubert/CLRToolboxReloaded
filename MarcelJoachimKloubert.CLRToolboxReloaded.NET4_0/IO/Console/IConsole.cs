// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE40 || PORTABLE45)
#define KNOWS_SECURE_STRING
#endif

using System;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Console
{
    /// <summary>
    /// Describes a console.
    /// </summary>
    public interface IConsole : IObject
    {
        #region Properties (2)

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        ConsoleColor? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        ConsoleColor? ForegroundColor { get; set; }

        #endregion Properties (2)

        #region Methods (10)

        /// <summary>
        /// Clears the console.
        /// </summary>
        /// <returns>That instance.</returns>
        IConsole Clear();

        /// <summary>
        /// Reads the current line of console input.
        /// </summary>
        /// <returns>The read line.</returns>
        string ReadLine();

#if KNOWS_SECURE_STRING

        /// <summary>
        /// Reads data as secure string from standard input.
        /// </summary>
        /// <returns>The read password.</returns>
        global::System.Security.SecureString ReadPassword();

#endif

        /// <summary>
        /// Writes an object to console.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>That instance.</returns>
        IConsole Write(object obj);

        /// <summary>
        /// Writes a char sequence to console.
        /// </summary>
        /// <param name="chars">The chars to write.</param>
        /// <returns>That instance.</returns>
        IConsole Write(string chars);

        /// <summary>
        /// Writes a formated string to console.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments to write.</param>
        /// <returns>That instance.</returns>
        IConsole Write(string format, params object[] args);

        /// <summary>
        /// Writes a new line.
        /// </summary>
        /// <returns>That instance.</returns>
        IConsole WriteLine();

        /// <summary>
        /// Writes an object to console and adds a new line.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>That instance.</returns>
        IConsole WriteLine(object obj);

        /// <summary>
        /// Writes a char sequence to console and adds a new line.
        /// </summary>
        /// <param name="chars">The chars to write.</param>
        /// <returns>That instance.</returns>
        IConsole WriteLine(string chars);

        /// <summary>
        /// Writes a formated string to console and adds a new line.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The arguments to write.</param>
        /// <returns>That instance.</returns>
        IConsole WriteLine(string format, params object[] args);

        #endregion Methods (10)
    }
}