﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_SYSTEM_DIAGNOSTICS_PROCESS
#endif

using MarcelJoachimKloubert.CLRToolbox.Timing;
using System;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// Handles (custom) application time.
    /// </summary>
    public static class AppTime
    {
        #region Fields

        /// <summary>
        /// Stores the real start time of the app.
        /// </summary>
        public static readonly DateTimeOffset START_TIME;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Initilizes the <see cref="AppTime" /> class.
        /// </summary>
        static AppTime()
        {
#if KNOWS_SYSTEM_DIAGNOSTICS_PROCESS
            try
            {
                START_TIME = global::System.Diagnostics.Process.GetCurrentProcess().StartTime;
            }
            catch
            {
                START_TIME = global::System.DateTimeOffset.Now;
            }
#else
            START_TIME = global::System.DateTimeOffset.Now;
#endif
        }

        #endregion Constructors

        #region Properties (8)

        /// <summary>
        /// Gets the clock part of <see cref="AppTime.Now" />.
        /// </summary>
        public static Time ClockTime
        {
            get { return (Time)Now; }
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public static DateTimeOffset Now
        {
            get { return (NowProvider ?? GetNow)(); }
        }

        /// <summary>
        /// Gets or sets a custom handler that returns the value of <see cref="AppTime.Now" />.
        /// If the value is <see langword="null" /> the default logic is called.
        /// </summary>
        public static TimeProvider NowProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the time the app is running.
        /// </summary>
        public static TimeSpan RunTime
        {
            get { return (RunTimeProvider ?? GetRunTime)(); }
        }

        /// <summary>
        /// Gets or sets a custom handler that returns the value of <see cref="AppTime.RunTime" />.
        /// If the value is <see langword="null" /> the default logic is called.
        /// </summary>
        public static TimeSpanProvider RunTimeProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the start time of the application.
        /// </summary>
        public static DateTimeOffset StartTime
        {
            get { return (StartTimeProvider ?? GetStartTime)(); }
        }

        /// <summary>
        /// Gets or sets a custom handler that returns the value of <see cref="AppTime.StartTime" />.
        /// If the value is <see langword="null" /> the default logic is called.
        /// </summary>
        public static TimeProvider StartTimeProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the date part of <see cref="AppTime.Now" />.
        /// </summary>
        public static DateTime Today
        {
            get { return Now.Date; }
        }

        #endregion Properties

        #region Delegates and Events (2)

        // Delegates (2) 

        /// <summary>
        /// Decribes a method or function that returns a time.
        /// </summary>
        /// <returns>The time.</returns>
        public delegate DateTimeOffset TimeProvider();

        /// <summary>
        /// Decribes a method or function that returns a run time.
        /// </summary>
        /// <returns>The run time.</returns>
        public delegate TimeSpan TimeSpanProvider();

        #endregion Delegates and Events

        #region Methods (3)

        private static DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now;
        }

        private static TimeSpan GetRunTime()
        {
            return DateTimeOffset.Now - START_TIME;
        }

        private static DateTimeOffset GetStartTime()
        {
            return START_TIME;
        }

        #endregion Methods
    }
}