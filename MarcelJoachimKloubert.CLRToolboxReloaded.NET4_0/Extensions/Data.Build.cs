// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Creates an object from a data record.
        /// </summary>
        /// <typeparam name="TRec">Type of the record.</typeparam>
        /// <typeparam name="TResult">Type of the created object.</typeparam>
        /// <param name="rec">The record from where to build the object from.</param>
        /// <param name="func">The function that creates the result object.</param>
        /// <returns>The created object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rec" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult Build<TRec, TResult>(this TRec rec, Func<TRec, TResult> func)
            where TRec : System.Data.IDataRecord
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return Build<TRec, Func<TRec, TResult>, TResult>(rec,
                                                             (r, f) => f(r),
                                                             funcState: func);
        }

        /// <summary>
        /// Creates an object from a data record.
        /// </summary>
        /// <typeparam name="TRec">Type of the record.</typeparam>
        /// <typeparam name="TState">Type of <paramref name="funcState" />.</typeparam>
        /// <typeparam name="TResult">Type of the created object.</typeparam>
        /// <param name="rec">The record from where to build the object from.</param>
        /// <param name="func">The function that creates the result object.</param>
        /// <param name="funcState">The second argument of <paramref name="func" />.</param>
        /// <returns>The created object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rec" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult Build<TRec, TState, TResult>(this TRec rec,
                                                           Func<TRec, TState, TResult> func,
                                                           TState funcState)
            where TRec : System.Data.IDataRecord
        {
            return Build<TRec, TState, TResult>(rec,
                                                func,
                                                funcStateProvider: (r) => funcState);
        }

        /// <summary>
        /// Creates an object from a data record.
        /// </summary>
        /// <typeparam name="TRec">Type of the record.</typeparam>
        /// <typeparam name="TState">Type of the result of <paramref name="funcStateProvider" />.</typeparam>
        /// <typeparam name="TResult">Type of the created object.</typeparam>
        /// <param name="rec">The record from where to build the object from.</param>
        /// <param name="func">The function that creates the result object.</param>
        /// <param name="funcStateProvider">The function that returns the second argument of <paramref name="func" />.</param>
        /// <returns>The created object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rec" />, <paramref name="func" /> and/or <paramref name="funcStateProvider" /> are
        /// <see langword="null" />.
        /// </exception>
        public static TResult Build<TRec, TState, TResult>(this TRec rec,
                                                           Func<TRec, TState, TResult> func,
                                                           Func<TRec, TState> funcStateProvider)
            where TRec : System.Data.IDataRecord
        {
            if (rec == null)
            {
                throw new ArgumentNullException("rec");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateProvider == null)
            {
                throw new ArgumentNullException("funcStateProvider");
            }

            return func(rec,
                        funcStateProvider(rec));
        }

        #endregion Methods
    }
}