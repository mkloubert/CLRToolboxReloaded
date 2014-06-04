﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Scripting
{
    /// <summary>
    /// Describes an object that executes a script.
    /// </summary>
    public interface IScriptExecutor : IDisposableObject
    {
        #region Operations (5)

        /// <summary>
        /// Executes a script.
        /// </summary>
        /// <param name="src">The source code of the script.</param>
        /// <param name="autoStart">Automatically start script execution or not.</param>
        /// <param name="debug">Run in debug mode or not.</param>
        /// <returns>The execution context.</returns>
        /// <exception cref="ObjectDisposedException">Object has already been disposed.</exception>
        IScriptExecutionContext Execute(string src,
                                        bool autoStart = true,
                                        bool debug = false);

        /// <summary>
        /// Exposes a type.
        /// </summary>
        /// <typeparam name="T">Type to expose.</typeparam>
        /// <param name="alias">The name of the type to use in the script.</param>
        /// <returns>That instance.</returns>
        /// <exception cref="ObjectDisposedException">Object has already been disposed.</exception>
        IScriptExecutor ExposeType<T>(string alias = null);

        /// <summary>
        /// Exposes a type.
        /// </summary>
        /// <param name="type">Type to expose.</param>
        /// <param name="alias">he name of the type to use in the script.</param>
        /// <returns>That instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException">Object has already been disposed.</exception>
        IScriptExecutor ExposeType(Type type, string alias = null);

        /// <summary>
        /// Sets a global function.
        /// </summary>
        /// <param name="funcName">The name of the function.</param>
        /// <param name="func">The (new) function.</param>
        /// <returns>That instance.</returns>
        IScriptExecutor SetFunction(string funcName, Delegate func);

        /// <summary>
        /// Sets a global variable.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
        /// <param name="value">The (new) value for the variable.</param>
        /// <returns>That instance.</returns>
        IScriptExecutor SetVariable(string varName, object value);

        #endregion Operations
    }
}