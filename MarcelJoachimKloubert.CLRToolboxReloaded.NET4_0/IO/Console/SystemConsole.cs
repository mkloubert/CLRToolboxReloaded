// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Console
{
    /// <summary>
    /// Console based on <see cref="Console" /> class.
    /// </summary>
    public class SystemConsole : ConsoleBase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public SystemConsole(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public SystemConsole(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public SystemConsole(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public SystemConsole()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public override global::System.ConsoleColor? BackgroundColor
        {
            get { return global::System.Console.BackgroundColor; }

            set
            {
                if (value.HasValue)
                {
                    global::System.Console.BackgroundColor = value.Value;
                }
            }
        }

        /// <inheriteddoc />
        public override global::System.ConsoleColor? ForegroundColor
        {
            get { return global::System.Console.ForegroundColor; }

            set
            {
                if (value.HasValue)
                {
                    global::System.Console.ForegroundColor = value.Value;
                }
            }
        }

        #endregion Properties (2)

        #region Methods (3)

        /// <inheriteddoc />
        protected override void OnClear()
        {
            global::System.Console.Clear();
        }

        /// <inheriteddoc />
        protected override void OnReadLine(TextWriter line)
        {
            var str = global::System.Console.ReadLine();
            if (str != null)
            {
                line.Write(str);
            }
        }

        /// <inheriteddoc />
        protected override void OnWrite(string text)
        {
            global::System.Console.Write(value: text);
        }

        #endregion Methods (3)
    }
}