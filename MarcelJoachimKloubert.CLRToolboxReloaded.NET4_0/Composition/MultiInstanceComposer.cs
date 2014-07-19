// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace MarcelJoachimKloubert.CLRToolbox.Composition
{
    /// <summary>
    /// A MEF helper class for composing multi instances.
    /// </summary>
    /// <typeparam name="T">Type of the objects to compose.</typeparam>
    public sealed class MultiInstanceComposer<T> : ObjectBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiInstanceComposer{T}" /> class.
        /// </summary>
        /// <param name="container">The value for the <see cref="MultiInstanceComposer{T}.Container" /> property.</param>
        /// <param name="doRefresh">
        /// Do an initial call of <see cref="MultiInstanceComposer{T}.Refresh()" /> method or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container" /> is <see langword="null" />.
        /// </exception>
        public MultiInstanceComposer(CompositionContainer container,
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

        #region Properties (2)

        /// <summary>
        /// Gets the underlying <see cref="CompositionContainer" /> instance.
        /// </summary>
        public CompositionContainer Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last created instance by <see cref="MultiInstanceComposer{T}.Container" />.
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public List<T> Instances
        {
            get;
            private set;
        }

        #endregion Properties (2)

        #region Methods (1)

        /// <summary>
        /// Refreshes the instance in <see cref="MultiInstanceComposer{T}.Instances" /> property.
        /// </summary>
        public void Refresh()
        {
            this.Container
                .ComposeParts(this);
        }

        #endregion Methods (1)
    }
}