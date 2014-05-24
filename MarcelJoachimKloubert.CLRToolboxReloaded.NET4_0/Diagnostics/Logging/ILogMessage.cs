// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_DO_REMOTING
#define CAN_HANDLE_THREADS
#define CAN_SERIALIZE
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// Describes an object that stores the data of a log message.
    /// </summary>
    public partial interface ILogMessage : IIdentifiable
    {
        #region Data Members (10)

        /// <summary>
        /// Gets the calling assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets the list of underlying categories.
        /// </summary>
        LoggerFacadeCategories Categories { get; }

#if CAN_DO_REMOTING

        /// <summary>
        /// Gets the underlying (remoting) context.
        /// </summary>
        global::System.Runtime.Remoting.Contexts.Context Context { get; }

#endif

        /// <summary>
        /// Gets the tag.
        /// </summary>
        string LogTag { get; }

        /// <summary>
        /// Gets the calling member.
        /// </summary>
        MemberInfo Member { get; }

        /// <summary>
        /// Gets the message (value).
        /// </summary>
        object Message { get; }

        /// <summary>
        /// Gets the underlying principal.
        /// </summary>
        IPrincipal Principal { get; }

#if CAN_HANDLE_THREADS

        /// <summary>
        /// Gets the thread that has written that object.
        /// </summary>
        global::System.Threading.Thread Thread { get; }

#endif

        /// <summary>
        /// Gets the log time.
        /// </summary>
        DateTimeOffset Time { get; }

        #endregion Data Members

        #region Methods (3)

        /// <summary>
        /// Returns the value of <see cref="ILogMessage.Message" /> strong typed.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The strong typed value.</returns>
        T GetMessage<T>();

        /// <summary>
        /// Checks if value of <see cref="ILogMessage.Categories" /> has all
        /// specified categories.
        /// </summary>
        /// <param name="categories">The categories to check.</param>
        /// <returns>Has all categories or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="categories" /> is <see langword="null" />.
        /// </exception>
        bool HasAllCategories(IEnumerable<LoggerFacadeCategories> categories);

        /// <summary>
        /// Checks if value of <see cref="ILogMessage.Categories" /> has all
        /// specified categories.
        /// </summary>
        /// <param name="categories">The categories to check.</param>
        /// <returns>Has all categories or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="categories" /> is <see langword="null" />.
        /// </exception>
        bool HasAllCategories(params LoggerFacadeCategories[] categories);

        #endregion Methods (3)
    }
}