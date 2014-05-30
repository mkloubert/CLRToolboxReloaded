﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

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

            var list = coll as List<T>;
            if (list != null)
            {
                // use build in method
                list.AddRange(items);
            }
            else
            {
                ForEach(items,
                        action: (ctx) => ctx.State
                                            .Collection
                                            .Add(ctx.Item),
                        actionState: new
                            {
                                Collection = coll,
                            });
            }
        }

        #endregion Methods
    }
}