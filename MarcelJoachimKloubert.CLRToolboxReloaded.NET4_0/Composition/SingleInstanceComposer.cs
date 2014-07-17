// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace MarcelJoachimKloubert.CLRToolbox.Composition
{
    /// <summary>
    /// A MEF helper class for composing a single instance.
    /// </summary>
    /// <typeparam name="T">Type of the object to compose.</typeparam>
    public sealed class SingleInstanceComposer<T> : ObjectBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleInstanceComposer{T}" /> class.
        /// </summary>
        /// <param name="container">The value for the <see cref="SingleInstanceComposer{T}.Container" /> property.</param>
        /// <param name="doRefresh">
        /// Do an initial call of <see cref="SingleInstanceComposer{T}.Refresh()" /> method or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container" /> is <see langword="null" />.
        /// </exception>
        public SingleInstanceComposer(CompositionContainer container,
                                      bool doRefresh = true)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.Container = container;

            if (doRefresh)
            {
                this.Refresh();
            }
        }

        #endregion Constructors (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying <see cref="CompositionContainer" /> instance..
        /// </summary>
        public CompositionContainer Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last created instance by <see cref="SingleInstanceComposer{T}.Container" />.
        /// </summary>
        [Import(AllowRecomposition = true)]
        public T Instance
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <summary>
        /// Refreshes the instance in <see cref="SingleInstanceComposer{T}.Instance" /> property.
        /// </summary>
        public void Refresh()
        {
            this.Container
                .ComposeParts(this);
        }

        #endregion Methods (1)
    }
}