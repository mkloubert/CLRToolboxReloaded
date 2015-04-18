// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE40 || PORTABLE45)
#define KNOWS_SECURE_STRING
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Console
{
    /// <summary>
    /// A basic console.
    /// </summary>
    public abstract class ConsoleBase : ObjectBase, IConsole
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected ConsoleBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ConsoleBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected ConsoleBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ConsoleBase()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public virtual ConsoleColor? BackgroundColor
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public virtual ConsoleColor? ForegroundColor
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods (25)

        /// <inheriteddoc />
        public ConsoleBase Clear()
        {
            this.OnClear();

            return this;
        }

        IConsole IConsole.Clear()
        {
            return this.Clear();
        }

        /// <summary>
        /// Returns the expression that represents a new line for console output.
        /// </summary>
        /// <returns>Th expression that represents a new line.</returns>
        protected virtual string GetNewLineForOutput()
        {
            return Environment.NewLine;
        }

        /// <summary>
        /// The logic for <see cref="ConsoleBase.Clear()" /> method.
        /// </summary>
        protected abstract void OnClear();

        /// <summary>
        /// The logic for <see cref="ConsoleBase.ReadLine()" /> method.
        /// </summary>
        /// <param name="line">The target where to write the read line data to.</param>
        protected abstract void OnReadLine(TextWriter line);

#if KNOWS_SECURE_STRING

        /// <summary>
        /// The logic for <see cref="ConsoleBase.ReadPassword()" /> method.
        /// </summary>
        /// <param name="pwd">The result for <see cref="ConsoleBase.ReadPassword()" />.</param>
        protected virtual void OnReadPassword(ref global::System.Security.SecureString pwd)
        {
            var line = this.ReadLine();
            if (line != null)
            {
                line.ForEach(ctx =>
                    {
                        ctx.State
                           .Password.AppendChar(ctx.Item);
                    }, new
                    {
                        Password = pwd,
                    });
            }
            else
            {
                pwd = null;
            }
        }

#endif

        /// <summary>
        /// The logic for <see cref="ConsoleBase.Write(string)" /> method.
        /// </summary>
        /// <param name="text">The text to write.</param>
        protected abstract void OnWrite(string text);

        /// <inheriteddoc />
        public string ReadLine()
        {
            using (var writer = new StringWriter())
            {
                this.OnReadLine(writer);

                return writer.ToString();
            }
        }

#if KNOWS_SECURE_STRING

        /// <inheriteddoc />
        public global::System.Security.SecureString ReadPassword()
        {
            var pwd = new global::System.Security.SecureString();
            this.OnReadPassword(ref pwd);

            return pwd;
        }

#endif

        /// <summary>
        /// Converts the items of a sequence of objects to a sequence of console arguments.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        /// <returns>The parsed argument list.</returns>
        protected virtual IEnumerable ToConsoleArguments(IEnumerable args)
        {
            if (args == null)
            {
                return new object[] { null };
            }

            return args.Cast<object>()
                       .Select(a => this.ToConsoleString(a))
                       .ToArray();
        }

        /// <summary>
        /// Converts an object to a char sequence for console.
        /// </summary>
        /// <param name="obj">The input object.</param>
        /// <returns>The converted object.</returns>
        protected virtual string ToConsoleString(object obj)
        {
            return obj.AsString();
        }

        /// <inheriteddoc />
        public ConsoleBase Write(object obj)
        {
            return this.Write(chars: this.ToConsoleString(obj));
        }

        IConsole IConsole.Write(object obj)
        {
            return this.Write(obj: obj);
        }

        /// <inheriteddoc />
        public ConsoleBase Write(string chars)
        {
            this.OnWrite(text: chars);

            return this;
        }

        IConsole IConsole.Write(string chars)
        {
            return this.Write(chars: chars);
        }

        /// <inheriteddoc />
        public ConsoleBase Write(string format, params object[] args)
        {
            return this.Write(chars: string.Format(format: format,
                                                   args: this.ToConsoleArguments(args: args)
                                                             .AsArray()));
        }

        IConsole IConsole.Write(string format, params object[] args)
        {
            return this.Write(format: format,
                              args: args);
        }

        /// <inheriteddoc />
        public ConsoleBase WriteLine()
        {
            return this.Write(chars: this.GetNewLineForOutput());
        }

        IConsole IConsole.WriteLine()
        {
            return this.WriteLine();
        }

        /// <inheriteddoc />
        public ConsoleBase WriteLine(object obj)
        {
            return this.Write(chars: string.Format("{0}{1}",
                                                   this.ToConsoleString(obj),
                                                   this.GetNewLineForOutput()));
        }

        IConsole IConsole.WriteLine(object obj)
        {
            return this.WriteLine(obj: obj);
        }

        /// <inheriteddoc />
        public ConsoleBase WriteLine(string chars)
        {
            return this.Write(chars: string.Format("{0}{1}",
                                                   chars,
                                                   this.GetNewLineForOutput()));
        }

        IConsole IConsole.WriteLine(string chars)
        {
            return this.WriteLine(chars: chars);
        }

        /// <inheriteddoc />
        public ConsoleBase WriteLine(string format, params object[] args)
        {
            return this.Write(chars: string.Format("{0}{1}",
                                                   string.Format(format: format,
                                                                 args: this.ToConsoleArguments(args: args)
                                                                           .AsArray()),
                                                   this.GetNewLineForOutput()));
        }

        IConsole IConsole.WriteLine(string format, params object[] args)
        {
            return this.WriteLine(format: format,
                                  args: args);
        }

        #endregion
    }
}