// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Windows.Forms
{
    static partial class ClrToolboxWinFormsExtensionMethods
    {
        #region Methods (4)

        /// <summary>
        /// Returns a sequence of controls of a specific type from the collection of a control.
        /// </summary>
        /// <param name="ctrl">The underlying control.</param>
        /// <returns>The child controls of <paramref name="ctrl" />.</returns>
        /// <exception cref="NullReferenceException">
        /// <paramref name="ctrl" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<Control> EnumerateControls(this Control ctrl)
        {
            if (ctrl == null)
            {
                throw new ArgumentNullException("ctrl");
            }

            return EnumerateControls(coll: ctrl.Controls);
        }

        /// <summary>
        /// Returns a sequence of controls.
        /// </summary>
        /// <param name="coll">The underlying collection.</param>
        /// <returns>The controls.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="coll" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<Control> EnumerateControls(this Control.ControlCollection coll)
        {
            if (coll == null)
            {
                throw new ArgumentNullException("coll");
            }

            return EnumerateControls<Control>(coll: coll);
        }

        /// <summary>
        /// Returns a sequence of controls of a specific type from the collection of a control.
        /// </summary>
        /// <typeparam name="TCtrl">The type of controls that should be filtered out.</typeparam>
        /// <param name="ctrl">The underlying control.</param>
        /// <returns>The child controls of <paramref name="ctrl" />.</returns>
        /// <exception cref="NullReferenceException">
        /// <paramref name="ctrl" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<TCtrl> EnumerateControls<TCtrl>(this Control ctrl)
            where TCtrl : global::System.Windows.Forms.Control
        {
            return EnumerateControls<TCtrl>(coll: ctrl.Controls);
        }

        /// <summary>
        /// Returns a sequence of controls of a specific type.
        /// </summary>
        /// <typeparam name="TCtrl">The type of controls that should be filtered out.</typeparam>
        /// <param name="coll">The underlying collection.</param>
        /// <returns>The controls.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="coll" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<TCtrl> EnumerateControls<TCtrl>(this Control.ControlCollection coll)
            where TCtrl : global::System.Windows.Forms.Control
        {
            if (coll == null)
            {
                throw new ArgumentNullException("coll");
            }

            return coll.OfType<TCtrl>();
        }

        #endregion Methods (4)
    }
}