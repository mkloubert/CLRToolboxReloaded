// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Data
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Creates an object from a data record.
        /// </summary>
        /// <typeparam name="TRead">Type of the reader.</typeparam>
        /// <typeparam name="TResult">Type of the created objects.</typeparam>
        /// <param name="reader">The record from where to build the object from.</param>
        /// <param name="func">The function that create the result object.</param>
        /// <returns>The created objects.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static IEnumerable<TResult> BuildAll<TRead, TResult>(TRead reader,
                                                                    Func<TRead, long, TResult> func)
            where TRead : System.Data.IDataReader
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return BuildAll<TRead, Func<TRead, long, TResult>, TResult>(reader,
                                                                        (r, f, i) => f(r, i),
                                                                        funcState: func);
        }

        /// <summary>
        /// Creates a list of objects from a data reader.
        /// </summary>
        /// <typeparam name="TRead">Type of the reader.</typeparam>
        /// <typeparam name="TState">Type of <paramref name="funcState" />.</typeparam>
        /// <typeparam name="TResult">Type of the created objects.</typeparam>
        /// <param name="reader">The record from where to build the object from.</param>
        /// <param name="func">The function that create the result object.</param>
        /// <param name="funcState">The second argument of <paramref name="func" />.</param>
        /// <returns>The created objects.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static IEnumerable<TResult> BuildAll<TRead, TState, TResult>(TRead reader,
                                                                            Func<TRead, TState, long, TResult> func,
                                                                            TState funcState)
            where TRead : System.Data.IDataReader
        {
            return BuildAll<TRead, TState, TResult>(reader,
                                                    func,
                                                    (r, i) => funcState);
        }

        /// <summary>
        /// Creates a list of objects from a data reader.
        /// </summary>
        /// <typeparam name="TReader">Type of the reader.</typeparam>
        /// <typeparam name="TState">Type of the result of <paramref name="funcStateProvider" />.</typeparam>
        /// <typeparam name="TResult">Type of the created objects.</typeparam>
        /// <param name="reader">The record from where to build the object from.</param>
        /// <param name="func">The function that create the result object.</param>
        /// <param name="funcStateProvider">The function that returns the second argument of <paramref name="func" />.</param>
        /// <returns>The created objects.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" />, <paramref name="func" /> and/or <paramref name="funcStateProvider" /> are
        /// <see langword="null" />.
        /// </exception>
        public static IEnumerable<TResult> BuildAll<TReader, TState, TResult>(TReader reader,
                                                                              Func<TReader, TState, long, TResult> func,
                                                                              Func<TReader, long, TState> funcStateProvider)
            where TReader : System.Data.IDataReader
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateProvider == null)
            {
                throw new ArgumentNullException("funcStateFactory");
            }

            long index = -1;
            while (reader.Read())
            {
                yield return Build(reader,
                                   (r, s) => s.Function(r,
                                                        s.StateProvider(r, s.Index),
                                                        s.Index),
                                   funcStateProvider: (r) => new
                                   {
                                       Function = func,
                                       Index = ++index,
                                       StateProvider = funcStateProvider,
                                   });
            }
        }

        #endregion Methods
    }
}