// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Values
{
    partial class RoutedState<T>
    {
        /// <summary>
        /// List of directions.
        /// </summary>
        public enum Stradegy
        {
            /// <summary>
            /// Ascending (take the highest value)
            /// </summary>
            Ascending,

            /// <summary>
            /// Descending (take the lowest value)
            /// </summary>
            Descending,
        }
    }
}