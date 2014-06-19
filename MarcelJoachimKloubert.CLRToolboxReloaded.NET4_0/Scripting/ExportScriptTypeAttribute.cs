// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Scripting.Export
{
    /// <summary>
    /// Marks a type to expose in a script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Delegate |
                    AttributeTargets.Enum |
                    AttributeTargets.Struct,
                    AllowMultiple = false,
                    Inherited = false)]
    public sealed class ExportScriptTypeAttribute : Attribute
    {
        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportScriptTypeAttribute" /> class.
        /// </summary>
        /// <param name="alias">The value for <see cref="ExportScriptTypeAttribute.Alias" /> property.</param>
        public ExportScriptTypeAttribute(string alias)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportScriptTypeAttribute" /> class.
        /// </summary>
        public ExportScriptTypeAttribute()
            : this(null)
        {
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets the alias that should be used when expose type in script.
        /// </summary>
        public string Alias
        {
            get;
            private set;
        }

        #endregion Properties
    }
}