// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Walks from the beginning of <paramref name="value" /> with a specific count of ticks.
        /// </summary>
        /// <param name="value">The beginning value.</param>
        /// <param name="stepInTicks">The number of ticks to walk.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<DateTimeOffset> Walk(this DateTimeOffset value, long stepInTicks)
        {
            return Walk(value, TimeSpan.FromTicks(stepInTicks));
        }

        /// <summary>
        /// Walks from the beginning of <paramref name="value" /> with a specific step.
        /// </summary>
        /// <param name="value">The beginning value.</param>
        /// <param name="step">The step to walk.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<DateTimeOffset> Walk(this DateTimeOffset value, TimeSpan step)
        {
            var currentValue = value;
            while (true)
            {
                yield return currentValue;

                try
                {
                    currentValue = value.Add(step);
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Walks from the beginning of <paramref name="value" /> with a specific count of ticks.
        /// </summary>
        /// <param name="value">The beginning value.</param>
        /// <param name="stepInTicks">The number of ticks to walk.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<DateTime> Walk(this DateTime value, long stepInTicks)
        {
            return Walk(value,
                        TimeSpan.FromTicks(stepInTicks));
        }

        /// <summary>
        /// Walks from the beginning of <paramref name="value" /> with a specific step.
        /// </summary>
        /// <param name="value">The beginning value.</param>
        /// <param name="step">The step to walk.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<DateTime> Walk(this DateTime value, TimeSpan step)
        {
            var currentValue = value;
            while (true)
            {
                yield return currentValue;

                try
                {
                    currentValue = value.Add(step);
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Walks from the beginning of <paramref name="value" /> with a specific count of ticks.
        /// </summary>
        /// <param name="value">The beginning value.</param>
        /// <param name="stepInTicks">The number of ticks to walk.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<TimeSpan> Walk(this TimeSpan value, long stepInTicks)
        {
            return Walk(value,
                        TimeSpan.FromTicks(stepInTicks));
        }

        /// <summary>
        /// Walks from the beginning of <paramref name="value" /> with a specific step.
        /// </summary>
        /// <param name="value">The beginning value.</param>
        /// <param name="step">The step to walk.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<TimeSpan> Walk(this TimeSpan value, TimeSpan step)
        {
            var currentValue = value;
            while (true)
            {
                yield return currentValue;

                try
                {
                    currentValue = value.Add(step);
                }
                catch (OverflowException)
                {
                    break;
                }
            }
        }

        #endregion Methods (6)
    }
}