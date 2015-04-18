// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Values
{
    /// <summary>
    /// A routed state that uses <see cref="TrafficLight" /> values.
    /// </summary>
    public sealed class TrafficState : RoutedState<TrafficLight>
    {
        #region Constructors (2)

        /// <inheriteddoc />
        public TrafficState(object sync)
            : base(sync: sync,
                   stradegy: Stradegy.Ascending)
        {
        }

        /// <inheriteddoc />
        public TrafficState()
            : base(stradegy: Stradegy.Ascending)
        {
        }

        #endregion Constructors (2)
    }
}