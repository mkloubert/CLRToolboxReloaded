// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        // Public Methods (1) 

        /// <summary>
        ///
        /// </summary>
        /// <see cref="List{T}.AddRange(IEnumerable{T})" />
        public static void AddRange<T>(this ICollection<T> coll, IEnumerable<T> items)
        {
            if (coll == null)
            {
                throw new ArgumentNullException("coll");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (coll is global::System.Collections.Generic.List<T>)
            {
                // use build in method

                ((List<T>)coll).AddRange(items);
                return;
            }

            if (coll is global::MarcelJoachimKloubert.CLRToolbox.Collections.Generic.NullIndexDictionary<T>)
            {
                // use build in method

                ((NullIndexDictionary<T>)coll).AddRange(items);
                return;
            }

            ForEach(items,
                    action: (ctx) => ctx.State
                                        .Collection
                                        .Add(ctx.Item),
                    actionState: new
                        {
                            Collection = coll,
                        });
        }

        #endregion Methods
    }
}