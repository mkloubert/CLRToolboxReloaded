// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if (PORTABLE40)
#define ARGUMENT_OUT_OF_RANGE_HAS_TWO_CONSTRUCTOR_PARAMS
#endif

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MarcelJoachimKloubert.CLRToolbox.Timing
{
    /// <summary>
    /// Stores a (clock) time.
    /// In other words: A limitation of a <see cref="TimeSpan" /> that only supports a tick
    /// range between 0 (00:00:00.00000) and 863999999999 (23:59:59.99999).
    /// </summary>
    public struct Time : IEquatable<Time>,
                         IComparable<Time>, IComparable, IFormattable
    {
        #region Fields (6)

        private readonly long _TICKS;

        /// <summary>
        /// Saves the maximum tick value.
        /// </summary>
        public const long MaxTicks = 863999999999;

        /// <summary>
        /// The maximum value.
        /// </summary>
        public static readonly Time MaxValue = new Time(ticks: 863999999999);

        /// <summary>
        /// Saves the minimum tick value.
        /// </summary>
        public const long MinTicks = 0;

        /// <summary>
        /// The minimum value.
        /// </summary>
        public static readonly Time MinValue = new Time(ticks: 0);

        /// <summary>
        /// Represents a zero value.
        /// </summary>
        public static readonly Time Zero = new Time(ticks: 0);

        #endregion Fields (6)

        #region Constructors (4)

        /// <summary>
        /// Initializes the <see cref="Time" /> struct.
        /// </summary>
        static Time()
        {
            // set 1 second as default value
            IncrementAndDecrementBy = TimeSpan.FromTicks(10000000L);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time" /> struct.
        /// </summary>
        /// <param name="ticks">
        /// The value for the <see cref="Time.Ticks" /> property.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ticks" /> is invalid.
        /// </exception>
        public Time(long ticks)
        {
            CheckTickValue(ticks: ticks);

            this._TICKS = ticks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time" /> struct.
        /// </summary>
        /// <param name="ts">
        /// The value for the <see cref="Time.Ticks" /> property that is
        /// token from <see cref="TimeSpan.Ticks" /> property of <paramref name="ts" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ts" /> is invalid.
        /// </exception>
        public Time(TimeSpan ts)
            : this(ticks: ts.Ticks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time" /> struct.
        /// </summary>
        /// <param name="hours">The value for the <see cref="Time.Hours" /> property.</param>
        /// <param name="minutes">The value for the <see cref="Time.Minutes" /> property.</param>
        /// <param name="secs">The value for the <see cref="Time.Seconds" /> property.</param>
        /// <param name="msecs">The value for the <see cref="Time.Milliseconds" /> property.</param>
        /// <param name="ticks">The additional ticks to add.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Combination of all parameters is invalid.
        /// </exception>
        public Time(int hours, int minutes, int secs = 0, int msecs = 0, long ticks = 0)
            : this(ts: new TimeSpan(days: 0,
                                    hours: hours, minutes: minutes, seconds: secs, milliseconds: msecs).Add(TimeSpan.FromTicks(ticks)))
        {
        }

        #endregion Constructors (4)

        #region Properties (29)

        /// <summary>
        /// Gets or sets the step value for decrement (--) operations.
        /// </summary>
        public static TimeSpan DecrementBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.DecrementBy" /> as hours.
        /// </summary>
        public static double DecrementByHours
        {
            get { return DecrementBy.TotalHours; }

            set { DecrementBy = TimeSpan.FromHours(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.DecrementBy" /> as milliseconds.
        /// </summary>
        public static double DecrementByMilliseconds
        {
            get { return DecrementBy.TotalMilliseconds; }

            set { DecrementBy = TimeSpan.FromMilliseconds(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.DecrementBy" /> as minutes.
        /// </summary>
        public static double DecrementByMinutes
        {
            get { return DecrementBy.TotalMinutes; }

            set { DecrementBy = TimeSpan.FromMinutes(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.DecrementBy" /> as seconds.
        /// </summary>
        public static double DecrementBySeconds
        {
            get { return DecrementBy.TotalSeconds; }

            set { DecrementBy = TimeSpan.FromSeconds(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.DecrementBy" /> as ticks.
        /// </summary>
        public static long DecrementByTicks
        {
            get { return DecrementBy.Ticks; }

            set { DecrementBy = TimeSpan.FromTicks(value); }
        }

        /// <summary>
        /// Gets the hour parts of that <see cref="Time" /> value.
        /// </summary>
        public int Hours
        {
            get { return (int)(this._TICKS / 36000000000L % 24L); }
        }

        /// <summary>
        /// Sets the values for <see cref="Time.DecrementBy" /> and
        /// <see cref="Time.IncrementBy" /> properties.
        /// </summary>
        public static TimeSpan IncrementAndDecrementBy
        {
            set
            {
                DecrementBy = value;
                IncrementBy = value;
            }
        }

        /// <summary>
        /// Sets the values for <see cref="Time.DecrementByHours" /> and
        /// <see cref="Time.IncrementByHours" /> properties.
        /// </summary>
        public static double IncrementAndDecrementByHours
        {
            set
            {
                DecrementByHours = value;
                IncrementByHours = value;
            }
        }

        /// <summary>
        /// Sets the values for <see cref="Time.DecrementByMilliseconds" /> and
        /// <see cref="Time.IncrementByMilliseconds" /> properties.
        /// </summary>
        public static double IncrementAndDecrementByMilliseconds
        {
            set
            {
                DecrementByMilliseconds = value;
                IncrementByMilliseconds = value;
            }
        }

        /// <summary>
        /// Sets the values for <see cref="Time.DecrementByMinutes" /> and
        /// <see cref="Time.IncrementByMinutes" /> properties.
        /// </summary>
        public static double IncrementAndDecrementByMinutes
        {
            set
            {
                DecrementByMinutes = value;
                IncrementByMinutes = value;
            }
        }

        /// <summary>
        /// Sets the values for <see cref="Time.DecrementBySeconds" /> and
        /// <see cref="Time.IncrementBySeconds" /> properties.
        /// </summary>
        public static double IncrementAndDecrementBySeconds
        {
            set
            {
                DecrementBySeconds = value;
                IncrementBySeconds = value;
            }
        }

        /// <summary>
        /// Sets the values for <see cref="Time.DecrementByTicks" /> and
        /// <see cref="Time.IncrementByTicks" /> properties.
        /// </summary>
        public static long IncrementAndDecrementByTicks
        {
            set
            {
                DecrementByTicks = value;
                IncrementByTicks = value;
            }
        }

        /// <summary>
        /// Gets or sets the step value for increment (++) operations.
        /// </summary>
        public static TimeSpan IncrementBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.IncrementBy" /> as hours.
        /// </summary>
        public static double IncrementByHours
        {
            get { return IncrementBy.TotalHours; }

            set { IncrementBy = TimeSpan.FromHours(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.IncrementBy" /> as milliseconds.
        /// </summary>
        public static double IncrementByMilliseconds
        {
            get { return IncrementBy.TotalMilliseconds; }

            set { IncrementBy = TimeSpan.FromMilliseconds(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.IncrementBy" /> as minutes.
        /// </summary>
        public static double IncrementByMinutes
        {
            get { return IncrementBy.TotalMinutes; }

            set { IncrementBy = TimeSpan.FromMinutes(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.IncrementBy" /> as seconds.
        /// </summary>
        public static double IncrementBySeconds
        {
            get { return IncrementBy.TotalSeconds; }

            set { IncrementBy = TimeSpan.FromSeconds(value); }
        }

        /// <summary>
        /// Gets or sets the value for/of <see cref="Time.IncrementBy" /> as ticks.
        /// </summary>
        public static long IncrementByTicks
        {
            get { return IncrementBy.Ticks; }

            set { IncrementBy = TimeSpan.FromTicks(value); }
        }

        /// <summary>
        /// Gets the millisecond parts of that <see cref="Time" /> value.
        /// </summary>
        public int Milliseconds
        {
            get { return (int)(this._TICKS / 10000L % 1000L); }
        }

        /// <summary>
        /// Gets the minute parts of that <see cref="Time" /> value.
        /// </summary>
        public int Minutes
        {
            get { return (int)(this._TICKS / 600000000L % 60L); }
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public static Time Now
        {
            get { return (Time)AppTime.Now; }
        }

        /// <summary>
        /// Gets the second parts of that <see cref="Time" /> value.
        /// </summary>
        public int Seconds
        {
            get { return (int)(this._TICKS / 10000000L % 60L); }
        }

        /// <summary>
        /// Gets the number of ticks that represents the time value.
        /// </summary>
        public long Ticks
        {
            get { return this._TICKS; }
        }

        /// <summary>
        /// Gets the number of ticks to the maximum (<see cref="Time.MaxTicks" />).
        /// </summary>
        public long TicksToMax
        {
            get { return MaxTicks - this._TICKS; }
        }

        /// <summary>
        /// Gets the total number of hours that represents that time value.
        /// </summary>
        public double TotalHours
        {
            get { return (double)this._TICKS * 2.7777777777777777E-11; }
        }

        /// <summary>
        /// Gets the total number of milliseconds that represents that time value.
        /// </summary>
        public double TotalMilliseconds
        {
            get
            {
                double num = (double)this._TICKS * 0.0001;

                if (num > 922337203685477.0)
                {
                    return 922337203685477.0;
                }

                if (num < -922337203685477.0)
                {
                    return -922337203685477.0;
                }

                return num;
            }
        }

        /// <summary>
        /// Gets the total number of minutes that represents that time value.
        /// </summary>
        public double TotalMinutes
        {
            get { return (double)this._TICKS * 1.6666666666666667E-09; }
        }

        /// <summary>
        /// Gets the total number of seconds that represents that time value.
        /// </summary>
        public double TotalSeconds
        {
            get { return (double)this._TICKS * 1E-07; }
        }

        #endregion Properties (29)

        #region Methods (53)

        private static void CheckTickValue(long ticks)
        {
            if (IsTicksValueInRange(ticks) == false)
            {
#if ARGUMENT_OUT_OF_RANGE_HAS_TWO_CONSTRUCTOR_PARAMS
                throw new ArgumentOutOfRangeException("ticks",
                                                      string.Format("The tick value has to be a value between {0} and {1}!",
                                                                    MinTicks,
                                                                    MaxTicks));
#else
                throw new ArgumentOutOfRangeException("ticks",
                                                      ticks,
                                                      string.Format("The tick value has to be a value between {0} and {1}!",
                                                                    MinTicks,
                                                                    MaxTicks));
#endif
            }
        }

        /// <summary>
        /// Adds a <see cref="TimeSpan" /> value to this value.
        /// </summary>
        /// <param name="ts">The value to add.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time Add(TimeSpan ts)
        {
            return this.AddTicks(ts.Ticks);
        }

        /// <summary>
        /// Adds a specific number of hours to this value.
        /// </summary>
        /// <param name="hours">The value to add.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time AddHours(double hours)
        {
            return this.Add(TimeSpan.FromHours(hours));
        }

        /// <summary>
        /// Adds a specific number of milliseconds to this value.
        /// </summary>
        /// <param name="msecs">The value to add.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time AddMilliseconds(double msecs)
        {
            return this.Add(TimeSpan.FromMilliseconds(msecs));
        }

        /// <summary>
        /// Adds a specific number of minutes to this value.
        /// </summary>
        /// <param name="minutes">The value to add.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time AddMinutes(double minutes)
        {
            return this.Add(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Adds a specific number of seconds to this value.
        /// </summary>
        /// <param name="secs">The value to add.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time AddSeconds(double secs)
        {
            return this.Add(TimeSpan.FromSeconds(secs));
        }

        /// <summary>
        /// Adds a specific number of ticks to this value.
        /// </summary>
        /// <param name="ticks">The value to add.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time AddTicks(long ticks)
        {
            return new Time(ticks: this._TICKS + ticks);
        }

        /// <summary>
        /// Checks if a specific of number of ticks can be added to this value or not.
        /// </summary>
        /// <param name="ticks">The value to check.</param>
        /// <returns>Ticks can be added or not.</returns>
        public bool CanAddTicks(long ticks)
        {
            bool result;

            try
            {
                result = IsTicksValueInRange(this._TICKS + ticks);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Returns if <see cref="Time.Decrement()" /> method can be invoked or not.
        /// </summary>
        /// <returns>Can invoke method or not.</returns>
        public bool CanDecrement()
        {
            return this.CanSubtractTicks(ticks: DecrementByTicks);
        }

        /// <summary>
        /// Returns if <see cref="Time.Increment()" /> method can be invoked or not.
        /// </summary>
        /// <returns>Can invoke method or not.</returns>
        public bool CanIncrement()
        {
            return this.CanAddTicks(ticks: IncrementByTicks);
        }

        /// <summary>
        /// Checks if a specific of number of ticks can be subtracted from this value or not.
        /// </summary>
        /// <param name="ticks">The value to check.</param>
        /// <returns>Ticks can be subtracted or not.</returns>
        public bool CanSubtractTicks(long ticks)
        {
            bool result;

            try
            {
                result = IsTicksValueInRange(this._TICKS - ticks);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Compare two time values.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>The compare value.</returns>
        public static int Compare(Time x, Time y)
        {
            return x.CompareTo(y);
        }

        /// <inheriteddoc />
        public int CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }

            return this.CompareTo(GlobalConverter.Current
                                                 .ChangeType<Time>(value: other));
        }

        /// <inheriteddoc />
        public int CompareTo(Time other)
        {
            if (this._TICKS > other._TICKS)
            {
                return 1;
            }

            if (this._TICKS < other._TICKS)
            {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Decrements that value by <see cref="Time.DecrementBy" /> property.
        /// </summary>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time Decrement()
        {
            return this.Subtract(DecrementBy);
        }

        /// <inheriteddoc />
        public bool Equals(Time other)
        {
            return this._TICKS
                       .Equals(other._TICKS);
        }

        /// <inheriteddoc />
        public override bool Equals(object other)
        {
            if (other is Time)
            {
                return this.Equals((Time)other);
            }

            return base.Equals(other);
        }

        /// <summary>
        /// Checks if two time values are equal.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal or not.</returns>
        public static bool Equals(Time left, Time right)
        {
            return left.Equals(right);
        }

        /// <inheriteddoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Creates a new value from a value of total hours.
        /// </summary>
        /// <param name="hours">The value.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="hours" /> is invalid.
        /// </exception>
        public static Time FromHours(double hours)
        {
            return new Time(ts: TimeSpan.FromHours(value: hours));
        }

        /// <summary>
        /// Creates a new value from a value of total milli seconds.
        /// </summary>
        /// <param name="msecs">The value.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="msecs" /> is invalid.
        /// </exception>
        public static Time FromMilliseconds(double msecs)
        {
            return new Time(ts: TimeSpan.FromMilliseconds(value: msecs));
        }

        /// <summary>
        /// Creates a new value from a value of total minutes.
        /// </summary>
        /// <param name="minutes">The value.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minutes" /> is invalid.
        /// </exception>
        public static Time FromMinutes(double minutes)
        {
            return new Time(ts: TimeSpan.FromMinutes(value: minutes));
        }

        /// <summary>
        /// Creates a new value from a value of total seconds.
        /// </summary>
        /// <param name="secs">The value.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="secs" /> is invalid.
        /// </exception>
        public static Time FromSeconds(double secs)
        {
            return new Time(ts: TimeSpan.FromSeconds(value: secs));
        }

        /// <summary>
        /// Creates a new value from a tick value.
        /// </summary>
        /// <param name="ticks">The value.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ticks" /> is invalid.
        /// </exception>
        public static Time FromTicks(long ticks)
        {
            return new Time(ticks: ticks);
        }

        /// <summary>
        /// Calculates the difference of this with another time values.
        /// </summary>
        /// <param name="other">The other value.</param>
        /// <returns>The difference.</returns>
        public TimeSpan GetDifference(Time other)
        {
            return TimeSpan.FromTicks(this._TICKS - other._TICKS);
        }

        /// <summary>
        /// Returns a sequence of current values from <see cref="Time.Now" /> property.
        /// </summary>
        /// <param name="count">The number of instance.</param>
        /// <returns>
        /// The values (each item represents a new and up-to-date value from <see cref="Time.Now" /> property).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count" /> is smaller than 0.
        /// </exception>
        public static IEnumerable<Time> GetNows(long count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            for (long i = 0; i < count; i++)
            {
                yield return Now;
            }
        }

        /// <summary>
        /// Increments that value by <see cref="Time.IncrementBy" /> property.
        /// </summary>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time Increment()
        {
            return this.Add(IncrementBy);
        }

        private static bool IsTicksValueInRange(long ticks)
        {
            return (ticks >= MinTicks) &&
                   (ticks <= MaxTicks);
        }

        /// <summary>
        /// Creates a new (normalized) value and takes the hour part.
        /// The other values are set to 0.
        /// </summary>
        /// <returns>The normalized value.</returns>
        public Time NormalizeHours()
        {
            return new Time(hours: this.Hours,
                            minutes: 0);
        }

        /// <summary>
        /// Creates a new (normalized) value and takes the hour, minute, second and millisecond parts.
        /// The other values are set to 0.
        /// </summary>
        /// <returns>The normalized value.</returns>
        public Time NormalizeMilliseconds()
        {
            return new Time(hours: this.Hours,
                            minutes: this.Minutes,
                            secs: this.Seconds,
                            msecs: this.Milliseconds);
        }

        /// <summary>
        /// Creates a new (normalized) value and takes the hour and minute parts.
        /// The other values are set to 0.
        /// </summary>
        /// <returns>The normalized value.</returns>
        public Time NormalizeMinutes()
        {
            return new Time(hours: this.Hours,
                            minutes: this.Minutes);
        }

        /// <summary>
        /// Creates a new (normalized) value and takes the hour, minute and second parts.
        /// The other values are set to 0.
        /// </summary>
        /// <returns>The normalized value.</returns>
        public Time NormalizeSeconds()
        {
            return new Time(hours: this.Hours,
                            minutes: this.Minutes,
                            secs: this.Seconds);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="TimeSpan.Parse(string, IFormatProvider)" />
        public static Time Parse(string format, IFormatProvider formatProvider = null)
        {
            return new Time(ts: formatProvider == null ? TimeSpan.Parse(format)
                                                       : TimeSpan.Parse(format, formatProvider));
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="TimeSpan.ParseExact(string, string, IFormatProvider, TimeSpanStyles)" />
        public static Time ParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles = TimeSpanStyles.None)
        {
            return (Time)TimeSpan.ParseExact(input, format, formatProvider, styles);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="TimeSpan.ParseExact(string, string[], IFormatProvider, TimeSpanStyles)" />
        public static Time ParseExact(string input, IEnumerable<string> formats, IFormatProvider formatProvider, TimeSpanStyles styles = TimeSpanStyles.None)
        {
            return (Time)TimeSpan.ParseExact(input, formats.AsArray(), formatProvider, styles);
        }

        /// <summary>
        /// Subtracts a <see cref="TimeSpan" /> value from this value.
        /// </summary>
        /// <param name="ts">The value to subtract.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time Subtract(TimeSpan ts)
        {
            return this.SubtractTicks(ts.Ticks);
        }

        /// <summary>
        /// Subtracts a specific number of hours from this value.
        /// </summary>
        /// <param name="hours">The value to subtract.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time SubtractHours(double hours)
        {
            return this.Subtract(TimeSpan.FromHours(hours));
        }

        /// <summary>
        /// Subtracts a specific number of milliseconds from this value.
        /// </summary>
        /// <param name="msecs">The value to subtract.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time SubtractMilliseconds(double msecs)
        {
            return this.Subtract(TimeSpan.FromMilliseconds(msecs));
        }

        /// <summary>
        /// Subtracts a specific number of minutes from this value.
        /// </summary>
        /// <param name="minutes">The value to subtract.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time SubtractMinutes(double minutes)
        {
            return this.Subtract(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Subtracts a specific number of seconds from this value.
        /// </summary>
        /// <param name="secs">The value to subtract.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time SubtractSeconds(double secs)
        {
            return this.Subtract(TimeSpan.FromSeconds(secs));
        }

        /// <summary>
        /// Subtracts a specific number of ticks from this value.
        /// </summary>
        /// <param name="ticks">The value to subtract.</param>
        /// <returns>The new value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// New value is invalid.
        /// </exception>
        public Time SubtractTicks(long ticks)
        {
            return new Time(ticks: this._TICKS - ticks);
        }

        /// <summary>
        /// Applies the value from <see cref="Time.Hours" /> and moves to the last possible tick of
        /// the hour.
        /// </summary>
        /// <returns>The new value.</returns>
        public Time ToEndOfHour()
        {
            return this.NormalizeHours()
                       .AddTicks(35999999999);
        }

        /// <summary>
        /// Applies the values from <see cref="Time.Hours" />, <see cref="Time.Minutes" />, <see cref="Time.Seconds" />
        /// and <see cref="Time.Milliseconds" /> and moves to the last possible tick of the millisecond.
        /// </summary>
        /// <returns>The new value.</returns>
        public Time ToEndOfMillisecond()
        {
            return this.NormalizeMilliseconds()
                       .AddTicks(9999);
        }

        /// <summary>
        /// Applies the values from <see cref="Time.Hours" /> and <see cref="Time.Minutes" />
        /// and moves to the last possible tick of the minute.
        /// </summary>
        /// <returns>The new value.</returns>
        public Time ToEndOfMinute()
        {
            return this.NormalizeMinutes()
                       .AddTicks(599999999);
        }

        /// <summary>
        /// Applies the values from <see cref="Time.Hours" />, <see cref="Time.Minutes" /> and <see cref="Time.Seconds" />
        /// and moves to the last possible tick of the second.
        /// </summary>
        /// <returns>The new value.</returns>
        public Time ToEndOfSecond()
        {
            return this.NormalizeSeconds()
                       .AddTicks(9999999);
        }

        /// <inheriteddoc />
        public override string ToString()
        {
            return this.ToString("c");
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IFormattable.ToString(string, IFormatProvider)" />
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            TimeSpan ts = this;

            return formatProvider == null ? ts.ToString(format)
                                          : ts.ToString(format, formatProvider);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="TimeSpan.TryParse(string, IFormatProvider, out TimeSpan)" />
        public static bool TryParse(string format, out Time value, IFormatProvider formatProvider = null)
        {
            bool result;

            TimeSpan ts;
            if (formatProvider == null)
            {
                result = TimeSpan.TryParse(format, out ts);
            }
            else
            {
                result = TimeSpan.TryParse(format, formatProvider, out ts);
            }

            if (result)
            {
                // ticks out of range?
                result = IsTicksValueInRange(ts.Ticks);
            }

            value = result ? new Time(ts: ts) : new Time();
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="TimeSpan.TryParseExact(string, string, IFormatProvider, TimeSpanStyles, out TimeSpan)" />
        public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, out Time value, TimeSpanStyles styles = TimeSpanStyles.None)
        {
            TimeSpan ts;
            var result = TimeSpan.TryParseExact(input, format, formatProvider, styles, out ts);

            if (result)
            {
                // ticks out of range?
                result = IsTicksValueInRange(ts.Ticks);
            }

            value = result ? new Time(ts: ts) : new Time();
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="TimeSpan.TryParseExact(string, string[], IFormatProvider, TimeSpanStyles, out TimeSpan)" />
        public static bool TryParseExact(string input, IEnumerable<string> formats, IFormatProvider formatProvider, out Time value, TimeSpanStyles styles = TimeSpanStyles.None)
        {
            TimeSpan ts;
            var result = TimeSpan.TryParseExact(input, formats.AsArray(), formatProvider, styles, out ts);

            if (result)
            {
                // ticks out of range?
                result = IsTicksValueInRange(ts.Ticks);
            }

            value = result ? new Time(ts: ts) : new Time();
            return result;
        }

        /// <summary>
        /// Walks a specific count of ticks starting from this value.
        /// </summary>
        /// <param name="stepInTicks">The step in ticks.</param>
        /// <returns>The sequence of values.</returns>
        public IEnumerable<Time> Walk(long stepInTicks)
        {
            var currentValue = this;
            while (true)
            {
                yield return currentValue;

                if (currentValue.CanAddTicks(stepInTicks))
                {
                    currentValue = currentValue.AddTicks(stepInTicks);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Walks a specific timespan starting from this value.
        /// </summary>
        /// <param name="step">The step to walk.</param>
        /// <returns>The sequence of values.</returns>
        public IEnumerable<Time> Walk(TimeSpan step)
        {
            return this.Walk(step.Ticks);
        }

        #endregion Methods (53)

        #region Operators (31)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.Add(TimeSpan)" />
        public static Time operator +(Time time, TimeSpan ts)
        {
            return time.Add(ts);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.AddTicks(long)" />
        public static Time operator +(Time time, long ticks)
        {
            return time.AddTicks(ticks);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.Increment()" />
        public static Time operator ++(Time time)
        {
            return time.Increment();
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.Subtract(TimeSpan)" />
        public static Time operator -(Time time, TimeSpan ts)
        {
            return time.Subtract(ts);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.SubtractTicks(long)" />
        public static Time operator -(Time time, long ticks)
        {
            return time.SubtractTicks(ticks);
        }

        /// <summary>
        /// Calculates the difference of two time values.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The difference.</returns>
        public static TimeSpan operator -(Time left, Time right)
        {
            return left.GetDifference(right);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.Decrement()" />
        public static Time operator --(Time time)
        {
            return time.Decrement();
        }

        /// <summary>
        /// Checks if a left value has the same value like a right value.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal or not.</returns>
        public static bool operator ==(Time left, Time right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks if a left value has the NOT same value like a right value.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Are equal (<see langword="false" />) or not (<see langword="true" />).</returns>
        public static bool operator !=(Time left, Time right)
        {
            return (left == right) == false;
        }

        /// <summary>
        /// Checks if a left value smaller than a right value.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns><paramref name="left" /> is smaller than <paramref name="right" /> or not.</returns>
        public static bool operator <(Time left, Time right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Checks if a left value smaller or equal than a right value.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns><paramref name="left" /> is smaller/equal than <paramref name="right" /> or not.</returns>
        public static bool operator <=(Time left, Time right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Checks if a left value greater than a right value.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns><paramref name="left" /> is greater than <paramref name="right" /> or not.</returns>
        public static bool operator >(Time left, Time right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Checks if a left value greater or equal than a right value.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns><paramref name="left" /> is greater/equal than <paramref name="right" /> or not.</returns>
        public static bool operator >=(Time left, Time right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset" /> value to its time part.
        /// </summary>
        /// <param name="dateTimeOff">The input value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator Time(DateTimeOffset dateTimeOff)
        {
            return (Time)dateTimeOff.DateTime;
        }

        /// <summary>
        /// Converts a <see cref="DateTime" /> value to its time part.
        /// </summary>
        /// <param name="dateTime">The input value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator Time(DateTime dateTime)
        {
            return new Time(ts: dateTime - dateTime.Date);
        }

        /// <summary>
        /// Converts a <see cref="Time" /> to a <see cref="DateTimeOffset" /> value.
        /// </summary>
        /// <param name="time">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// The date part from <see cref="AppTime.Now" /> is token.
        /// </remarks>
        public static implicit operator DateTimeOffset(Time time)
        {
            return AppTime.Now <= time;
        }

        /// <summary>
        /// Converts a <see cref="Time" /> to a <see cref="DateTime" /> value.
        /// </summary>
        /// <param name="time">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// The date part from <see cref="AppTime.Now" /> is token.
        /// </remarks>
        public static implicit operator DateTime(Time time)
        {
            return AppTime.Now.DateTime <= time;
        }

        /// <summary>
        /// Converts a <see cref="Time" /> value to a <see cref="long" /> value
        /// that represents the ticks of <paramref name="time" />.
        /// </summary>
        /// <param name="time">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static implicit operator long(Time time)
        {
            return time._TICKS;
        }

        /// <summary>
        /// Converts a <see cref="long" /> value that represents ticks to an instance of that struct.
        /// </summary>
        /// <param name="ticks">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ticks" /> is invalid.
        /// </exception>
        public static explicit operator Time(long ticks)
        {
            return new Time(ticks: ticks);
        }

        /// <summary>
        /// Converts a <see cref="Time" /> value to a <see cref="TimeSpan" /> value.
        /// </summary>
        /// <param name="time">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static implicit operator TimeSpan(Time time)
        {
            return TimeSpan.FromTicks(time._TICKS);
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan" /> value to an instance of that struct.
        /// </summary>
        /// <param name="ts">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ts" /> is invalid.
        /// </exception>
        public static explicit operator Time(TimeSpan ts)
        {
            return new Time(ts: ts);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.ToString()" />
        public static explicit operator string(Time time)
        {
            return time.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Time.Parse(string, IFormatProvider)" />
        public static implicit operator Time(string str)
        {
            return Parse(str);
        }

        /// <summary>
        /// Overwrites the time part of a <see cref="DateTime" /> value.
        /// </summary>
        /// <param name="dateTime">The source value.</param>
        /// <param name="time">The time value to apply.</param>
        /// <returns>The new value.</returns>
        public static DateTime operator <=(DateTime dateTime, Time time)
        {
            return dateTime.Date
                           .AddTicks(time._TICKS);
        }

        /// <summary>
        /// NOT SUPPORTED!
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public static DateTime operator >=(DateTime dateTime, Time time)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Overwrites the time part of a <see cref="DateTimeOffset" /> value.
        /// </summary>
        /// <param name="dateTimeOff">The source value.</param>
        /// <param name="time">The time value to apply.</param>
        /// <returns>The new value.</returns>
        public static DateTimeOffset operator <=(DateTimeOffset dateTimeOff, Time time)
        {
            return new DateTimeOffset(dateTimeOff.DateTime
                                                 .Date
                                                 .AddTicks(time._TICKS),
                                      dateTimeOff.Offset);
        }

        /// <summary>
        /// NOT SUPPORTED!
        /// </summary>
        /// <param name="dateTimeOff"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public static DateTimeOffset operator >=(DateTimeOffset dateTimeOff, Time time)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a sequence of time values. Each next item is increased by <paramref name="ticks" /> parameter.
        /// </summary>
        /// <param name="time">The start value.</param>
        /// <param name="ticks">The step in ticks.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<Time> operator *(Time time, long ticks)
        {
            return time.Walk(ticks);
        }

        /// <summary>
        /// Returns a sequence of time values. Each next item is increased by <paramref name="ticks" /> parameter.
        /// </summary>
        /// <param name="ticks">The step in ticks.</param>
        /// <param name="time">The start value.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<Time> operator *(long ticks, Time time)
        {
            return time * ticks;
        }

        /// <summary>
        /// Returns a sequence of time values. Each next item is increased by <paramref name="ts" /> parameter.
        /// </summary>
        /// <param name="time">The start value.</param>
        /// <param name="ts">The step.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<Time> operator *(Time time, TimeSpan ts)
        {
            return time.Walk(ts);
        }

        /// <summary>
        /// Returns a sequence of time values. Each next item is increased by <paramref name="ts" /> parameter.
        /// </summary>
        /// <param name="ts">The step.</param>
        /// <param name="time">The start value.</param>
        /// <returns>The sequence of values.</returns>
        public static IEnumerable<Time> operator *(TimeSpan ts, Time time)
        {
            return time * ts;
        }

        #endregion Operators (31)
    }
}