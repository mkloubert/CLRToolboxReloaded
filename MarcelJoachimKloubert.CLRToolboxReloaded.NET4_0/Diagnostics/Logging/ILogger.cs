// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// Describes a logger.
    /// </summary>
    public interface ILogger : IObject
    {
        #region Operations (2)

        /// <summary>
        /// Loggs a message.
        /// </summary>
        /// <param name="msgObj">The message to log.</param>
        /// <returns>Logging was successful or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="msgObj" /> is <see langword="null" />.
        /// </exception>
        bool Log(ILogMessage msgObj);

        /// <summary>
        /// Logs an object.
        /// </summary>
        /// <param name="msg">The object to log.</param>
        /// <param name="tag">The tag, like the name of the source.</param>
        /// <param name="categories">The list of categaories the message object belongs to.</param>
        /// <returns>Logging was successful or not.</returns>
        bool Log(object msg,
                 IEnumerable<char> tag = null,
                 LogCategories categories = LogCategories.None);

        #endregion Operations (2)
    }
}